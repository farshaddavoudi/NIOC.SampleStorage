using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Client.Service.AppSettingsOptions;
using NIOC.SampleStorage.Client.Web.Implementations;
using NIOC.SampleStorage.Client.Web.Installers.Contract;

namespace NIOC.SampleStorage.Client.Web.Installers.Implementations
{
    public class AuthInstaller : IClientServiceInstaller
    {
        public void InstallServices(IServiceCollection services, ClientAppSettings clientAppSettings)
        {
            services.AddOptions();

            services.AddAuthorizationCore();

            services.AddScoped<AuthenticationStateProvider, AppAuthenticationStateProvider>();

            services.AddTransient(serviceProvider => (AppAuthenticationStateProvider)serviceProvider.GetRequiredService<AuthenticationStateProvider>());
        }
    }
}