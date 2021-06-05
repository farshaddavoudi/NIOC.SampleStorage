using System.ComponentModel.DataAnnotations.Schema;

namespace NIOC.SampleStorage.Shared.Core.Dto.Identity
{
    [ComplexType]
    public class LoginJwtDto
    {
        public string? AccessToken { get; set; }

        public int ExpiresIn { get; set; }

        public string? TokenType { get; set; }
    }
}
