using System.ComponentModel.DataAnnotations;

namespace NIOC.SampleStorage.Server.Service.NIOCFileHosting.Dtos
{
    public class UsingByteArrayArgs : FileUploadBaseArgs
    {
        [Required]
        public byte[]? FileData { get; set; }

        /// <summary>
        /// e.g. ".pdf" 
        /// </summary>
        public string? FileExtensionWithDot { get; set; }
    }
}