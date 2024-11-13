using Microsoft.Extensions.Configuration;

namespace CompanyName.Core.Logging;

public class FileLogger : ILogger
{
	readonly string _sessionStarted = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

	readonly SemaphoreSlim _fileLock = new(1, 1);

	int _entriesCounter;

	public string LogDirectory { get; } = "Logs";
	public string Extension { get; private set; } = ".log";
	public string CurrentFileName { get; private set; } = "";

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

	public async Task Log(string text, LogLevel level = LogLevel.Debug)
	{
		if (level < LogLevel)
			return;

		string category = level.ToString().PadRight(12);

		await WriteToFile($"{DateTime.Now:HH:mm:ss.fff}\t{category}\t{text}{Environment.NewLine}");
	}

	private async Task WriteToFile(string line)
	{
		await _fileLock.WaitAsync().ConfigureAwait(false);

		if (++_entriesCounter >= MaxEntriesPerFile)
		{
			CurrentFileName = GetNextFileName();
			_entriesCounter = 0;
		}

		try
		{
			await File.AppendAllTextAsync(CurrentFileName, line).ConfigureAwait(false);
		}
		finally
		{
			_fileLock.Release();
		}
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
	}
}