using System.ComponentModel.DataAnnotations.Schema;

namespace NIOC.SampleStorage.Shared.Core.Dto.Identity
{
    [ComplexType]
    public class LoginDto
    {
        /// <summary>
        /// User SSOToken
        /// </summary>
        public string? UserDomainName { get; set; }

        /// <summary>
        /// Application name in Security system
        /// </summary>
        public string? AppName { get; set; }
    }
}
