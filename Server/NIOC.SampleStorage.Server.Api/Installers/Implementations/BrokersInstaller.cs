using Bit.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Server.Api.Installers.Contract;
using NIOC.SampleStorage.Server.Model.AppSettingsOptions;
using NIOC.SampleStorage.Server.Service.NIOCSSO;
using System;

namespace NIOC.SampleStorage.Server.Api.Installers.Implementations
{
    public class BrokersInstaller : IServerServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IDependencyManager dependencyManager,
            ServerAppSettings serverAppSettings)
        {
            services.AddHttpClient<SSOClient>(client =>
            {
                client.BaseAddress = new Uri(serverAppSettings.UrlOptions!.SSOAddress!);
            });

        }
    }
}