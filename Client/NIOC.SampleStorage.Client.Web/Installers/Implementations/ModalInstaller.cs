using Blazored.Modal;
using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Client.Service.AppSettingsOptions;
using NIOC.SampleStorage.Client.Web.Installers.Contract;

namespace NIOC.SampleStorage.Client.Web.Installers.Implementations
{
    public class ModalInstaller : IClientServiceInstaller
    {
        public void InstallServices(IServiceCollection services, ClientAppSettings clientAppSettings)
        {
            // https://github.com/Blazored/Modal
            services.AddBlazoredModal();
        }
    }
}