using Avalonia.Interactivity;

using Microsoft.Extensions.DependencyInjection;

using System;

using CompanyName.Core.Auth;

using CompanyName.UI;
using CompanyName.UI.Windows;

namespace ProjectExampleHMI.Windows;

public partial class MainWindow : BaseWindow
{
    readonly IServiceProvider? _provider;

    readonly IAuthManager? _authManager;

    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(IServiceProvider provider, IAuthManager authManager)
       : this()
    {
        _authManager = authManager;
        _provider = provider;
    }

    public async void Exit(object sender, RoutedEventArgs args)
    {
        var result = await MessageBox.Show(this, CompanyName.UI.Assets.Resources.CloseQuestion, "Exit", MessageBox.MessageBoxButtons.YesNo);

        if (result == MessageBox.MessageBoxResult.Yes)
            Close();
    }

    public void Login(object sender, RoutedEventArgs args)
    {
        if (_authManager?.User == "")
            _provider?.GetRequiredService<UserLoginDialog>().Show(this, "Login");
        else
            _authManager?.Logout();
    }
}