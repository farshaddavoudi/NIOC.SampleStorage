namespace NIOC.SampleStorage.Server.Model.Entities.Contracts
{
    public interface INIOCEntity<TKey> : IAuditableEntity //, IArchivableEntity
    {
        TKey Id { get; set; }
    }

    public interface INIOCEntity : INIOCEntity<int>
    {
    }


}