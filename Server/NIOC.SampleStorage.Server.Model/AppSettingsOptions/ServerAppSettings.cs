namespace NIOC.SampleStorage.Server.Model.AppSettingsOptions
{
    public class ServerAppSettings
    {
        public ConnectionStringOptions? ConnectionStringOptions { get; set; }

        public UrlOptions? UrlOptions { get; set; }

        public UploadOptions? UploadOptions { get; set; }
    }
}