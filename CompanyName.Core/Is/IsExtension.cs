namespace CompanyName.Core.Is;

using System.Numerics;
using System.Collections;

public static class IsExtension
{
	public static T Is<T>(this object actual) =>
		actual is T cast ? cast : throw new Exception(actual.Actually("is no", typeof(T)));

	public static bool IsExactly(this object actual, object expected) =>
		actual.IsEqualTo(expected);

	public static bool Is(this object actual, params object[]? expected) =>
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
		values.Length == expected.Length ? expected.Length.For().All(i => values[i].Is(expected[i]))
			: throw new Exception(values.Actually("are not", expected));

	private static bool IsEqualTo<T>(this T? actual, T? expected) =>
		EqualityComparer<T>.Default.Equals(actual, expected) || actual.IsCloseTo(expected) ? true
			: throw new Exception(actual.Actually("is not", expected));

	private static bool IsCloseTo<T>(this T? actual, T? expected) =>
		(actual, expected) switch
		{
			(float a, float e) => a.IsInTolerance(e, 1e-6f),
			(double a, double e) => a.IsInTolerance(e, 1e-6),
			_ => false
		};

	private static bool IsInTolerance<T>(this T actual, T expected, T tolerance) where T : IFloatingPoint<T> =>
		T.Abs(actual - expected) <= tolerance * T.Max(T.One, T.Abs(expected)) ? true
			: throw new Exception(actual.Actually("is not close to", expected));

	private static string Actually(this object? actual, string equality, object? expected) =>
		actual.Format() + " actually " + equality + " " + expected.Format();

	private static string Format(this object? value) =>
		value.FormatValue() + value.FormatType();

	private static string FormatValue(this object? value) =>
		value switch
		{
			null => "<NULL>",
			string => $"\"{value}\"",
			IEnumerable list => $"[{list.FormatArray()}]",
			_ => $"{value}"
		};

	private static string FormatArray(this IEnumerable list) =>
		string.Join("|", list.Cast<object>().Select(x => x.FormatValue()));

	private static string FormatType(this object? value) =>
		value is null or Type ? "" : $" ({value.GetType()})";
}