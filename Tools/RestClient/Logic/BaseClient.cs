using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using RestClient.Facade;

namespace RestClient.Logic
{
    public class BaseClient : IBaseClient
    {
       private readonly HttpClient httpClient;

        public BaseClient()
        {
            this.httpClient = new HttpClient();
        }

        public bool Run()
        {
            RunAsync().GetAwaiter().GetResult();
            return true;
        }

        public async Task RunAsync()
        {
            this.httpClient.BaseAddress = new Uri("https://api-explorer.alfresco.com");
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                string x = await GetBookAsync("/alfresco/api/-default-/public/search/versions/1/search");

            }
             catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        private async Task<string> GetBookAsync(string path)
        {
            string x = null;
            HttpResponseMessage response = await this.httpClient.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                x = await response.Content.ReadAsStringAsync();
            }
            return x;
        }
    }
}
