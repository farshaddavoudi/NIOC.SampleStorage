using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NIOC.SampleStorage.Shared.Core.Dto
{
    [ComplexType]
    public class UserDto
    {
        public string? UserDomainName { get; set; }

        public int? RahkaranId { get; set; }

        public string? SSOToken { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public int PersonnelCode { get; set; }

        public string? Mobile { get; set; }

        public string? UnitName { get; set; }

        public string? JobTitle { get; set; }

        public int? WorkLocationCode { get; set; }

        public string? WorkLocation { get; set; }

        public List<string>? Roles { get; set; }

        public string? IdentityTokenId { get; set; }

        public UserJobPosition? JobPosition { get; set; }


    }

    [ComplexType]
    public class UserJobPosition
    {
        public int? BoxID { get; set; }
        public string? BoxName { get; set; }
        public int? UnitBoxId { get; set; }
        public string? UnitName { get; set; }
        public int? ParentBoxID { get; set; }
        public string? ParentBoxName { get; set; }
        public int? PostID { get; set; }
        public string? PostTitle { get; set; }
    }
}