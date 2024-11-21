using CompanyName.Core.Logging;

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
			for (int i = 1; i <= count; i++)
				Sut.Log("Entry " + i, level);
		}

		CreateLogs(2, LogLevel.Trace);
		CreateLogs(3, LogLevel.Debug);
		CreateLogs(4, LogLevel.Information);
		CreateLogs(5, LogLevel.Warning);
		CreateLogs(6, LogLevel.Error);
		CreateLogs(7, LogLevel.Critical);

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
		Sut.MaxEntriesPerFile = maxEntries;

		// Act
		for (int i = 1; i <= entries; i++)
		{
			Sut.Log("Entry " + i);

			if (i % 123 == 0) // add some random pauses
				Task.Delay(Sut.FlushInterval).Wait();
		}

		// Assert
		Assert.AreEqual((files, entries), CountFilesAndEntries());
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
	[DataRow(10, 42, 5)]
	public void Log_MultipleExceptionMessages_ProperPageBreaks(
		int maxEntries, int entries, int files)
	{
		Sut.MaxEntriesPerFile = maxEntries;

		var inner = new Exception("inner ex");
		var ex = new AggregateException(inner, inner, inner);

		// 1 trace creates 3 lines per entry
		for (int i = 1; i <= entries; i++)
			this.Trace(ex);

		Assert.AreEqual((files, entries * 3), CountFilesAndEntries());
	}

	[TestMethod]
	[DoNotParallelize]
	public void TraceWatch_SimpleCall_VerifyLoggedRowCount()
	{
		using (var tw = new TraceWatch(this))
		{
			Assert.AreEqual((1, 1), CountFilesAndEntries());
		}

		Assert.AreEqual((1, 2), CountFilesAndEntries());
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
		if (Directory.Exists(Sut.LogDirectory))
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