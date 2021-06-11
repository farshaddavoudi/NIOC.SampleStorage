using Bit.Core.Exceptions;
using Microsoft.AspNetCore.Hosting;
using NIOC.SampleStorage.Server.Service.NIOCFileHosting.Contracts;
using NIOC.SampleStorage.Server.Service.NIOCFileHosting.Dtos;
using NIOC.SampleStorage.Shared.App;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NIOC.SampleStorage.Server.Service.NIOCFileHosting.Implementations
{
    public class NIOCFileHostingService : INIOCFileHostingService
    {
        private readonly IHostingEnvironment _environment;

        #region Constructor Injection

        public NIOCFileHostingService(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        #endregion

        private string CDNBasePath => @"C:\SiteSource\cdn\portal";

        public string GetBaseUrl()
        {
            return $"https://cdn.nioc.ir/portal/{AppMetadata.IdentityName}";
        }

        public string GetDownloadUrl(string filePath)
        {
            return $"{CDNBasePath}/{AppMetadata.IdentityName}/{filePath}";
        }

        public async Task<string> UploadFileAsync(UsingIFormFileArgs args, CancellationToken cancellationToken)
        {
            if (args.FormFile!.Length == 0) throw new ArgumentNullException(nameof(args.FormFile));

            var maxSize = args.MaxFileSizeInKiloBytes;
            if (maxSize != null && args.FormFile.Length > maxSize)
                throw new BadRequestException("حجم فایل بیشتر از حد مجاز می‌باشد");

            await using var memoryStream = new MemoryStream();

            await args.FormFile.CopyToAsync(memoryStream, cancellationToken);

            var path = await UploadFileAsync(new UsingByteArrayArgs
            {
                FileData = memoryStream.ToArray(),
                FileExtensionWithDot = MimeTypes.MimeTypeMap.GetExtension(args.FormFile.ContentType),
                MaxFileSizeInKiloBytes = args.MaxFileSizeInKiloBytes,
                DevelopmentBasePath = args.DevelopmentBasePath,
                SubfolderName = args.SubfolderName
            }, cancellationToken);

            return path;
        }

        public async Task<string> UploadFileAsync(UsingByteArrayArgs args, CancellationToken cancellationToken)
        {
            string filePath = "";

            var maxSize = args.MaxFileSizeInKiloBytes;
            if (maxSize != null && args.FileData!.Length > maxSize * 1000)
                throw new BadRequestException("حجم فایل بیشتر از حد مجاز می‌باشد");

            string? basePath = _environment.IsDevelopment() ? args.DevelopmentBasePath : CDNBasePath;

            if (basePath == null) throw new DomainLogicException("هیچ مسیر پایه‌ای برای ذخیره فایل مشخص نشده است");

            basePath = Path.Combine(basePath, AppMetadata.IdentityName);

            CreateDirectoryIfNotExists(basePath);

            if (!string.IsNullOrWhiteSpace(args.SubfolderName))
            {
                basePath = Path.Combine(basePath, args.SubfolderName);
                CreateDirectoryIfNotExists(basePath);
                filePath = args.SubfolderName;

                if (!string.IsNullOrWhiteSpace(args.ChildSubfolderName))
                {
                    basePath = Path.Combine(basePath, args.ChildSubfolderName);
                    CreateDirectoryIfNotExists(basePath);
                    filePath = Path.Combine(filePath, args.ChildSubfolderName);

                    if (!string.IsNullOrWhiteSpace(args.GrandchildSubfolderName))
                    {
                        basePath = Path.Combine(basePath, args.GrandchildSubfolderName);
                        CreateDirectoryIfNotExists(basePath);
                        filePath = Path.Combine(filePath, args.GrandchildSubfolderName);
                    }
                }
            }

            string fileName = $"{Guid.NewGuid()}{args.FileExtensionWithDot}";

            filePath = string.IsNullOrWhiteSpace(filePath) ? fileName : Path.Combine(filePath, fileName);

            string fullPath = Path.Combine(basePath, fileName);

            await using var fileStream = File.Create(fullPath);

            await fileStream.WriteAsync(args.FileData, cancellationToken);

            return filePath;
        }

        private void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}