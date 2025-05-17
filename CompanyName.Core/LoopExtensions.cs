namespace CompanyName.Core;

public static class LoopExtensions
{
	public static IEnumerable<int> To(this int from, int to)
	{
		if (from < to)
			foreach (var i in from.Up(to))
				yield return i;

		if (from > to)
			foreach (var i in from.Down(to))
				yield return i;
	}

	public static IEnumerable<int> ToIncluding(this int from, int to)
	{
		if (from <= to)
			foreach (var i in from.Up(to + 1))
				yield return i;

		if (from >= to)
			foreach (var i in from.Down(to - 1))
				yield return i;
	}

	public static IEnumerable<int> For(this int to) => 0.To(to);

	public static IEnumerable<T> Step<T>(this IEnumerable<T> source, int step)
	{
		if (step == 0)
			throw new ArgumentException("Step cannot be zero.");

		return source.Where((_, i) => i % step == 0);
	}

	private static IEnumerable<int> Up(this int from, int to)
	{
		while (from < to)
			yield return from++;
	}

	private static IEnumerable<int> Down(this int from, int to)
	{
		while (from > to)
			yield return from--;
	}
}