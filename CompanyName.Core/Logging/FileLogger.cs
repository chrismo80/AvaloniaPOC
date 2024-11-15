using System.Text;
using Microsoft.Extensions.Configuration;

namespace CompanyName.Core.Logging;

public class FileLogger : ILogger
{
    readonly string _sessionStarted = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

    private readonly StringBuilder _cache = new();
    private readonly object _lock = new();

    private readonly System.Timers.Timer _flushTimer = new();

    int _entriesCounter;

    public string LogDirectory { get; } = "Logs";

    public string Extension { get; } = ".log";

    public string CurrentFileName { get; private set; } = "";

    public int FlushInterval { get; } = 1_000;

    public LogLevel LogLevel { get; set; } = LogLevel.Debug;

    public int MaxEntriesPerFile { get; set; } = 100_000;

    // ctor for unit tests only
    public FileLogger()
    {
        // small interval for unit tests
        FlushInterval = 42;

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

        string logEntry = $"{DateTime.Now:HH:mm:ss.fff}\t{level,-12}\t{text}";

        lock (_lock)
        {
            _cache.AppendLine(logEntry);
        }
    }

    private void FlushLog()
    {
        if (_cache.Length == 0)
            return;

        string contentToFlush;

        lock (_lock)
        {
            contentToFlush = _cache.ToString();

            _cache.Clear();
        }

        var lines = contentToFlush.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        while (_entriesCounter + lines.Length > MaxEntriesPerFile)
        {
            int spaceLeft = MaxEntriesPerFile - _entriesCounter;

            WriteToFile(lines.Take(spaceLeft).ToArray());

            lines = lines.Skip(spaceLeft).ToArray();

            CurrentFileName = GetNextFileName();

            _entriesCounter = 0;
        }

        _entriesCounter += lines.Length;

        WriteToFile(lines);
    }

    private void WriteToFile(params string[] lines) => File.AppendAllLines(CurrentFileName, lines);

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