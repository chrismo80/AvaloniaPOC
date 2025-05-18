using System.Collections;

namespace CompanyName.Core.Is;

public static class IsExtension
{
	public static T Is<T>(this object actual) =>
		actual is T cast ? cast : throw new IsNotException(actual, typeof(T));

	public static bool Is(this object actual, params object[]? expected) =>
		AreEqual(actual, expected) ? true : throw new IsNotException(actual, expected);

	public static bool IsNot(this object actual, params object[]? expected) =>
		!AreEqual(actual, expected) ? true : throw new IsException(actual, expected);

	private static bool AreEqual(object actual, params object[]? expected) =>
		actual.ShouldBe(expected?.Unwrap());

	private static bool ShouldBe(this object actual, object[]? expected) =>
		expected?.Length switch
		{
			null => actual.IsEqualTo(null),
			1 => actual.IsEqualTo(expected[0]),
			_ => actual.ToArray().Are(expected)
		};

	private static object[] Unwrap(this object[] array) =>
		array is [IEnumerable first and not string] ? first.ToArray() : array;

	private static object[] ToArray(this object value) =>
		Enumerable.ToArray(value.Is<IEnumerable>().Cast<object>());

	private static bool Are(this object[] values, object[] expected) =>
		values.Length == expected.Length && expected.Length.For().All(i => AreEqual(values[i], expected[i]));

	private static bool IsEqualTo<T>(this T? actual, T? expected) =>
		EqualityComparer<T>.Default.Equals(actual, expected);
}

public class IsException(object? actual, object? expected)
	: Exception(actual.Format() + " is " + expected.Format());

public class IsNotException(object? actual, object? expected)
	: Exception(actual.Format() + " is not " + expected.Format());

internal static class ExceptionFormatter
{
	internal static string Format(this object? value) =>
		value.Value() + value.Type();

	private static string Value(this object? value) => value switch
	{
		null => "<NULL>",
		string => $"\"{value}\"",
		IEnumerable enumerable => $"[{string.Join("|", enumerable.Cast<object>())}]",
		_ => $"{value}"
	};

	private static string Type(this object? value) =>
		value is null or System.Type ? "" : $" ({value.GetType()})";
}