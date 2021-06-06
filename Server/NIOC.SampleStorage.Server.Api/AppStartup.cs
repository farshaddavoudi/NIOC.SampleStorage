using Bit.Core;
using Bit.Core.Contracts;
using Bit.Model.Implementations;
using Bit.OData.Contracts;
using Bit.Owin;
using Bit.Owin.Contracts;
using Bit.Owin.Implementations;
using Bit.WebApi.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using NIOC.SampleStorage.Server.Api.ActionFilters;
using NIOC.SampleStorage.Server.Api.Helpers;
using NIOC.SampleStorage.Server.Api.Identity;
using NIOC.SampleStorage.Server.Api.Implementations;
using NIOC.SampleStorage.Server.Api.Installers;
using NIOC.SampleStorage.Server.Api.Installers.Contract;
using NIOC.SampleStorage.Server.Model.AppSettingsOptions;
using NIOC.SampleStorage.Shared.App;
using NIOC.SampleStorage.Shared.Core.Extensions;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

[assembly: ODataModule("SampleStorage")]

namespace NIOC.SampleStorage.Server.Api
{
    public class AppStartup : AutofacAspNetCoreAppStartup, IAppModule, IAppModulesProvider
    {
        public AppStartup(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            AspNetCoreAppEnvironmentsProvider.Current.Init();
        }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            DefaultAppModulesProvider.Current = this;

            return base.ConfigureServices(services);
        }

        public IEnumerable<IAppModule> GetAppModules()
        {
            yield return this;
        }

        public virtual void ConfigureDependencies(IServiceCollection services, IDependencyManager dependencyManager)
        {
            AssemblyContainer.Current.Init();

            #region Configure services

            dependencyManager.RegisterMinimalDependencies();

            dependencyManager.RegisterDefaultLogger(typeof(SeqLogStore).GetTypeInfo()
#if DEBUG
                , typeof(DebugLogStore).GetTypeInfo()
                , typeof(ConsoleLogStore).GetTypeInfo()
#endif
            );

            dependencyManager.RegisterDefaultAspNetCoreApp();

            dependencyManager.RegisterDefaultWebApiAndODataConfiguration();

            dependencyManager.Register<IExceptionToHttpErrorMapper, NIOCExceptionToHttpErrorMapper>();

            dependencyManager
                .RegisterWebApiConfigurationCustomizer<GlobalDefaultExceptionHandlerActionFilterProvider<
                    NIOCExceptionHandlerFilterAttribute>>();

            dependencyManager.RegisterModelStateValidator();

            dependencyManager.RegisterDtoEntityMapper();
            dependencyManager.RegisterMapperConfiguration<DefaultMapperConfiguration>();
            dependencyManager.RegisterAutoMapperConfigurations(new[] { typeof(Program).Assembly });

            services.AddRazorPages();
            services.AddControllers(options =>
            {
                options.Filters.Add(new NIOCAuthorizeAttribute());
                options.Conventions.Add(new RouteTokenTransformerConvention(new CamelCaseDasherizeParameterTransformer()));
            });

            services.AddResponseCompression(opts =>
            {
                opts.Providers.Add<BrotliCompressionProvider>();
                opts.Providers.Add<GzipCompressionProvider>();
            })
                .Configure<BrotliCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest)
                .Configure<GzipCompressionProviderOptions>(opt => opt.Level = CompressionLevel.Fastest);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = AppMetadata.AppPersianFullName, Version = "v1" });
            });


            // Configure Dependencies with Service Installers 
            var serverAppSettings = AspNetCoreAppEnvironmentsProvider.Current.Configuration.GetSection(nameof(ServerAppSettings)).Get<ServerAppSettings>();
            var assemblies = new[] { Assembly.GetExecutingAssembly() };
            var installers = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic && typeof(IServerServiceInstaller).IsAssignableFrom(c))
                .Select(Activator.CreateInstance).Cast<IServerServiceInstaller>().ToList();
            installers.ForEach(i => i.InstallServices(services, dependencyManager, serverAppSettings));

            #endregion

            #region Configure middlewares

            dependencyManager.RegisterAspNetCoreMiddlewareUsing(aspNetCoreApp =>
            {
#if BlazorClient
                if (AspNetCoreAppEnvironmentsProvider.Current.WebHostEnvironment.IsDevelopment())
                    aspNetCoreApp.UseWebAssemblyDebugging();
                aspNetCoreApp.UseBlazorFrameworkFiles();
#endif
                aspNetCoreApp.UseResponseCompression();
                aspNetCoreApp.UseStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                        {
                            MaxAge = TimeSpan.FromDays(365),
                            Public = true
                        };
                    }
                });

                aspNetCoreApp.UseRouting();

                aspNetCoreApp.UseSwagger();
                aspNetCoreApp.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{AppMetadata.AppEnglishFullName} v1"));
            });

            dependencyManager.RegisterAspNetCoreSingleSignOnClient();

            dependencyManager.RegisterMinimalAspNetCoreMiddlewares();

            dependencyManager.RegisterODataMiddleware(odataDependencyManager =>
            {
                odataDependencyManager.RegisterGlobalWebApiCustomizerUsing(httpConfiguration =>
                {
                    httpConfiguration.Filters.Add(new NIOCAuthorizeAttribute());
                    httpConfiguration.EnableSwagger(c =>
                    {
                        var xmlDocs = new[] { Assembly.GetExecutingAssembly() }
                            .Select(a => Path.Combine(Path.GetDirectoryName(a.Location)!, $"{a.GetName().Name}.xml"))
                            .Where(File.Exists).ToArray();
                        c.SingleApiVersion("v1", $"Swagger-Api");
                        Array.ForEach(xmlDocs, c.IncludeXmlComments);
                        c.ApplyDefaultODataConfig(httpConfiguration);
                    }).EnableBitSwaggerUi();
                });

                odataDependencyManager.RegisterWebApiODataMiddlewareUsingDefaultConfiguration();
            });

            dependencyManager.RegisterSingleSignOnServer<IdentityUserService, IdentityClientsProvider>();

            dependencyManager.RegisterAspNetCoreMiddlewareUsing(aspNetCoreApp =>
            {
                aspNetCoreApp.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers().RequireAuthorization();
#if BlazorClient
                    endpoints.MapFallbackToPage("/_Host");
#endif
                });
            }, MiddlewarePosition.AfterOwinMiddlewares);

            #endregion
        }


    }
}
