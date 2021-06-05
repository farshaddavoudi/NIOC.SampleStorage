using Bit.Data.Contracts;
using Bit.Model.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Server.Model.Contracts
{
    public interface INIOCRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        Task<TEntity> GetByIdAsync(int id, CancellationToken cancellationToken,
            params Expression<Func<TEntity, object?>>[]? includes);

        Task<TEntity> GetByIdWithoutQueryFilterAsync(int id, CancellationToken cancellationToken,
            params Expression<Func<TEntity, object?>>[]? includes);

        Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken);

        Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

        IQueryable<TEntity> GetAllWithoutQueryFilter();
    }
}