using System.Collections;

namespace UnitTests;

using Microsoft.VisualStudio.TestTools.UnitTesting;

public static class Extensions
{
	public static bool Is(this object? value, params object?[] expected)
	{
		if (expected[0] is IEnumerable enumerable and not string)
			expected = enumerable.Cast<object>().ToArray();

		return expected?.Length switch
		{
			null => value.IsEqualTo(expected),
			1 => value.IsEqualTo(expected[0]),
			_ => expected.Are(value as IEnumerable<object>)
		};
	}

	private static bool Are(this IEnumerable<object> expected, IEnumerable<object> values)
	{
		for (var i = 0; i < expected.Count(); i++)
			values.ElementAt(i).Is(expected.ElementAt(i));

		return true;
	}

	private static bool IsEqualTo(this object? value, object? expected)
	{
		Assert.AreEqual(expected, value);

		return true;
	}
}