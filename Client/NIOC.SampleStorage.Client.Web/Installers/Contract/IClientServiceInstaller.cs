using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Client.Service.AppSettingsOptions;

namespace NIOC.SampleStorage.Client.Web.Installers.Contract
{
    public interface IClientServiceInstaller
    {
        void InstallServices(IServiceCollection services, ClientAppSettings clientAppSettings);
    }
}