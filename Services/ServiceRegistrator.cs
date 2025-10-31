using Lab1.Core.Device;
using Lab1.Core.Device.Interfaces;
using Lab1.Core.ScriptInterpreter.Interfaces;
using Lab1.Core.ScriptInterpreter.Services;
using Lab1.Core.ScriptInterpreter.Utils;
using Lab1.Services.Interface;
using Lab1.ViewModels;
using Microsoft.Extensions.DependencyInjection;


namespace Lab1.Services
{
    static class ServiceRegistrator
    {
        public static IServiceCollection AddServices(this IServiceCollection services) => services
            .AddSingleton<IConnection, CommunicationService>()
            .AddSingleton<IFindRegistryKey, FindRegKeyService>()
            .AddSingleton<IUSBDeviceFinder, FindUSBDevicesService>()
            .AddSingleton<IDllWorkerService, DllWorkerService>()
            ;

    }
}
