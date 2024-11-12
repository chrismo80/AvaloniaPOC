using Avalonia.Controls;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyName.UI.Windows;

public partial class SplashScreen : Window
{
	public SplashScreen() => InitializeComponent();

	public async Task<ServiceProvider> Process(Func<IServiceCollection> buildServices)
	{
		try
		{
			Dispatcher.UIThread.Invoke(() =>
			{
				ApplicationName.Text = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Name ?? "HMI";
				LoadingText.Text = Assets.Resources.Init;
			});

			return await Task.Run(() => Init(buildServices()));
		}
		catch (Exception ex)
		{
			Dispatcher.UIThread.Invoke(() =>
			{
				LoadingText.Text += $" {Assets.Resources.Failed.ToLower()}!\n{Assets.Resources.ApplicationClosed}.";
				ErrorText.Text = ex.Message;
			});

			await Task.Delay(TimeSpan.FromSeconds(10));

			throw;
		}
	}

	private ServiceProvider Init(IServiceCollection serviceCollection)
	{
		var serviceProvider = serviceCollection.AddDefaultServices().BuildServiceProvider();

		var services = serviceCollection
			.Where(item => item is { Lifetime: ServiceLifetime.Singleton, ServiceType.IsInterface: false })
			.ToList();

		for (int i = 1; i <= services.Count; i++)
		{
			int index = i;

			Dispatcher.UIThread.Invoke(() =>
			{
				LoadingText.Text = Assets.Resources.Loading + "... " + services[index - 1].ServiceType.Name;
				Count.Text = index + " / " + services.Count;
			});

			// let service provider call service ctors to check if all are ready
			_ = serviceProvider.GetRequiredService(services[i - 1].ServiceType);

			Dispatcher.UIThread.Invoke(() => ProgressBar.Value = i * 100 / services.Count);
		}

		Dispatcher.UIThread.Invoke(() => LoadingText.Text = Assets.Resources.Loading + "...");

		return serviceProvider;
	}
}