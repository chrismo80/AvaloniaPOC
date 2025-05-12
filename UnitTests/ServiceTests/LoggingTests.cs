using CompanyName.Core.Logging;
using CompanyName.Core.Is;

namespace UnitTests.ServiceTests;

[TestClass]
public class LoggingTests : ServiceTests<FileLogger>
{
    [TestMethod]
    [DoNotParallelize]
    [DataRow(LogLevel.Trace, 27)]
    [DataRow(LogLevel.Debug, 25)]
    [DataRow(LogLevel.Information, 22)]
    [DataRow(LogLevel.Warning, 18)]
    [DataRow(LogLevel.Error, 13)]
    [DataRow(LogLevel.Critical, 7)]
    public void Log_ChangeLogLevel_CountMatches(
        LogLevel logLevel, int entries)
    {
        Sut.MaxEntriesPerFile = 100;
        Sut.LogLevel = logLevel;

        void CreateLogs(int count, LogLevel level)
        {
            for (var i = 1; i <= count; i++)
                Sut.Log("Entry " + i, level);
        }

        CreateLogs(2, LogLevel.Trace);
        CreateLogs(3, LogLevel.Debug);
        CreateLogs(4, LogLevel.Information);
        CreateLogs(5, LogLevel.Warning);
        CreateLogs(6, LogLevel.Error);
        CreateLogs(7, LogLevel.Critical);

        CountFilesAndEntries().Is((1, entries));
    }

    [TestMethod]
    [DoNotParallelize]
    [DataRow(10, 1, 1)]
    [DataRow(10, 3, 1)]
    [DataRow(10, 25, 3)]
    [DataRow(10, 42, 5)]
    [DataRow(100, 25, 1)]
    [DataRow(100, 950, 10)]
    public async Task Log_MultipleFileMultipleEntries_CountMatches(
        int maxEntries, int entries, int files)
    {
        // Arrange
        Sut.MaxEntriesPerFile = maxEntries;

        // Act
        for (var i = 1; i <= entries; i++)
        {
            Sut.Log("Entry " + i);

            if (i % 123 == 0) // add some random pauses
                await Task.Delay(Sut.FlushInterval);
        }

        // Assert
        CountFilesAndEntries().Is((files, entries));
    }

    [TestMethod]
    [DoNotParallelize]
    [DataRow(1_000, 500, 1)]
    public async Task Log_Concurrency_NoRaceConditions(
        int maxEntries, int entries, int files)
    {
        Sut.MaxEntriesPerFile = maxEntries;

        // A trigger for all tasks to start simultaneously
        var trigger = new TaskCompletionSource();

        async Task LogWhenTriggered(Task go, int i)
        {
            // Wait for the trigger to start
            await go;
            Sut.Log("Entry " + i);
        }

        var tasks = Enumerable.Range(1, entries)
            .Select(i => LogWhenTriggered(trigger.Task, i));

        // Set trigger
        trigger.SetResult();

        // Wait for all tasks to be finished
        await Task.WhenAll(tasks);

        CountFilesAndEntries().Is((files, entries));
    }

    [TestMethod]
    [DoNotParallelize]
    public void Trace_WithExtension_Success()
    {
        this.Trace("Entry via Extension");
        CountFilesAndEntries().Is((1, 1));
    }

    [TestMethod]
    [DoNotParallelize]
    public void Log_ExceptionMessage_Success()
    {
        this.Trace(new Exception("Test"));
        CountFilesAndEntries().Is((1, 1));
    }

    [TestMethod]
    [DoNotParallelize]
    [DataRow(10, 42, 5)]
    public void Log_MultipleExceptionMessages_ProperPageBreaks(
        int maxEntries, int entries, int files)
    {
        Sut.MaxEntriesPerFile = maxEntries;

        var inner = new Exception("inner ex");
        var ex = new AggregateException(inner, inner, inner);

        // 1 trace creates 3 lines per entry
        for (var i = 1; i <= entries; i++)
            this.Trace(ex);

        CountFilesAndEntries().Is((files, entries * 3));
    }

    [TestMethod]
    [DoNotParallelize]
    public void TraceWatch_SimpleCall_VerifyLoggedRowCount()
    {
        using (var tw = new TraceWatch(this))
        {
            CountFilesAndEntries().Is((1, 1));
        }

        CountFilesAndEntries().Is((1, 2));
    }

    [TestMethod]
    [DoNotParallelize]
    public void AppendToFile_HandleException_NewFileCreated()
    {
        Sut.Log("Entry 1");

        CountFilesAndEntries().Files.Is(1);

        // force AccessException
        File.SetAttributes(Sut.CurrentFileName, FileAttributes.ReadOnly);

        Sut.Log("Entry 2");

        CountFilesAndEntries().Files.Is(2);
    }

    protected override void Pre()
    {
        Sut.FlushInterval = 42;
        Sut.Init();

        // this enables logging via object extension
        Sut.ConfigureTraceExtensions();
    }

    protected override void Post()
    {
        // remove log dir, because all unit tests use the same dir, must be clean
        CleanUpLogDirectory();
    }

    private void CleanUpLogDirectory()
    {
        if (!Directory.Exists(Sut.LogDirectory))
            return;

        foreach (var file in Directory.GetFiles(Sut.LogDirectory))
        {
            File.SetAttributes(file, FileAttributes.Normal); // Reset attributes
            File.Delete(file);
        }

        Directory.Delete(Sut.LogDirectory, true);
    }

    private (int Files, int Entries) CountFilesAndEntries()
    {
        // wait for logger to finish flushing
        Task.Delay(Sut.FlushInterval * 2).Wait();

        var files = Directory.GetFiles(Sut.LogDirectory);
        var entries = files.Select(file => File.ReadLines(file).Count());

        return (files.Length, entries.Sum());
    }
}