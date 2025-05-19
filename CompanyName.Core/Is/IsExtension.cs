using System.Collections;

namespace CompanyName.Core.Is;

public static class IsExtension
{
    public static T Is<T>(this object actual) =>
        actual is T cast ? cast : throw new Exception(actual.Actually("is not", typeof(T)));

    public static bool Is(this object actual, params object[]? expected) =>
        AreEqual(actual, expected) ? true : throw new Exception(actual.Actually("is not", expected));

    public static bool IsNot(this object actual, params object[]? expected) =>
        !AreEqual(actual, expected) ? true : throw new Exception(actual.Actually("is", expected));

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
        EqualityComparer<T>.Default.Equals(actual, expected) ? true : throw new Exception(actual.Actually("is not", expected));

    private static string Actually(this object? actual, string actually, object? expected) =>
        actual.Format() + " actually " + actually + " " + expected.Format();

    private static string Format(this object? value) =>
        value.FormatValue() + value.FormatType();

    private static string FormatValue(this object? value) =>
        value switch
        {
            null => "<NULL>",
            string => $"\"{value}\"",
            IEnumerable enumerable => $"[{string.Join("|", enumerable.Cast<object>())}]",
            _ => $"{value}"
        };

    private static string FormatType(this object? value) =>
        value is null or Type ? "" : $" ({value.GetType()})";
}