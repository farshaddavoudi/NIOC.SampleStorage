using Bit.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Server.Api.Installers.Contract;
using NIOC.SampleStorage.Server.Model.AppSettingsOptions;

namespace NIOC.SampleStorage.Server.Api.Installers.Implementations
{
    public class AppSettingsJsonInstaller : IServerServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IDependencyManager dependencyManager,
            ServerAppSettings serverAppSettings)
        {
            // Register (Server)AppSettings as Singleton to easy use
            dependencyManager.RegisterInstance(serverAppSettings);
        }
    }
}