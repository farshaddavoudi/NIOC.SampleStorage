using System.ComponentModel.DataAnnotations.Schema;

namespace NIOC.SampleStorage.Shared.Core.Dto.Identity
{
    [ComplexType]
    public class CustomPropsDto
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? RahkaranId { get; set; }

        public string? SSOToken { get; set; }

        public string? IdentityTokenId { get; set; }

        public string? PersonnelCode { get; set; }

        public string? UnitName { get; set; }

        public string? JobTitle { get; set; }

        public string? Roles { get; set; }
    }
}
