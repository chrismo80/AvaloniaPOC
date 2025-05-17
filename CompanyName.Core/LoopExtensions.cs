using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CompanyName.Core;

public static class LoopExtensions
{
	public static IEnumerable<int> To(this int from, int to)
	{
		if (from < to)
			while (from <= to)
				yield return from++;
		else
			while (from >= to)
				yield return from--;
	}

	public static IEnumerable<int> ToExcluding(this int from, int to) =>
		from.To(to - 1);

	public static IEnumerable<T> Step<T>(this IEnumerable<T> source, int step)
	{
		if (step == 0)
			throw new ArgumentException("Step cannot be zero.");

		return source.Where((_, i) => i % step == 0);
	}
}