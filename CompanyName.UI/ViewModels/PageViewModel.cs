using CommunityToolkit.Mvvm.ComponentModel;

namespace CompanyName.UI.ViewModels;

public abstract partial class PageViewModel : BaseViewModel
{
	[ObservableProperty]
	string _message = "";

	[ObservableProperty]
	string _name = "";
}