using Bit.Core.Contracts;
using Bit.Data;
using Bit.Data.Contracts;
using Bit.Owin.Implementations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NIOC.SampleStorage.Server.Api.Installers.Contract;
using NIOC.SampleStorage.Server.Data;
using NIOC.SampleStorage.Server.Data.Repository;
using NIOC.SampleStorage.Server.Model.AppSettingsOptions;
using NIOC.SampleStorage.Server.Model.Contracts;
using System;
using System.Data.Common;
using System.Reflection;

namespace NIOC.SampleStorage.Server.Api.Installers.Implementations
{
    public class DataAccessInstaller : IServerServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IDependencyManager dependencyManager,
            ServerAppSettings serverAppSettings)
        {

            dependencyManager.RegisterRepository(typeof(NIOCRepository<>).GetTypeInfo());
            dependencyManager.RegisterRepository(typeof(NIOCRepositoryReadOnly<>).GetTypeInfo());

            dependencyManager.RegisterGeneric(typeof(INIOCRepository<>).GetTypeInfo(), typeof(NIOCRepository<>).GetTypeInfo());
            dependencyManager.RegisterGeneric(typeof(IReadOnlyNIOCRepository<>).GetTypeInfo(), typeof(NIOCRepositoryReadOnly<>).GetTypeInfo());

            dependencyManager.Register<IDbConnectionProvider, DefaultDbConnectionProvider<SqlConnection>>();

            dependencyManager.RegisterEfCoreDbContext<NIOCDbContext>((serviceProvider, optionsBuilder) =>
            {
                var connectionString = serverAppSettings.ConnectionStringOptions!.AppDbConnectionString!;

                DbConnection connection = serviceProvider.GetRequiredService<IDbConnectionProvider>().GetDbConnection(connectionString, rollbackOnScopeStatusFailure: true);

                optionsBuilder.UseSqlServer(connection, sqlServerOptions =>
                {
                    sqlServerOptions.CommandTimeout((int)TimeSpan.FromMinutes(1)
                        .TotalSeconds); //Default is 30 seconds
                });

                // Interceptors

                // Show Detailed Errors
                if (AspNetCoreAppEnvironmentsProvider.Current.WebHostEnvironment.IsDevelopment())
                    optionsBuilder.EnableSensitiveDataLogging().EnableDetailedErrors();
            });
        }

    }
}