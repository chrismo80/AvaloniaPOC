﻿namespace CompanyName.Core.Logging;

public enum LogLevel
{
	Trace,
	Debug,
	Information,
	Warning,
	Error,
	Critical,
}

public interface ILogger
{
	public Task Log(string text, LogLevel level = LogLevel.Debug);
}