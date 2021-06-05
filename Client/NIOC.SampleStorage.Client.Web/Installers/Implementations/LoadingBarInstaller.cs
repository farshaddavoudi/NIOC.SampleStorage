using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Client.Service.AppSettingsOptions;
using NIOC.SampleStorage.Client.Web.Installers.Contract;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace NIOC.SampleStorage.Client.Web.Installers.Implementations
{
    public class LoadingBarInstaller : IClientServiceInstaller
    {
        public void InstallServices(IServiceCollection services, ClientAppSettings clientAppSettings)
        {
            services.AddLoadingBar();
        }
    }
}