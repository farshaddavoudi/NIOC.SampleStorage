using Bit.Model.Contracts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NIOC.SampleStorage.Server.Model.Entities.Identity
{
    [Table("IdentityTokens")]
    public class IdentityTokenEntity : NIOCEntity, IArchivableEntity
    {
        public bool IsArchived { get; set; }

        [Required, StringLength(100)]
        public string? ClientName { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset ModifiedOn { get; set; }

        public string? UserDomainName { get; set; }

        public string? IPAddress { get; set; }

        public string? DeviceName { get; set; }

        public override string ToString()
        {
            return $"{nameof(ClientName)}: {ClientName}, UserDomainName: {UserDomainName}";
        }
    }
}