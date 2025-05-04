using System.Collections;

namespace UnitTests;

public class IsNotException : Exception
{
	public IsNotException(object? value, object? expected, string? message = null)
		: base($"{Format(value)} is not {Format(expected)}\n" + message)
	{ }

	public IsNotException(string message)
		: base(message)
	{ }

	private static string Format(object? value)
	{
		var text = value switch
		{
			null => "NULL",
			string => $"\"{value}\" ({value.GetType()})",
			IEnumerable enumerable => $"[{string.Join(", ", enumerable.Cast<object>())}]",
			_ => $"{value} ({value.GetType()})"
		};

		return text;
	}
}

public static class IsExtension
{
	/// <summary>
	/// checks if two objects are equal or sequence equal in cases of enumerables
	/// </summary>
	/// <returns>throws exception if not, never returns false</returns>
	public static bool Is(this object value, params object[]? expected)
	{
		expected = expected?.Unwrap();

		return expected?.Length switch
		{
			null => value.IsEqualTo(expected),
			1 => value.IsEqualTo(expected[0]),
			_ => value.ToEnumerable().ToArray().Are(expected)
		};
	}

	/// <summary>
	/// unwraps inner array of first element (due to params usage)
	/// use with caution for nested arrays, may lead to false positives
	/// </summary>
	/// <returns>the inner enumerable as array of the first element</returns>
	private static object[] Unwrap(this object[] array) =>
		array is [IEnumerable list and not string] ? list.ToEnumerable().ToArray() : array;

	/// <summary>
	/// checks each value of the lists on equality
	/// (uses recursion for nested lists)
	/// </summary>
	private static bool Are(this object[] values, object[] expected)
	{
		if (values.Length == expected.Length)
			return Enumerable.Range(0, expected.Length).All(i => values[i].Is(expected[i]));

		throw new IsNotException(values, expected);
	}

	private static IEnumerable<object> ToEnumerable(this object list)
	{
		if (list is IEnumerable enumerable)
			return enumerable.Cast<object>();

		throw new IsNotException($"{list} is not an IEnumerable");
	}

	private static bool IsEqualTo<T>(this T? value, T? expected, string? message = null)
	{
		if (EqualityComparer<T>.Default.Equals(expected, value))
			return true;

		throw new IsNotException(value, expected, message);
	}
}

[TestClass]
public class IsExtensionTests
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

	[ExpectedException(typeof(IsNotException))]
	[TestMethod]
	[DataRow(null, true)]
	[DataRow(null, false)]
	[DataRow(true, false)]
	[DataRow(5, null)]
	[DataRow(6, 6d)]
	[DataRow(7, true)]
	[DataRow(false, 7d)]
	[DataRow(8d, 8f)]
	[DataRow(99, "99")]
	[DataRow("ABC", false)]
	[DataRow("ABC", null)]
	[DataRow("ABC", "ABD")]
	public void Value_Not_Equals_Expected(object value, object expected)
	{
		value.Is(expected);
		expected.Is(value);
	}

	[TestMethod]
	public void ListValues_Equal_Expected() =>
		VerifyEquality(new List<int> { 1, 2, 3, 4 });

	[TestMethod]
	public void ArrayValues_Equal_Expected() =>
		VerifyEquality(new int[] { 1, 2, 3, 4 });

	private void VerifyEquality(IEnumerable<int> values)
	{
		values.Is(new int[] { 1, 2, 3, 4 });
		values.Is(new List<int> { 1, 2, 3, 4 });
		values.Is(1, 2, 3, 4);
		values.Where(i => i % 2 == 0).Is(2, 4);
	}

	[TestMethod]
	public void ValuesWithNull_Equal_Expected() =>
		new int?[] { 1, 2, null, 4 }.Is(1, 2, null, 4);

	[ExpectedException(typeof(IsNotException))]
	[TestMethod]
	public void List_Not_Equal_List() =>
		new List<int> { 1, 2, 3, 5 }.Is(new List<int> { 1, 2, 3, 4 });

	[ExpectedException(typeof(IsNotException))]
	[TestMethod]
	public void Array_Not_Equal_Params() =>
		new int[] { 1, 2, 3, 5 }.Is(1, 2, 3, 4);

	[ExpectedException(typeof(IsNotException))]
	[TestMethod]
	public void IEnumerable_Not_Equal_Params_TooShort() =>
		new List<int> { 1, 2, 3, 5 }.Where(i => i % 2 == 0).Is(2, 4);

	[ExpectedException(typeof(IsNotException))]
	[TestMethod]
	public void IEnumerable_Not_Equal_Params_TooLong() =>
		new List<int> { 1, 2, 3, 4, 5, 6 }.Where(i => i % 2 == 0).Is(2, 4);

	[TestMethod]
	public void JaggedArrays_Equals_Expected() =>
		new object[] { new[] { 1, 2 }, 3 }.Is(new object[] { new[] { 1, 2 }, 3 });

	[TestMethod]
	public void DifferentDepth_EqualsThough_Expected() =>
		new List<object> { 1, 2 }.Is(new List<object> { 1, new List<object> { 2 } });

	[ExpectedException(typeof(IsNotException))]
	[TestMethod]
	public void Value_NotEquals_List() =>
		5.Is(new List<int> { 1, 2 });
}