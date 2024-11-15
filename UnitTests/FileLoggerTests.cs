using CompanyName.Core.Logging;

namespace UnitTests;

[TestClass]
public class FileLoggerTests
{
	FileLogger _sut = null!;

	[TestInitialize]
	public void Initialize()
	{
		_sut = new FileLogger();

		// this enables logging via object extension
		_sut.ConfigureTraceExtensions();
	}

	[TestCleanup]
	public void Cleanup()
	{
		// remove log dir, because all unit tests use the same dir, must be clean
		if (Directory.Exists(_sut.LogDirectory))
			Directory.Delete(_sut.LogDirectory, true);
	}

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
		_sut.LogLevel = logLevel;

		for (int i = 1; i <= 2; i++)
			_sut.Log("Entry " + i, LogLevel.Trace);

		for (int i = 1; i <= 3; i++)
			_sut.Log("Entry " + i, LogLevel.Debug);

		for (int i = 1; i <= 4; i++)
			_sut.Log("Entry " + i, LogLevel.Information);

		for (int i = 1; i <= 5; i++)
			_sut.Log("Entry " + i, LogLevel.Warning);

		for (int i = 1; i <= 6; i++)
			_sut.Log("Entry " + i, LogLevel.Error);

		for (int i = 1; i <= 7; i++)
			_sut.Log("Entry " + i, LogLevel.Critical);

		Assert.AreEqual((1, entries), CountFilesAndEntries());
	}

	[TestMethod]
	[DoNotParallelize]
	[DataRow(10, 1, 1)]
	[DataRow(10, 3, 1)]
	[DataRow(10, 25, 3)]
	[DataRow(10, 42, 5)]
	[DataRow(100, 25, 1)]
	[DataRow(100, 950, 10)]
	public void Log_MultipleFileMultipleEntries_CountMatches(
		int maxEntries, int entries, int files)
	{
		// Arrange
		_sut.MaxEntriesPerFile = maxEntries;

		// Act
		for (int i = 1; i <= entries; i++)
		{
			_sut.Log("Entry " + i);

			if (i % 123 == 0) // add some random pauses
				Task.Delay(_sut.FlushInterval).Wait();
		}

		// Assert
		Assert.AreEqual((files, entries), CountFilesAndEntries());
	}

	[TestMethod]
	[DoNotParallelize]
	[DataRow(1_000, 500, 1)]
	public void Log_Concurrency_NoRaceConditions(
		int maxEntries, int entries, int files)
	{
		_sut.MaxEntriesPerFile = maxEntries;

		// A trigger for all tasks to start simultaneously
		var trigger = new TaskCompletionSource();

		async Task LogWhenTriggered(Task go, int i)
		{
			// Wait for the trigger to start
			await go;
			_sut.Log("Entry " + i);
		}

		var tasks = Enumerable.Range(1, entries)
			.Select(i => LogWhenTriggered(trigger.Task, i));

		// Set trigger
		trigger.SetResult();

		// Wait for all tasks to be finished
		Task.WhenAll(tasks).Wait();

		Assert.AreEqual((files, entries), CountFilesAndEntries());
	}

	[TestMethod]
	[DoNotParallelize]
	public void Trace_WithExtension_Success()
	{
		this.Trace("Entry via Extension");
		Assert.AreEqual((1, 1), CountFilesAndEntries());
	}

	[TestMethod]
	[DoNotParallelize]
	public void Log_ExceptionMessage_Success()
	{
		this.Trace(new Exception("Test"));
		Assert.AreEqual((1, 1), CountFilesAndEntries());
	}

	[TestMethod]
	[DoNotParallelize]
	[DataRow(10, 42, 13)]
	public void Log_MultipleExceptionMessages_ProperPageBreaks(
		int maxEntries, int entries, int files)
	{
		_sut.MaxEntriesPerFile = maxEntries;

		var inner = new Exception("inner ex");
		var ex = new AggregateException(inner, inner, inner);

		for (int i = 1; i <= entries; i++)
			this.Trace(ex);

		Assert.AreEqual((files, entries * 3), CountFilesAndEntries());
	}

	private (int Files, int Entries) CountFilesAndEntries()
	{
		// wait for logger to finish flushing
		Task.Delay(_sut.FlushInterval * 2).Wait();

		var files = Directory.GetFiles(_sut.LogDirectory);
		var entries = files.Select(file => File.ReadLines(file).Count());

		return (files.Length, entries.Sum());
	}
}