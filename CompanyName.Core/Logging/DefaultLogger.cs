namespace CompanyName.Core.Logging;

public class DefaultLogger : ILogger
{
	public void Log(string text, LogLevel level = LogLevel.Debug)
	{
		System.Diagnostics.Trace.WriteLine(text, level.ToString());
	}
}