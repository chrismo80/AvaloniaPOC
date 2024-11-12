using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Data.Core.Plugins;
using Avalonia.Controls.ApplicationLifetimes;

using CompanyName.UI;

namespace ProjectExampleHMI;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        switch (ApplicationLifetime)
        {
            // Mobile or embedded platforms -> no window, one view
            case ISingleViewApplicationLifetime singleView:
                //singleView.MainView = new MainView();
                break;

            // Desktop platforms (Windows, Linux, macOS) -> one or more windows
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.StartWithSplashScreen<Windows.MainWindow, ViewModels.MainWindowViewModel>(ServiceBuilder.Build);

                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                BindingPlugins.DataValidators.RemoveAt(0);
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}