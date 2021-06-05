using Bit.Model.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NIOC.SampleStorage.Server.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void RegisterDbSets(this ModelBuilder modelBuilder, Assembly entitiesAssembly)
        {
            IEnumerable<Type> entityTypes = entitiesAssembly
                .GetExportedTypes()
                .Where(type => type.IsClass &&
                               !type.IsAbstract &&
                               typeof(IEntity).IsAssignableFrom(type))
                .ToList();

            foreach (var entity in entityTypes)
            {
                modelBuilder.Entity(entity);
            }
        }

        /// <summary>
        /// Register database Views in a way that EF do not mistake it will ordinary tables
        /// and map it to real View instead of creating a table
        /// </summary>
        /// <typeparam name="TView">POCO class mapped to View that implement IEntity</typeparam>
        /// <param name="modelBuilder"></param>
        /// <param name="viewName">Name of View in SQL Server database</param>
        public static void ConfigureDbView<TView>(this ModelBuilder modelBuilder, string viewName) where TView : class
        {
            modelBuilder.Entity<TView>().ToView(viewName).HasNoKey();
        }

        #region IsArchived Global Query Filter
        public static void RegisterIsArchivedGlobalQueryFilter(this ModelBuilder modelBuilder)
        {
            foreach (var type in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IArchivableEntity).IsAssignableFrom(type.ClrType)
                    && (type.BaseType == null
                        || !typeof(IArchivableEntity).IsAssignableFrom(type.BaseType.ClrType)))
                {
                    modelBuilder.SetIsArchivedFilter(type.ClrType);
                }
            }
        }

        public static void SetIsArchivedFilter(this ModelBuilder modelBuilder, Type entityType)
        {
            SetEfGlobalFilterMethod.MakeGenericMethod(entityType)
                .Invoke(null, new object[] { modelBuilder });
        }

        private static readonly MethodInfo SetEfGlobalFilterMethod = typeof(ModelBuilderExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == nameof(SetIsArchivedFilter));

        public static void SetIsArchivedFilter<TEntity>(this ModelBuilder modelBuilder)
            where TEntity : class, IArchivableEntity
        {
            modelBuilder.Entity<TEntity>().HasQueryFilter(x => x.IsArchived == false);
        }
        #endregion

        public static void ConfigureDecimalPrecision(this ModelBuilder modelBuilder)
        {
            var decimalProperties = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(type => type.GetProperties())
                .Where(prop => prop.ClrType == typeof(decimal) || prop.ClrType == typeof(decimal?));

            foreach (IMutableProperty prop in decimalProperties)
            {
                prop.SetColumnType("decimal(20, 10)");
            }
        }

        public static void SetRestrictAsDefaultDeleteBehavior(this ModelBuilder modelBuilder)
        {
            IEnumerable<IMutableForeignKey> cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (IMutableForeignKey fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        public static void ApplyConfigurations(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            MethodInfo applyGenericMethod = typeof(ModelBuilder).GetMethods()
                .First(m => m.Name == nameof(ModelBuilder.ApplyConfiguration));

            IEnumerable<Type> types = assemblies.SelectMany(a => a.GetExportedTypes())
                .Where(c => c.IsClass && !c.IsAbstract && c.IsPublic);

            foreach (Type type in types)
            {
                foreach (Type interfacee in type.GetInterfaces())
                {
                    if (interfacee.IsConstructedGenericType && interfacee.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                    {
                        MethodInfo applyConcreteMethod = applyGenericMethod.MakeGenericMethod(interfacee.GenericTypeArguments[0]);
                        applyConcreteMethod.Invoke(modelBuilder, new[] { Activator.CreateInstance(type) });
                    }
                }
            }
        }

        public static ModelBuilder UseJsonDbFunctions(this ModelBuilder builder)
        {
            var jsonValueMethodInfo = typeof(JsonDbFunctions)
                .GetRuntimeMethod(
                    nameof(JsonDbFunctions.Value),
                    new[] { typeof(string), typeof(string) }
                );
            builder
                .HasDbFunction(jsonValueMethodInfo)
                .HasTranslation(args =>
                    new SqlFunctionExpression("JSON_VALUE", true, typeof(string), null));

            return builder;
        }
    }
}