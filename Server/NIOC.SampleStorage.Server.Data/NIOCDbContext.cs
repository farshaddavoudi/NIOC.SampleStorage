using Bit.Data.EntityFrameworkCore.Implementations;
using Microsoft.EntityFrameworkCore;
using NIOC.SampleStorage.Server.Data.Extensions;
using NIOC.SampleStorage.Server.Model.Entities;

namespace NIOC.SampleStorage.Server.Data
{
    public class NIOCDbContext : EfCoreDbContextBase
    {
        public NIOCDbContext(DbContextOptions<NIOCDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Auto Register all Entities
            modelBuilder.RegisterDbSets(typeof(NIOCEntity).Assembly);

            modelBuilder.UseJsonDbFunctions();

            // Configure Views
            //modelBuilder.ConfigureDbView<AppUserViewEntity>(DbViewNames.vAppUsers);

            base.OnModelCreating(modelBuilder);

            modelBuilder.ConfigureDecimalPrecision();

            // Restrict Delete (in Hard delete scenarios)
            // Ef default is Cascade
            modelBuilder.SetRestrictAsDefaultDeleteBehavior();

            // Auto Register all Entity Configurations (Fluent-API)
            modelBuilder.ApplyConfigurations(typeof(NIOCDbContext).Assembly);
        }
    }
}