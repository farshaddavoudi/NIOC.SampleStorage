using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NIOC.SampleStorage.Client.Service.AppSettingsOptions;
using NIOC.SampleStorage.Client.Web.Installers.Contract;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace NIOC.SampleStorage.Client.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var clientAppSettings = builder.Configuration.GetSection(nameof(ClientAppSettings)).Get<ClientAppSettings>();
            clientAppSettings.UrlOptions!.AppBaseAddress = builder.HostEnvironment.BaseAddress;

            // Register HttpClient (Host client)
            builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            // Configure Dependencies with Service Installers
            var installers = new[] { Assembly.GetExecutingAssembly() }.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && typeof(IClientServiceInstaller).IsAssignableFrom(c))
                .Select(Activator.CreateInstance).Cast<IClientServiceInstaller>().ToList();
            installers.ForEach(i => i.InstallServices(builder.Services, clientAppSettings));

            await builder
                .Build()
                .UseLoadingBar()
                .RunAsync();
        }
    }
}
