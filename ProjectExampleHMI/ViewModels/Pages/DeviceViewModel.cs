using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using CompanyName.Core;
using CompanyName.Core.Devices;

namespace ProjectExampleHMI.ViewModels;

public partial class DeviceViewModel : CompanyName.UI.ViewModels.PageViewModel
{
	[ObservableProperty] ObservableCollection<BaseDevice> _devices = [];

	[ObservableProperty] int _selectedIndex;

	public DeviceViewModel()
	{
	}

	public DeviceViewModel(IServiceProvider provider)
	{
		Name = "Devices";
		Message = "This is the Devices ViewModel";

		Devices = new(provider.GetServicesThatImplement<BaseDevice>());
	}

	public void Reset()
	{
		if (SelectedIndex < Devices.Count)
			Devices[SelectedIndex].Reset();
	}
}