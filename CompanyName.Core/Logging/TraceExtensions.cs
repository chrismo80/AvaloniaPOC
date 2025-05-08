namespace CompanyName.Core.Logging;

public static class TraceExtensions
{
	const int TYPE_LENGTH = 70;

	static ILogger? _logger;

	/// <summary>
	/// Sets the Trace extensions to use this ILogger for tracing
	/// </summary>
	/// <param name="logger">the ILogger to use</param>
	public static void ConfigureTraceExtensions(this ILogger logger) =>
		_logger = logger;

	public static void Trace(this object me, string text, LogLevel level = LogLevel.Information) =>
		_logger?.Log(me.FormatMessage(text), level);

	public static void Trace(this object me, Exception ex) =>
		me.Trace(ex.ToString(), LogLevel.Error);

	public static string ToValidFileName(this string fileName)
	{
		foreach (char c in Path.GetInvalidFileNameChars())
			fileName = fileName.Replace(c, '-');

		return fileName;
	}

	private static string FormatMessage(this object me, string text) =>
		GetTypeText(me.GetType().FullName ?? "") + ":\t" + text;

	private static string GetTypeText(string type = "") =>
		type.Length < TYPE_LENGTH ? type.PadLeft(TYPE_LENGTH) : type[^TYPE_LENGTH..];

}