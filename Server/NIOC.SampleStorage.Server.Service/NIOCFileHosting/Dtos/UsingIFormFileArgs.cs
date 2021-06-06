using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NIOC.SampleStorage.Server.Service.NIOCFileHosting.Dtos
{
    public class UsingIFormFileArgs : FileUploadBaseArgs
    {
        [Required]
        public IFormFile? FormFile { get; set; }
    }
}