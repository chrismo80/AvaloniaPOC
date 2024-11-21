using CompanyName.Core.Messages;
using CompanyName.Core.Devices;
using CompanyName.Core.Models;
using CompanyName.Core.Logging;

namespace CompanyName.UI.Devices;

public abstract class SyncDevice(string plcRoot, IExecutable model) : BaseDevice
{
	public string PlcRoot { get; } = plcRoot;

	public IExecutable Model { get; } = model;

	public async void TriggerInspection()
	{
		using var tw = new TraceWatch(this);

		await Model.Execute();

		Executions++;

		this.CreateMessage("Model.Execute done", MessageType.Information);
	}
}