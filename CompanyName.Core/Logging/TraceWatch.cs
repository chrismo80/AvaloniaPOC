using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CompanyName.Core.Logging;

public class TraceWatch : Stopwatch, IDisposable
{
	readonly object? _caller;

	public string Message { get; set; }

	public TraceWatch(object? caller = null, string? message = null, [CallerMemberName] string method = "")
	{
		_caller = caller ?? this;

		Message = message ?? $"-------------- Method '{method}'";

		//_caller.Trace($"{Message} started");

		Start();
	}

	public void Dispose()
	{
		Stop();

		//Export(Message, $"{Elapsed.TotalSeconds:F3}");

		_caller!.Trace($"{Message} end (Duration={Elapsed.TotalSeconds:F3}s)");
	}

	private void Export(string fileName, string data)
	{
		var file = Path.Combine("Durations", _caller.ToString(), fileName.ToValidFileName() + ".txt");

		Directory.CreateDirectory(Path.GetDirectoryName(file)!);
		File.AppendAllText(file, data + Environment.NewLine);
	}
}