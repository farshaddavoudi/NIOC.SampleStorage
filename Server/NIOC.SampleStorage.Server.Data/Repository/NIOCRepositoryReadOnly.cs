using Bit.Data.EntityFrameworkCore.Implementations;
using Bit.Model.Contracts;
using Microsoft.EntityFrameworkCore;
using NIOC.SampleStorage.Server.Model.Contracts;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Server.Data.Repository
{
    public class NIOCRepositoryReadOnly<TEntity> : EfCoreRepository<TEntity>, IReadOnlyNIOCRepository<TEntity>
        where TEntity : class, IEntity
    {
        public IQueryable<TEntity> GetAllWithoutQueryFilter()
        {
            return Set.IgnoreQueryFilters();
        }

        public override IQueryable<TEntity> GetAll()
        {
            Expression<Func<TEntity, bool>>? getFilter = CreateGetFilter();

            IQueryable<TEntity> queryable = base.GetAll();

            if (getFilter != null)
            {
                queryable = queryable.Where(getFilter);
            }

            return queryable;
        }

        public override async Task<IQueryable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            Expression<Func<TEntity, bool>>? getFilter = CreateGetFilter();

            IQueryable<TEntity> queryable = await base.GetAllAsync(cancellationToken);

            if (getFilter != null)
            {
                queryable = queryable.Where(getFilter);
            }

            return queryable;
        }

        protected virtual Expression<Func<TEntity, bool>>? CreateGetFilter()
        {
            Expression<Func<TEntity, bool>>? criteria = null;

            return criteria;
        }
    }
}