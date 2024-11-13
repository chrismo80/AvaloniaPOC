using CompanyName.Core;
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
	public void Log_MultipleFileMultipleEntries_CountMatches(int maxEntries, int entries, int files)
	{
		// Arrange
		_sut.MaxEntriesPerFile = maxEntries;

		// Act
		for (int i = 1; i < entries; i++)
			_ = _sut.Log("Entry " + i);

		_sut.Log("Last Entry").Wait();

		// Assert
		var result = Count();

		Assert.AreEqual(files, result.Files);
		Assert.AreEqual(entries, result.Entries);
	}

	private (int Files, int Entries) Count()
	{
		var files = Directory.GetFiles(_sut.LogDirectory);

		var entries = files.Select(file => File.ReadLines(file).Count());

		return (files.Length, entries.Sum());
	}
}