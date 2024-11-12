using Avalonia.Controls.ApplicationLifetimes;

using CommunityToolkit.Mvvm.ComponentModel;

using System;

using CompanyName.UI;
using CompanyName.Core.Auth;

namespace ProjectExampleHMI.ViewModels;

public partial class MainWindowViewModel : CompanyName.UI.ViewModels.NavigationPageViewModel
{
    readonly IServiceProvider? _serviceProvider;
    readonly IAuthManager? _authManager;

    [ObservableProperty]
    HeaderViewModel? _header;

    public string User => _authManager?.User ?? "";

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(IServiceProvider serviceProvider, IAuthManager authManager,
        HeaderViewModel header,
        AutomationViewModel automationScreen,
        WizardViewModel wizardScreen,
        SystemViewModel systemScreen,
        MessagesViewModel messagesScreen,
        DeviceViewModel deviceScreen)
        : base(
            automationScreen,
            systemScreen,
            wizardScreen,
            messagesScreen,
            deviceScreen)
    {
        _header = header;
        _serviceProvider = serviceProvider;
        _authManager = authManager;

        _authManager.UserChanged += (_, _) => OnPropertyChanged(nameof(User));
    }

    public void ChangeCulture(string culture)
    {
        culture.SetCulture();

        if (App.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.ReloadMainWindow(_serviceProvider!);
    }
}