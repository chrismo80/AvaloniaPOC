using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CompanyName.Core.Devices;
using CompanyName.UI.DataTemplates;
using ProjectExampleHMI.Devices;

namespace ProjectExampleHMI.ViewModels;

public partial class SystemDetailsViewModel : CompanyName.UI.ViewModels.PageViewModel
{
	readonly List<IDevice> _devices = [];

	public ObservableCollection<Axis> Axes { get; } = [];

	public SystemDetailsViewModel()
	{
		Name = "Details";
		Message = "This is the System Details ViewModel";
	}

	public SystemDetailsViewModel(DeviceX1 device1, DeviceY1 device2, DeviceX2 device3)
		: this()
	{
		Axes =
		[
			new("X-Achse", "Ready", 15.3),
			new("Y-Achse", "Error", 43.8),
			new("Z-Achse", "Busy", 29.4),
			new("Rotations-Achse", "Ready", 1.4),
			new("A-Achse", "Ready", 14.2),
			new("B-Achse", "Error", 51.2),
		];

		_devices.Add(device1);
		_devices.Add(device2);
		_devices.Add(device3);
	}

	public void SelectDevice1() => SelectDevice(1);

	public Task SelectDevice2() => SelectDeviceTask(2);

	public async Task SelectDevice3() => await SelectDeviceAsyncTask(3);

	public void SelectDevice4() => SelectDevice(4); // exception in sync thread

	private void SelectDevice(int deviceId)
	{
		var device = _devices.Find(d => d.Id == deviceId) ?? throw new NotSupportedException("Invalid device id");

		var anything = device.GetParameterValue("AnyName");

		var method = device.Id switch
		{
			1 => "DoStuff",
			2 => "DoThis",
			3 => "DoThat",
			_ => throw new NotImplementedException(),
		};

		device.Execute(method);

		Thread.Sleep(2000);

		Message = $"{device.Name}: {anything}";
	}

	private Task SelectDeviceTask(int deviceId)
	{
		SelectDevice(deviceId);

		return Task.CompletedTask;
	}

	private async Task SelectDeviceAsyncTask(int deviceId)
	{
		SelectDevice(deviceId);

		await Task.Delay(1);
	}
}