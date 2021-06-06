namespace NIOC.SampleStorage.Server.Service.NIOCFileHosting.Dtos
{
    public class FileUploadBaseArgs
    {
        /// <summary>
        /// Null means no limit
        /// </summary>
        public long? MaxFileSizeInKiloBytes { get; set; }

        /// <summary>
        /// Specify the subfolder name for file to be saved there, as a categorizing feature. It is optional
        /// </summary>
        public string? SubfolderName { get; set; }

        /// <summary>
        /// Specify another subfolder under main (father) subfolder for file to be saved there, as a categorizing feature. It is optional
        /// </summary>
        public string? ChildSubfolderName { get; set; }

        /// <summary>
        /// Specify another subfolder under defined ChildSubfolder for file to be saved there, as a categorizing feature. It is optional
        /// </summary>
        public string? GrandchildSubfolderName { get; set; }

        /// <summary>
        /// Specify the save base path for your development machine. This property only apply when you are developing. Default is "C:\ProjectsGit\ATAUploads".
        /// </summary>
        public string? DevelopmentBasePath { get; set; } = @"C:\Projects\NIOCUploads";
    }
}