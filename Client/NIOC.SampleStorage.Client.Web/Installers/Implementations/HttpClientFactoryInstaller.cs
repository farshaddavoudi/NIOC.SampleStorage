using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Client.Service.AppSettingsOptions;
using NIOC.SampleStorage.Client.Web.Installers.Contract;

namespace NIOC.SampleStorage.Client.Web.Installers.Implementations
{
    public class HttpClientFactoryInstaller : IClientServiceInstaller
    {
        public void InstallServices(IServiceCollection services, ClientAppSettings clientAppSettings)
        {
            //services.AddHttpClient<SsoClient>(client =>
            //{
            //    client.BaseAddress = new Uri(clientAppSettings.UrlOptions!.SecurityAppUrl!);
            //});
        }
    }
}