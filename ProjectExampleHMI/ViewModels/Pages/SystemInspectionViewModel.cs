using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CompanyName.UI.Devices;
using CompanyName.UI.Models;
using ProjectExampleHMI.Devices;
using ProjectExampleHMI.Models;

namespace ProjectExampleHMI.ViewModels;

public partial class SystemInspectionViewModel : CompanyName.UI.ViewModels.PageViewModel
{
	[ObservableProperty]
	ObservableCollection<InspectionModel> _models = [];

	[ObservableProperty]
	ObservableCollection<SyncDevice> _extSyncs = [];

	public SystemInspectionViewModel()
	{
	}

	public SystemInspectionViewModel(
		Inspection1 inspection1, SyncDevice1 extSync1,
		Inspection2 inspection2, SyncDevice2 extSync2,
		Inspection3 inspection3, SyncDevice3 extSync3
	)
	{
		Name = "Inspection";
		Message = "This is the System Inspection ViewModel";

		Models.Add(inspection1);
		Models.Add(inspection2);
		Models.Add(inspection3);

		ExtSyncs.Add(extSync1);
		ExtSyncs.Add(extSync2);
		ExtSyncs.Add(extSync3);
	}
}