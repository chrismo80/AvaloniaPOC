using System.Runtime.CompilerServices;

namespace CompanyName.Core.Logging;

public class TraceWatch : IDisposable
{
	readonly System.Diagnostics.Stopwatch _stopwatch;

	readonly string _message;
	readonly object? _caller;

	public TraceWatch(object? caller = null, string? message = null, [CallerMemberName] string method = "")
	{
		_message = message ?? $"Method '{method}'";
		_caller = caller;

		(_caller ?? this).Trace($"{_message} begin");

		_stopwatch = System.Diagnostics.Stopwatch.StartNew();
	}

	public void Dispose() =>
		(_caller ?? this).Trace($"{_message} end (Duration={_stopwatch.Elapsed.TotalSeconds:F3}s)");
}