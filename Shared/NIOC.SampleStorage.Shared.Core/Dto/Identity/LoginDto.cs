using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NIOC.SampleStorage.Shared.Core.Dto.Identity
{
    [ComplexType]
    public class LoginDto
    {
        /// <summary>
        /// Domain username
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }

        /// <summary>
        /// SSO header key
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
