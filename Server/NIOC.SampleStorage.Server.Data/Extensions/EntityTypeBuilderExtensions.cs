using Bit.Model.Contracts;
using System;
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
    public static class EntityTypeBuilderExtensions
    {
        // There is no IsArchived property in NIOC tables
        public static IndexBuilder HasUniqueIndex<TEntity>(
            this EntityTypeBuilder<TEntity> builder,
            Expression<Func<TEntity, object?>> indexExpression
        )
            where TEntity : class, IEntity
        {
            return builder
                .HasIndex(indexExpression)
                .IsUnique();
        }
    }
}