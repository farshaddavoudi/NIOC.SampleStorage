using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using NIOC.SampleStorage.Client.Service.AppSettingsOptions;
using NIOC.SampleStorage.Client.Web.Installers.Contract;
using System.Reflection;

namespace NIOC.SampleStorage.Client.Web.Installers.Implementations
{
    public class AutoRegisterDiInstaller : IClientServiceInstaller
    {
        public void InstallServices(IServiceCollection services, ClientAppSettings clientAppSettings)
        {
            var webAssembly = Assembly.GetExecutingAssembly();
            var clientWebServiceAssembly = typeof(ClientAppSettings).Assembly;

            var assembliesToScan = new[] { webAssembly, clientWebServiceAssembly };

            #region Generic Type Dependencies
            //services.AddScoped(typeof(IRepository<>), typeof(EfCoreRepositoryBase<,>));
            #endregion


            #region Register DIs By Name
            services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
                .Where(c => c.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);
            #endregion 

        }
    }
}