using System.Net.Http;

namespace NIOC.SampleStorage.Server.Service.NIOCSSO
{
    public class SSOClient
    {
        public HttpClient Client { get; }

        public SSOClient(HttpClient client)
        {
            Client = client;
        }
    }
}