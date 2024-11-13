using CompanyName.Core.Logging;

namespace CompanyName.Core;

public abstract class Manager : IDisposable
{
	private bool _disposed;

	protected Manager()
	{
		this.Trace("Ctor");
		Task.Delay(200).Wait();
	}

	public void Dispose()
	{
		if (_disposed)
			return;

		OnDisposing();

		GC.SuppressFinalize(this);

		_disposed = true;
	}

	protected virtual void OnDisposing() => this.Trace("Disposing");
}