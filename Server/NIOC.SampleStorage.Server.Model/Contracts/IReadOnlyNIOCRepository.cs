using Bit.Data.Contracts;
using Bit.Model.Contracts;

namespace NIOC.SampleStorage.Server.Model.Contracts
{
    public interface IReadOnlyNIOCRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity

    {
    }
}