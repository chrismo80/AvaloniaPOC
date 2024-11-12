namespace ProjectExampleHMI.ViewModels;

public partial class WizardViewModel : CompanyName.UI.ViewModels.NavigationPageViewModel
{
    public WizardViewModel()
    {
        Name = "Wizard";
        Message = "This is the Wizard ViewModel";
    }

    public WizardViewModel(
        Page1ViewModel page1,
        Page2ViewModel page2,
        Page3ViewModel page3)
        : base(page1, page2, page3)
    {
        Name = "Wizard";
        Message = "This is the Wizard ViewModel";
    }
}