using System.Collections;

namespace CompanyName.Core.Is;

public class IsNotException(object? actual, object? expected)
    : Exception($"{Format(actual)} is not {Format(expected)}")
{
    private static string Format(object? value) =>
        ValueOf(value) + TypeOf(value);

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
            default: actual.ToArray().Are(expected); break;
        }
    }

    public static T Is<T>(this object actual) =>
        actual is T cast ? cast : throw new IsNotException(actual, typeof(T));

    private static object[] Unwrap(this object[] array) =>
        array.Length == 1 && array[0] is IEnumerable first && !(first is string) ? first.ToArray() : array;

    private static object[] ToArray(this object value) =>
        Enumerable.ToArray(value.Is<IEnumerable>().Cast<object>());

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