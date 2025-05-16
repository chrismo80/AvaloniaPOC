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