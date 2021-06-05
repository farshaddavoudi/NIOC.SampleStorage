using Bit.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Server.Api.Identity;
using NIOC.SampleStorage.Server.Api.Installers.Contract;
using NIOC.SampleStorage.Server.Model.AppSettingsOptions;
using NIOC.SampleStorage.Server.Model.Contracts;

namespace NIOC.SampleStorage.Server.Api.Installers.Implementations
{
    public class UserInstaller : IServerServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IDependencyManager dependencyManager,
            ServerAppSettings serverAppSettings)
        {
            dependencyManager.Register<IUserInfoProvider, UserInfoProvider>();
        }
    }
}