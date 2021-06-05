using Bit.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Server.Model.AppSettingsOptions;

namespace NIOC.SampleStorage.Server.Api.Installers.Contract
{
    public interface IServerServiceInstaller
    {
        void InstallServices(IServiceCollection services,
            IDependencyManager dependencyManager,
            ServerAppSettings serverAppSettings);
    }
}