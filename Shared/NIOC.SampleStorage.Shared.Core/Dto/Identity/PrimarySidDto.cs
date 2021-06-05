using System.ComponentModel.DataAnnotations.Schema;

namespace NIOC.SampleStorage.Shared.Core.Dto.Identity
{
    [ComplexType]
    public class PrimarySidDto
    {
        public string? UserDomainName { get; set; }

        public CustomPropsDto? CustomProps { get; set; }
    }
}
