using System.Runtime.InteropServices;

using Microsoft.Extensions.DependencyInjection;

using CompanyName.Core;

namespace ProjectExampleHMI;

internal static class Services
{
    static readonly string _platform = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "mac" : "win";
    
    internal static IServiceCollection Setup() => new ServiceCollection()

        // Dependency injections (singletons), all their 'CompanyName' interfaces are automatically resolvable
        .AddInterfacesOf<CompanyName.Core.Logging.FileLogger>()
        .AddInterfacesOf<CompanyName.Core.Messages.MessageManager>()
        .AddInterfacesOf<CompanyName.Core.Auth.AuthManager>()
        .AddInterfacesOf<CompanyName.Core.Data.ParameterManager>()

        // Devices -> see Devices\Configuration.cs
        .AddSingleton<Devices.Cam110>()
        .AddSingleton<Devices.Cam111>()
        .AddSingleton<Devices.LedController1>()
        .AddSingleton<Devices.LedController2>()
        .AddSingleton<Devices.SyncDevice1>()
        .AddSingleton<Devices.SyncDevice2>()
        .AddSingleton<Devices.SyncDevice3>()
        .AddSingleton<Devices.DeviceX1>()
        .AddSingleton<Devices.DeviceY1>()
        .AddSingleton<Devices.DeviceX2>()

        // Models -> see Models\Configuration.cs
        .AddSingleton<Models.Inspection1>()
        .AddSingleton<Models.Inspection2>()
        .AddSingleton<Models.Inspection3>()

        // Application config(s), resolvable as 'IConfiguration'
        .AddConfiguration($"appsettings.{_platform}.json");
}