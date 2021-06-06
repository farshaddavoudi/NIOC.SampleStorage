using System.ComponentModel.DataAnnotations.Schema;

namespace NIOC.SampleStorage.Shared.Core.Dto.Identity
{
    [ComplexType]
    public class LoginDto
    {
        /// <summary>
        /// Domain username
        /// </summary>
        public string? UserDomainName { get; set; }

        /// <summary>
        /// SSO header key
        /// </summary>
        public string? HeaderKey { get; set; }
    }
}
