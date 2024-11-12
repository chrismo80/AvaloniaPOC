namespace ProjectExampleHMI.ViewModels;

public partial class SystemViewModel : CompanyName.UI.ViewModels.NavigationPageViewModel
{
	public SystemViewModel()
	{
	}

	public SystemViewModel(
		SystemOverviewViewModel overviewScreen,
		SystemDetailsViewModel detailsScreen,
		SystemChartsViewModel chartsScreen,
		SystemInspectionViewModel inspectionsScreen)
		: base(overviewScreen, detailsScreen, chartsScreen, inspectionsScreen)
	{
		Name = "System";
		Message = "This is the System ViewModel";
	}
}