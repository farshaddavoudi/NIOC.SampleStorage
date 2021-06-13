using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NIOC.SampleStorage.Shared.Core.Dto.Identity
{
    [ComplexType]
    public class RoleDto
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
    }
}