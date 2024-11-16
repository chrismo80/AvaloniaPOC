namespace CompanyName.Core.Logging;

public class FileLogger : BaseService, ILogger
{
	readonly string _sessionStarted = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

	private readonly List<string> _cache = [];
	private readonly object _lock = new();

	private readonly System.Timers.Timer _flushTimer = new();

	int _entriesCounter;

	public string LogDirectory { get; } = "Logs";

	public string Extension { get; } = ".log";

	public string CurrentFileName { get; private set; } = "";

	public int FlushInterval { get; } = 1_000;

	public LogLevel LogLevel { get; internal set; } = LogLevel.Debug;

	public int MaxEntriesPerFile { get; internal set; } = 100_000;

	public FileLogger(Microsoft.Extensions.Configuration.IConfiguration configuration)
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

	// ctor for unit tests only
	internal FileLogger()
	{
		// small interval for unit tests
		FlushInterval = 42;

		Init();
	}

	public void Log(string text, LogLevel level = LogLevel.Debug)
	{
		if (level < LogLevel)
			return;

		string logEntry = $"{DateTime.Now:HH:mm:ss.fff}\t{level,-12}\t{text}";

		lock (_lock)
		{
			_cache.Add(logEntry);
		}
	}

	protected override void OnDisposing()
	{
		_flushTimer.Stop();
		_flushTimer.Dispose();
	}

	private void FlushLog()
	{
		if (_cache.Count == 0)
			return;

		string[] entries;

		lock (_lock)
		{
			entries = _cache.ToArray();

			_cache.Clear();
		}

		// check if cache larger than remaining space in current file
		while (_entriesCounter + entries.Length > MaxEntriesPerFile)
			entries = FinishCurrentFile(entries);

		_entriesCounter += entries.Length;

		WriteToFile(entries);
	}

	private string[] FinishCurrentFile(string[] entries)
	{
		int spaceLeft = MaxEntriesPerFile - _entriesCounter;

		// finish current file and create new filename, reset line counter
		WriteToFile(entries[..spaceLeft]);
		CurrentFileName = GetNextFileName();
		_entriesCounter = 0;

		return entries[spaceLeft..];
	}

	private void WriteToFile(string[] lines) =>
		File.AppendAllLines(CurrentFileName, lines);

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

		_flushTimer.Elapsed += (_, _) => FlushLog();
		_flushTimer.Interval = FlushInterval;
		_flushTimer.AutoReset = true;
		_flushTimer.Start();
	}
}