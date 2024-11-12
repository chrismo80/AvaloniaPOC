using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Threading;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using System.Reflection;
using System.Globalization;

using CompanyName.Core;
using CompanyName.Core.Logging;
using CompanyName.Core.Messages;

using CompanyName.UI.Windows;

namespace CompanyName.UI;

public static class UiExtensions
{
    static IConfiguration? _configuration;

    /// <summary>
    /// Initialises a main window of the type <typeparamref name="TWindow" /><br/>
    /// Attaches a new instance of <typeparamref name="TViewModel" /> to the data context of that window<br/>
    /// Starts a splash screen and initialises the service provider with the service collection setup<br/>
    /// Opens the main window after service initialisation was successful
    /// </summary>
    /// <typeparam name="TWindow">Type of the main window</typeparam>
    /// <typeparam name="TViewModel">Type of the view model of the main window</typeparam>
    /// <param name="desktop">Dekstop object the main window to attach to</param>
    /// <param name="setupCollection">Delegate for the service collection setup during splash screen init</param>
    public static async void StartWithSplashScreen<TWindow, TViewModel>(
        this IClassicDesktopStyleApplicationLifetime desktop, Func<IServiceCollection> setupCollection)
        where TWindow : Window
        where TViewModel : notnull
    {
        // Show splash screen
        var splashScreen = new SplashScreen();
        desktop.MainWindow = splashScreen;
        splashScreen.Show();

        // Init services with splash screen
        var serviceProvider = await splashScreen.Process(setupCollection);

        // Dispose services when application is closed
        desktop.ShutdownRequested += (_, _) => serviceProvider.Dispose();

        // Set data directory if configured
        serviceProvider.GetService<IConfiguration>()?["Data:Directory"]?.SetDataDirectory();

        // Set services to extensions if configured
        serviceProvider.GetService<ILogger>()?.ConfigureTraceExtensions();
        serviceProvider.GetService<IMessageManager>()?.ConfigureMessageExtensions();
        serviceProvider.GetService<IConfiguration>()?.ConfigureSettingExtensions();

        // Change ui language if set in app settings
        serviceProvider.GetService<IConfiguration>()?["Language"]?.SetCulture();

        // Show main window
        var mainWindow = serviceProvider.GetRequiredService<TWindow>();
        mainWindow.DataContext = serviceProvider.GetRequiredService<TViewModel>();
        desktop.MainWindow = mainWindow;
        mainWindow.Show();

        splashScreen.Close();
    }

    /// <summary>
    /// Reloads the whole main window
    /// </summary>
    /// <param name="desktop">the desktop that holds the current main window</param>
    /// <param name="provider">the service provider to get the main window from</param>
    public static async void ReloadMainWindow(this IClassicDesktopStyleApplicationLifetime desktop, IServiceProvider provider)
    {
        if (desktop.MainWindow is not Window currentWindow)
            return;

        // get new instance of same window (transient service)
        var newWindow = (Window)provider!.GetRequiredService(currentWindow.GetType());
        newWindow.DataContext = currentWindow.DataContext;
        desktop.MainWindow = newWindow;

        newWindow.Show();

        await Task.Delay(100);
        currentWindow.Close();
    }

    /// <summary>
    /// Sets ui culture on current thread and ui thread
    /// </summary>
    /// <param name="culture">culture to set</param>
    public static void SetCulture(this string culture)
    {
        // Check if different lanugage than current
        if (Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName == culture)
            return;

        var cultureInfo = new CultureInfo(culture);

        // For current thread (main window at startup)
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;

        // For ui thread (views at startup)
        Dispatcher.UIThread.InvokeAsync(() => Thread.CurrentThread.CurrentUICulture = cultureInfo);
    }

    internal static void ConfigureSettingExtensions(this IConfiguration config) => _configuration = config;

    /// <summary>
    /// Fall back access to IConfiguration, if ctor DI not possible
    /// </summary>
    /// <param name="me">object for extension</param>
    /// <param name="setting">the setting full name to get</param>
    /// <returns>the value from IConfiguration</returns>
    internal static T? GetSetting<T>(this object me, string setting) =>
        _configuration?[setting] is string value ? (T)Convert.ChangeType(value, typeof(T)) : default;

    /// <summary>
    /// opens a virtual keyboard<br/>
    /// if text box is embedded into a numeric updown control, then a virtual numpad is opened
    /// </summary>
    /// <param name="textBox">the source textbox control where the input should be written to</param>
    internal static async void ShowVirtualInputControl(this TextBox textBox)
    {
        if (textBox.FindAncestorOfType<NumericUpDown>() is NumericUpDown numericUpDown)
        {
            // text box is embedded into a numeric updown -> numbers
            var input = await NumPad.Show(numericUpDown.GetVisualRoot() as Window, "Type!", numericUpDown.Value.ToString()!);

            if (decimal.TryParse(input, out decimal value))
                numericUpDown.Value = value;
        }
        else
        {
            // just a simple text box -> strings
            textBox.Text = await Keyboard.Show(textBox.GetVisualRoot() as Window, textBox.Watermark ?? "", textBox.Text!, textBox.PasswordChar);
        }
    }

    /// <summary>
    /// Adds the default services to the service collection, such as view models, windows and service provider itself
    /// </summary>
    /// <param name="collection">the collection ot modify</param>
    /// <returns>the modified collection</returns>
    internal static IServiceCollection AddDefaultServices(this IServiceCollection collection) => collection

        // add all viewmodels and windows of CompanyName.UI (for standard view models / windows)
        .AddTransients<BaseViewModel>(Assembly.GetExecutingAssembly())
        .AddTransients<BaseWindow>(Assembly.GetExecutingAssembly())

        // add all viewmodels and windows of entry assembly (custom project)
        .AddTransients<BaseViewModel>(Assembly.GetEntryAssembly()!)
        .AddTransients<BaseWindow>(Assembly.GetEntryAssembly()!)

        // add service provider itself to enable the extension IServiceProvider.New<T>(params objcet[] parameters)
        .AddSingleton(serviceProvider => serviceProvider);
}