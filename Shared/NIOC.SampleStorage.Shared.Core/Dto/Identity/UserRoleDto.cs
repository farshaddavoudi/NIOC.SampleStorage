using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NIOC.SampleStorage.Shared.Core.Dto.Identity
{
    [ComplexType]
    public class UserRoleDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User domain name is required")]
        public string? UserDomainName { get; set; }

        [Required(ErrorMessage = "Role name is required")]
        public string? RoleName { get; set; }
    }
}