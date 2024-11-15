using System.Text;
using Microsoft.Extensions.Configuration;

namespace CompanyName.Core.Logging;

public class FileLogger : ILogger
{
	readonly string _sessionStarted = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

	private readonly StringBuilder _cache = new StringBuilder();
	private readonly object _lock = new object();

	private System.Timers.Timer _flushTimer;

	int _entriesCounter;

	public string LogDirectory { get; } = "Logs";
	public string Extension { get; } = ".log";
	public string CurrentFileName { get; private set; } = "";
	public int FlushInterval => 1000;

	public LogLevel LogLevel { get; set; } = LogLevel.Debug;
	public int MaxEntriesPerFile { get; set; } = 100_000;

	public FileLogger()
	{
		Init();
	}

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

	public void Log(string text, LogLevel level = LogLevel.Debug)
	{
		if (level < LogLevel)
			return;

		string logEntry = $"{DateTime.Now:HH:mm:ss.fff}\t{level.ToString(),-12}\t{text}";

		lock (_lock)
		{
			_cache.AppendLine(logEntry);
		}
	}

	private void FlushLog()
	{
		if (_cache.Length == 0)
			return;

		string content;

		lock (_lock)
		{
			content = _cache.ToString();

			_cache.Clear();
		}

		_entriesCounter += content.Split(Environment.NewLine).Length;

		if (_entriesCounter >= MaxEntriesPerFile)
		{
			CurrentFileName = GetNextFileName();
			_entriesCounter = 0;
		}

		File.AppendAllText(CurrentFileName, content);
	}

	private string GetNextFileName()
	{
		var fileName = Path.Combine(LogDirectory, _sessionStarted + Extension);

		if (!File.Exists(fileName))
			return fileName;

		int i = 1;

		while (File.Exists(fileName))
			fileName = Path.Combine(LogDirectory, _sessionStarted + $".{i++}" + Extension);

		return fileName;
	}

	private void Init()
	{
		Directory.CreateDirectory(LogDirectory);

		CurrentFileName = GetNextFileName();

		_flushTimer = new System.Timers.Timer(FlushInterval);
		_flushTimer.Elapsed += (_, _) => FlushLog();
		_flushTimer.AutoReset = true;
		_flushTimer.Start();
	}
}