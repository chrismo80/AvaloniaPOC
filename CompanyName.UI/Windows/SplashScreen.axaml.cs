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
			LoadService(serviceProvider, services[i - 1], i, services.Count);

		Dispatcher.UIThread.Invoke(() => LoadingText.Text = Assets.Resources.Loading + "...");

		return serviceProvider;
	}

	private void LoadService(ServiceProvider provider, ServiceDescriptor descriptor, int index, int max)
	{
		Dispatcher.UIThread.Invoke(() =>
		{
			LoadingText.Text = Assets.Resources.Loading + "... " + descriptor.ServiceType.Name;
			Count.Text = index + " / " + max;
		});

		// let service provider call service ctor to check if service is ready, otherwise do not start app
		_ = provider.GetRequiredService(descriptor.ServiceType);

		Dispatcher.UIThread.Invoke(() => ProgressBar.Value = index * 100 / max);
	}
}