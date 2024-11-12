using Microsoft.Extensions.Configuration;

namespace CompanyName.Core.Logging;

public class FileLogger : ILogger
{
    readonly string _directory;
    readonly string _extension;
    readonly string _sessionStarted;

    readonly LogLevel _logLevel = LogLevel.Debug;
    readonly SemaphoreSlim _fileLock = new(1, 1);

    readonly int _maxEntriesPerFile = 100_000;

    int _entriesCounter;
    string _currentFileName;

    public FileLogger(IConfiguration configuration)
    {
        var config = configuration.GetSection("Logging");

        var fileConfig = config["File:Path"]!;
        _ = Enum.TryParse(config["LogLevel:Default"]!, out _logLevel);
        _ = int.TryParse(config["File:MaxEntriesPerFile"]!, out _maxEntriesPerFile);

        _directory = Path.GetDirectoryName(fileConfig)!;
        _extension = Path.GetExtension(fileConfig);

        Directory.CreateDirectory(_directory);

        _sessionStarted = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        _currentFileName = GetNextFileName();
    }

    public async Task Log(string text, LogLevel level = LogLevel.Debug)
    {
        if (level < _logLevel)
            return;

        string category = level.ToString().PadRight(12);

        await WriteToFile($"{DateTime.Now:HH:mm:ss.fff}\t{category}\t{text}{Environment.NewLine}");
    }

    private async Task WriteToFile(string line)
    {
        await _fileLock.WaitAsync().ConfigureAwait(false);

        if (++_entriesCounter >= _maxEntriesPerFile)
        {
            _currentFileName = GetNextFileName();
            _entriesCounter = 0;
        }

        try
        {
            await File.AppendAllTextAsync(_currentFileName, line).ConfigureAwait(false);
        }
        finally { _fileLock.Release(); }
    }

    private string GetNextFileName()
    {
        var fileName = Path.Combine(_directory, _sessionStarted + _extension);

        if (!File.Exists(fileName))
            return fileName;

        int i = 1;

        while (File.Exists(fileName))
            fileName = Path.Combine(_directory, _sessionStarted + $".{i++}" + _extension);

        return fileName;
    }
}