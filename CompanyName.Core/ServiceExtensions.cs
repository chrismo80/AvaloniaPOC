using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CompanyName.Core;

public static class ServiceExtensions
{
	/// <summary>
	/// abbreviation for object creation via service provider (if type requires dependencies)
	/// </summary>
	public static T New<T>(this IServiceProvider provider, params object[] parameters) =>
		ActivatorUtilities.CreateInstance<T>(provider, parameters);

	/// <summary>
	/// adds an instance of <typeparamref name="T" /> as singleton to the service collection<br/>
	/// adds each of its interfaces (whose namespace starts with 'CompanyName') as service to that instance as well<br/>
	/// this enables access to <typeparamref name="T" /> only for instantiating during application init for example
	/// </summary>
	public static IServiceCollection AddInterfacesOf<T>(this IServiceCollection collection, string defaultNamespace = "CompanyName")
		where T : class
	{
		collection.AddSingleton(provider => ActivatorUtilities.CreateInstance<T>(provider));

		foreach (var service in typeof(T).GetInterfaces().Where(i => i.Namespace!.StartsWith(defaultNamespace)))
		{
			if (collection.Any(s => s.ServiceType == service))
				throw new ArgumentException($"{service} already registered!");

			collection.AddSingleton(service, provider => provider.GetRequiredService<T>());
		}

		return collection;
	}

	/// <summary>
	/// creates singleton objects for each class that implements <typeparamref name="T" /> (either as base class or interface)<br/>
	/// searches the executable assembly by default<br/>
	/// optional: searches in other assemblies
	/// </summary>
	public static IServiceCollection AddSingletonsThatImplement<T>(this IServiceCollection collection, params string[] assemblyNames)
	{
		collection.AddSingletons<T>(Assembly.GetEntryAssembly()!);

		foreach (var assemblyName in assemblyNames)
			collection.AddSingletons<T>(Assembly.Load(assemblyName));

		return collection;
	}

	/// <summary>
	/// creates transient objects for each class that implements <typeparamref name="T" /> (either as base class or interface)<br/>
	/// searches the executable assembly by default<br/>
	/// optional: searches in other assemblies
	/// </summary>
	public static IServiceCollection AddTransientsThatImplement<T>(this IServiceCollection collection, params string[] assemblyNames)
	{
		collection.AddTransients<T>(Assembly.GetEntryAssembly()!);

		foreach (var assemblyName in assemblyNames)
			collection.AddTransients<T>(Assembly.Load(assemblyName));

		return collection;
	}

	/// <summary>
	/// reads json configuration files and adds them all as IConfiguration singelton to the service collection
	/// </summary>
	public static IServiceCollection AddConfiguration(this IServiceCollection collection, params string[] files)
	{
		var builder = new ConfigurationBuilder();

		foreach (var file in files)
			builder.AddJsonFile(file);

		collection.AddSingleton<IConfiguration>(builder.Build());

		return collection;
	}

	/// <summary>
	/// AppDomain.CurrentDomain.SetData("DataDirectory", directory)
	/// </summary>
	public static void SetDataDirectory(this string directory) =>
		AppDomain.CurrentDomain.SetData("DataDirectory", directory);

	/// <summary>
	/// returns all registered services that implement a certain base class or interace
	/// </summary>
	public static IEnumerable<T> GetServicesThatImplement<T>(this IServiceProvider serviceProvider)
	{
		var services = Assembly.GetEntryAssembly()!.GetTypes().Where(t => t.Implements<T>())
			.Select(serviceProvider.GetService).Where(service => service != null);

		foreach (var service in services)
		{
			if (service is T instance)
				yield return instance;
		}
	}

	public static IServiceCollection AddSingletons<T>(this IServiceCollection collection, Assembly assembly)
	{
		foreach (var type in assembly.GetTypes().Where(t => t.Implements<T>()))
			collection.AddSingleton(type);

		return collection;
	}

	public static IServiceCollection AddTransients<T>(this IServiceCollection collection, Assembly assembly)
	{
		foreach (var type in assembly.GetTypes().Where(t => t.Implements<T>()))
			collection.AddTransient(type);

		return collection;
	}

	private static bool Implements<T>(this Type t) =>
		t is { IsClass: true, IsAbstract: false } && t.IsAssignableTo(typeof(T));
}