using NIOC.SampleStorage.Server.Service.NIOCFileHosting.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Server.Service.NIOCFileHosting.Contracts
{
    public interface INIOCFileHostingService
    {
        /// <summary>
        /// Get uploaded file base-URL e.g. "https://cdn.nioc.ir/portal/legal"
        /// </summary>
        string GetBaseUrl();

        /// <summary>
        /// Get full download link to a file e.g. "https://cdn.app.ataair.ir/portal/legal/41047c30-5404-4e33-abe2-b9de0c232c5a.jpg"
        /// </summary>
        /// <param name="filePath">File relative path</param>
        /// <returns></returns>
        string GetDownloadUrl(string filePath);

        /// <summary>
        /// Upload file in ATA CDN and get path Using IFormFile 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> UploadFileAsync(UsingIFormFileArgs args, CancellationToken cancellationToken);

        /// <summary>
        /// Upload file in ATA CDN and get path Using Byte[] data
        /// </summary>
        /// <param name="args"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> UploadFileAsync(UsingByteArrayArgs args, CancellationToken cancellationToken);
    }
}