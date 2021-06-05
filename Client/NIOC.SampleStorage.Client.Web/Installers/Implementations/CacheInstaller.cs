using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Client.Service;
using NIOC.SampleStorage.Client.Service.AppSettingsOptions;
using NIOC.SampleStorage.Client.Web.Installers.Contract;

namespace NIOC.SampleStorage.Client.Web.Installers.Implementations
{
    public class CacheInstaller : IClientServiceInstaller
    {
        public void InstallServices(IServiceCollection services, ClientAppSettings clientAppSettings)
        {
            // Singleton cash using by directly injecting AppData into a class
            var appCache = new AppData();

            services.AddSingleton(sp => appCache);
        }
    }
}