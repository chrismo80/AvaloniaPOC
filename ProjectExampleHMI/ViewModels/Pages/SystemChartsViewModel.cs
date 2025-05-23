﻿using System.Collections.ObjectModel;
using CompanyName.UI.DataTemplates;
using ProjectExampleHMI.Devices;

namespace ProjectExampleHMI.ViewModels;

public partial class SystemChartsViewModel : CompanyName.UI.ViewModels.PageViewModel
{
	public ObservableCollection<Chart> Charts { get; } = [];

	public SystemChartsViewModel()
	{
	}

	public SystemChartsViewModel(DeviceX1 device1, DeviceX2 device2)
	{
		Name = "Charts";

		Charts =
		[
			new Chart(device1, $"{Assets.Resources.Force} [N]", "", 100, -10, 10, 2, 20),
			new Chart(device2, $"{Assets.Resources.Temperature} [°C]", "", 500, 15, 25, 1, 60),
		];
	}
}