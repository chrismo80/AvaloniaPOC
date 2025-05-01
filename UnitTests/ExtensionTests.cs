namespace UnitTests;

[TestClass]
public class ExtensionTests
{
	[TestMethod]
	[DataRow(null, null)]
	[DataRow(false, false)]
	[DataRow(true, true)]
	[DataRow(1, 1)]
	[DataRow(2.2, 2.2)]
	[DataRow(3f, 3f)]
	[DataRow("4", "4")]
	public void Value_Equals_Expected(object? value, object? expected) =>
		value.Is(expected);

	[TestMethod]
	[ExpectedException(typeof(AssertFailedException))]
	[DataRow(5, null)]
	[DataRow(6, 6d)]
	[DataRow(7, true)]
	[DataRow(8d, 8f)]
	public void Value_Not_Equals_Expected(object? value, object? expected)
	{
		value.Is(expected);
		expected.Is(value);
	}

	[TestMethod]
	public void Values_Equal_Expected()
	{
		var values = new List<int> { 1, 2, 3, 4 };
		var expected = new List<int> { 1, 2, 3, 4 };

		values.Is(expected);
		values.Is(1, 2, 3, 4);
		values.Where(i => i % 2 == 0).Is(2, 4);
	}

	[TestMethod]
	[ExpectedException(typeof(Exception))]
	public void Values_Not_Equal_Expected_List()
	{
		var values = new List<int> { 1, 2, 3, 5 };
		var expected = new List<int> { 1, 2, 3, 4 };

		values.Is(expected);
	}

	[TestMethod]
	[ExpectedException(typeof(AssertFailedException))]
	public void Values_Not_Equal_Expected_Params()
	{
		var values = new List<int> { 1, 2, 3, 5 };

		values.Is(1, 2, 3, 4);
	}

	[TestMethod]
	[ExpectedException(typeof(AssertFailedException))]
	public void Values_Not_Equal_Expected_Params2()
	{
		var values = new List<int> { 1, 2, 3, 5 };

		values.Where(i => i % 2 == 0).Is(2, 4);
	}
}