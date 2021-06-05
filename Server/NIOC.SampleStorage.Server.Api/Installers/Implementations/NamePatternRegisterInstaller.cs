using Autofac;
using Bit.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Server.Api.Installers.Contract;
using NIOC.SampleStorage.Server.Model.AppSettingsOptions;
using NIOC.SampleStorage.Server.Service;

namespace NIOC.SampleStorage.Server.Api.Installers.Implementations
{
    /// <summary>
    /// Register Assembly Public NonAbstract Classes with PropertyInjection enabled
    /// </summary>
    public class NamePatternRegisterInstaller : IServerServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IDependencyManager dependencyManager,
            ServerAppSettings serverAppSettings)
        {
            dependencyManager
                .GetContainerBuilder()
                .RegisterAssemblyTypes(typeof(EntityService<>).Assembly, typeof(AppStartup).Assembly)
                .PublicOnly()
                .Where(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    type.Name.EndsWith("Service") &&
                    type.Name != "EntityService")
                .AsSelf()
                .As(t => t.BaseType!)
                .AsImplementedInterfaces()
                .PropertiesAutowired(PropertyWiringOptions.PreserveSetValues)
                .PreserveExistingDefaults();
        }
    }
}