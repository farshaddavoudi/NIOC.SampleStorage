using Bit.Http.Contracts;
using Bit.Http.Implementations;
using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Client.Service.AppSettingsOptions;
using NIOC.SampleStorage.Client.Web.Installers.Contract;

namespace NIOC.SampleStorage.Client.Web.Installers.Implementations
{
    public class BitInstaller : IClientServiceInstaller
    {
        public void InstallServices(IServiceCollection services, ClientAppSettings clientAppSettings)
        {
            services.AddScoped<ISecurityService, DefaultSecurityService>();
            services.AddTransient<ITokenProvider, DefaultTokenProvider>();
        }
    }
}