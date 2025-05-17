using System.Collections;

namespace CompanyName.Core.Is;

public static class IsExtension
{
	public static T Is<T>(this object actual) =>
		actual is T cast ? cast : throw new IsNotException(actual, typeof(T));

	public static void Is(this object actual, params object[]? expected) =>
		actual.VerifyEqualityTo(expected?.Unwrap());

	private static void VerifyEqualityTo(this object actual, object[]? expected)
	{
		switch (expected?.Length)
		{
			case null: actual.ShouldBe(null); break;
			case 1: actual.ShouldBe(expected[0]); break;
			default: actual.ToArray().Are(expected); break;
		}
	}

	private static object[] Unwrap(this object[] array) =>
		array is [IEnumerable first and not string] ? first.ToArray() : array;

	private static object[] ToArray(this object value) =>
		Enumerable.ToArray(value.Is<IEnumerable>().Cast<object>());

	private static void Are(this object[] values, object[] expected)
	{
		if (values.Length != expected.Length)
			throw new IsNotException(values, expected);

		foreach (var i in expected.Length.For())
			values[i].Is(expected[i]);
	}

	private static void ShouldBe<T>(this T? actual, T? expected)
	{
		if (!EqualityComparer<T>.Default.Equals(actual, expected))
			throw new IsNotException(actual, expected);
	}
}