using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;

namespace CompanyName.Core.Logging;

public class FileLogger : BaseService, ILogger
{
	readonly string _started = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

	readonly ConcurrentQueue<string> _cache = new();
	readonly System.Timers.Timer _flushTimer = new();

	int _fileCounter = 1;
	int _entriesCounter;

	public string LogDirectory { get; } = "Logs";

	public string Extension { get; } = ".log";

	public string CurrentFileName { get; private set; } = "";

	public int FlushInterval { get; internal set; } = 1_000;

	public LogLevel LogLevel { get; internal set; } = LogLevel.Debug;

	public int MaxEntriesPerFile { get; internal set; } = 100_000;

	public FileLogger(IConfiguration configuration)
	{
		var config = configuration.GetSection("Logging");

		var fileConfig = config["File:Path"]!;
		_ = Enum.TryParse(config["LogLevel:Default"]!, out LogLevel logLevel);
		_ = int.TryParse(config["File:MaxEntriesPerFile"]!, out int maxEntriesPerFile);

		LogLevel = logLevel;
		MaxEntriesPerFile = maxEntriesPerFile;

		LogDirectory = Path.GetDirectoryName(fileConfig)!;
		Extension = Path.GetExtension(fileConfig);

		Init();
	}

	internal FileLogger()
	{
	}

	internal void Init()
	{
		Directory.CreateDirectory(LogDirectory);

		CurrentFileName = Path.Combine(LogDirectory, _started + Extension);

		_flushTimer.Elapsed += (_, _) => FlushCache();
		_flushTimer.Interval = FlushInterval;
		_flushTimer.Start();
	}

	public void Log(string text, LogLevel level = LogLevel.Debug)
	{
		if (level < LogLevel)
			return;

		string logEntry = $"{DateTime.Now:HH:mm:ss.fff}\t{level,-12}\t{text}";

		_cache.Enqueue(logEntry);
	}

	protected override void OnDisposing()
	{
		if (!_flushTimer.Enabled)
			return;

		_flushTimer.Stop();
		_flushTimer.Dispose();
	}

	private void FlushCache()
	{
		if (_cache.IsEmpty)
			return;

		var entries = ReadCache();

		WriteToFile(entries);
	}

	private Span<string> ReadCache()
	{
		var entriesToWrite = new List<string>(_cache.Count);

		while (_cache.TryDequeue(out var logEntry))
			entriesToWrite.Add(logEntry);

		return entriesToWrite.ToArray().AsSpan();
	}

	private void WriteToFile(Span<string> entries)
	{
		int spaceLeft;

		while ((spaceLeft = MaxEntriesPerFile - _entriesCounter) < entries.Length)
			entries = FinishCurrentFile(entries, spaceLeft);

		if (entries.IsEmpty)
			return;

		_entriesCounter += entries.Length;

		AppendToFile(entries);
	}

	private Span<string> FinishCurrentFile(Span<string> entries, int spaceLeft)
	{
		AppendToFile(entries[..spaceLeft]);

		CurrentFileName = Path.Combine(LogDirectory, _started + $".{_fileCounter++}" + Extension);

		_entriesCounter = 0;

		return entries[spaceLeft..];
	}

	private void AppendToFile(Span<string> logEntries) =>
		File.AppendAllLines(CurrentFileName, logEntries.ToArray());
}