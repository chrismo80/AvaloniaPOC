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
	}

	[TestCleanup]
	public void Cleanup()
	{
		if (Directory.Exists(_sut.LogDirectory))
			Directory.Delete(_sut.LogDirectory, true);
	}

	[TestMethod]
	[DataRow(10, 1, 1)]
	[DataRow(10, 3, 1)]
	[DataRow(10, 25, 3)]
	[DataRow(100, 25, 1)]
	[DataRow(100, 950, 10)]
	public void Log_MultipleFileMultipleEntries_CountMatches(
		int maxEntries, int entries, int files)
	{
		// Arrange
		_sut.MaxEntriesPerFile = maxEntries;

		// Act
		for (int i = 1; i < entries; i++)
			_ = _sut.Log("Entry " + i);

		_sut.Log("Last Entry").Wait();

		// Assert
		Assert.AreEqual((files, entries), CountFilesAndEntries());
	}

	[TestMethod]
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
			await _sut.Log("Entry " + i);
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
	public void Trace_WithExtension_Success()
	{
		this.Trace("Entry");
		Assert.AreEqual(default, CountFilesAndEntries());

		// this enables logging via object extension
		_sut.ConfigureTraceExtensions();

		this.Trace("Entry");

		// wait for async task to finish (not awaitable via extension)
		Thread.Sleep(1);
		Assert.AreEqual((1, 1), CountFilesAndEntries());
	}

	private (int Files, int Entries) CountFilesAndEntries()
	{
		var files = Directory.GetFiles(_sut.LogDirectory);
		var entries = files.Select(file => File.ReadLines(file).Count());

		return (files.Length, entries.Sum());
	}
}