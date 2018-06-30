namespace RestClient.Facade
{
    using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks; 

    public class HttpClientx : IHttpClientx
    {
        HttpClientx httpClient;
        public HttpClientx()
        {
            this.httpClient = new HttpClientx();
        }
    }
}
