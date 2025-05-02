using System.Collections;

namespace UnitTests;

public static class TestExtensions
{
	/// <summary>
	/// checks if two objects are equal or have equal values in cases of enumerables
	/// </summary>
	/// <returns>throws exception if not, never returns false</returns>
	public static bool Is(this object value, params object[]? expected)
	{
		expected = expected?.Unwrap();

		return expected?.Length switch
		{
			null => value.IsEqualTo(expected),
			1 => value.IsEqualTo(expected[0]),
			_ => value.ToEnumerable().Are(expected)
		};
	}

	/// <summary>
	/// unwraps inner array of first element (due to params usage)
	/// use with caution for nested arrays, may lead to false positives
	/// </summary>
	/// <returns>the inner enumerable as array of the first element</returns>
	private static object[]? Unwrap(this object[] array) =>
		array is [IEnumerable list and not string] ? list.ToEnumerable().ToArray() : array;

	private static IEnumerable<object> ToEnumerable(this object list)
	{
		Assert.IsInstanceOfType(list, typeof(IEnumerable), $"{list} is not an IEnumerable");

		return (list as IEnumerable).Cast<object>();
	}

	/// <summary>
	/// checks each value of the lists on equality
	/// (uses recursion for nested lists)
	/// </summary>
	private static bool Are(this IEnumerable<object> values, IEnumerable<object> expected)
	{
		var (valuesList, expectedList) = (values.ToList(), expected.ToList());

		Assert.AreEqual(expectedList.Count, valuesList.Count,
			$"Count mismatch\n{valuesList.Format()} != {expectedList.Format()}");

		return Enumerable.Range(0, expectedList.Count).All(i => valuesList[i].Is(expectedList[i]));
	}

	private static bool IsEqualTo(this object? value, object? expected)
	{
		Assert.AreEqual(expected, value);

		return true;
	}

	private static string Format(this IEnumerable<object> values) => string.Join("|", values);
}

[TestClass]
public class TestExtensionTests
{
	[TestMethod]
	[DataRow(null, null)]
	[DataRow(false, false)]
	[DataRow(true, true)]
	[DataRow(1, 1)]
	[DataRow(2.2, 2.2)]
	[DataRow(3f, 3f)]
	[DataRow("4", "4")]
	public void Value_Equals_Expected(object value, object expected) =>
		value.Is(expected);

	[TestMethod]
	[ExpectedException(typeof(AssertFailedException))]
	[DataRow(null, true)]
	[DataRow(null, false)]
	[DataRow(true, false)]
	[DataRow(5, null)]
	[DataRow(6, 6d)]
	[DataRow(7, true)]
	[DataRow(false, 7d)]
	[DataRow(8d, 8f)]
	[DataRow(9, "9")]
	[DataRow("ABC", false)]
	[DataRow("ABC", null)]
	[DataRow("ABC", "ABD")]
	public void Value_Not_Equals_Expected(object value, object expected)
	{
		value.Is(expected);
		expected.Is(value);
	}

	[TestMethod]
	public void ListValues_Equal_Expected()
	{
		var values = new List<int> { 1, 2, 3, 4 };

		values.Is(new int[] { 1, 2, 3, 4 });
		values.Is(new List<int> { 1, 2, 3, 4 });
		values.Is(1, 2, 3, 4);
		values.Where(i => i % 2 == 0).Is(2, 4);
	}

	[TestMethod]
	public void ArrayValues_Equal_Expected()
	{
		var values = new int[] { 1, 2, 3, 4 };

		values.Is(new int[] { 1, 2, 3, 4 });
		values.Is(new List<int> { 1, 2, 3, 4 });
		values.Is(1, 2, 3, 4);
		values.Where(i => i % 2 == 0).Is(2, 4);
	}

	[TestMethod]
	[ExpectedException(typeof(AssertFailedException))]
	public void List_Not_Equal_List() =>
		new List<int> { 1, 2, 3, 5 }.Is(new List<int> { 1, 2, 3, 4 });

	[TestMethod]
	[ExpectedException(typeof(AssertFailedException))]
	public void Array_Not_Equal_Params() =>
		new int[] { 1, 2, 3, 5 }.Is(1, 2, 3, 4);

	[TestMethod]
	[ExpectedException(typeof(AssertFailedException))]
	public void IEnumerable_Not_Equal_Params_TooLong() =>
		new List<int> { 1, 2, 3, 5 }.Where(i => i % 2 == 0).Is(2, 4);

	[TestMethod]
	[ExpectedException(typeof(AssertFailedException))]
	public void IEnumerable_Not_Equal_Params_TooShort() =>
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4);

	[TestMethod]
	public void JaggedArrays_Equals_Expected()
	{
		var values = new object[] { new int[] { 1, 2 }, 3 };
		var expected = new object[] { new int[] { 1, 2 }, 3 };

		values.Is(expected);
	}

	[TestMethod]
	public void DifferentDepth_EqualsThough_Expected()
	{
		var values = new List<object> { 1, 2 };
		var expected = new List<object> { 1, new List<object> { 2 } };

		values.Is(expected);
	}

	[ExpectedException(typeof(AssertFailedException))]
	[TestMethod]
	public void Value_NotEquals_List()
	{
		var value = 5;
		var expected = new List<int> { 1, 2 };

		value.Is(expected);
	}
}