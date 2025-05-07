using System.Collections;

namespace UnitTests;

public class IsNotException(object? actual, object? expected)
	: Exception($"{Format(actual)} is not {Format(expected)}")
{
	private static string Format(object? value) =>
		$"{ValueOf(value)}{TypeOf(value)}";

	private static string ValueOf(object? value) => value switch
	{
		null => "<NULL>",
		string => $"\"{value}\"",
		IEnumerable enumerable => $"[{string.Join("|", enumerable.Cast<object>())}]",
		_ => $"{value}"
	};

	private static string TypeOf(object? value) =>
		value is null or Type ? "" : $" ({value.GetType()})";
}

public static class IsExtension
{
	public static void Is(this object actual, params object[]? expected)
	{
		expected = expected?.Unwrap();

		switch (expected?.Length)
		{
			case null: actual.IsEqualTo(null); break;
			case 1: actual.IsEqualTo(expected[0]); break;
			default: actual.CastToArray().Are(expected); break;
		}
	}

	public static T Is<T>(this object actual) =>
		actual is T cast ? cast : throw new IsNotException(actual, typeof(T));

	private static object[] Unwrap(this object[] array) =>
		array is [IEnumerable list and not string] ? list.CastToArray() : array;

	private static object[] CastToArray(this object values) =>
		values.Is<IEnumerable>().Cast<object>().ToArray();

	private static void Are(this object[] values, object[] expected)
	{
		if (values.Length != expected.Length)
			throw new IsNotException(values, expected);

		foreach (var i in Enumerable.Range(0, expected.Length))
			values[i].Is(expected[i]);
	}

	private static void IsEqualTo<T>(this T? actual, T? expected)
	{
		if (!EqualityComparer<T>.Default.Equals(actual, expected))
			throw new IsNotException(actual, expected);
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
	public void Actual_Equals_Expected(object actual, object expected) =>
		actual.Is(expected);

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
	public void Actual_Not_Equals_Expected(object actual, object expected)
	{
		actual.Is(expected);
		expected.Is(actual);
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

	[TestMethod]
	public void Value_Is_Type() =>
		new List<int>().Is<IReadOnlyList<int>>();

	[ExpectedException(typeof(IsNotException))]
	[TestMethod]
	public void Value_IsNot_Type() =>
		new List<int>().Is<IReadOnlyList<double>>();
}