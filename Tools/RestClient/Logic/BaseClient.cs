using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using QuickType;
using RestClient.Facade;

namespace RestClient.Logic
{
    public class BaseClient : IBaseClient
    {
        private readonly HttpClient httpClient;
        private Book book;
        private readonly IOutputWriter outputWriter;

        public BaseClient(IOutputWriter outputWriter)
        {
            this.httpClient = new HttpClient();
            this.outputWriter = outputWriter;
        }

        public bool Run()
        {
            RunAsync().GetAwaiter().GetResult();
            return true;
        }

        private async Task RunAsync()
        {
            this.httpClient.BaseAddress = new Uri("https://api-explorer.alfresco.com");
            this.httpClient.DefaultRequestHeaders.Accept.Clear();
            this.httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var authenticationBytes = Encoding.ASCII.GetBytes("admin:admin");

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                                                                      "Basic",
                                                                      Convert.ToBase64String(authenticationBytes));

            try
            {
                bool isSucceful = await PostBookAsync();
                if (isSucceful)
                {
                    foreach (QuickType.EntryElement entry in this.book.List.Entries)
                    {
                        this.outputWriter.WriteLine($"id {entry.Entry.Id} name {entry.Entry.Name} modifiedAt {entry.Entry.ModifiedAt}");

                        bool isDownloaded = RunDownload(entry.Entry.Id, entry.Entry.Name);
                    }
                }
                else
                {
                    this.outputWriter.WriteLine("PostBookAsync is not succeed.");
                }
            }
            catch (Exception e)
            {
                this.outputWriter.WriteLine(e.Message);
            }

            this.outputWriter.WriteLine("SVP Press any key to continue.");
            Console.ReadLine();
        }
        private async Task<bool> PostBookAsync()
        {
            string responseContent = null;
            string requestUri = "https://api-explorer.alfresco.com/alfresco/api/-default-/public/search/versions/1/search";

            HttpContent content = new StringContent(
                                   @"{
                                        ""query"": {
                                            ""query"": ""name: '928_4.PDF'""
                                          },
                                          ""include"": [""aspectNames"", ""properties""]
                                      }");

            HttpResponseMessage response = await this.httpClient.PostAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                responseContent = await response.Content.ReadAsStringAsync();
                this.book = Book.FromJson(responseContent);
            }

            return true;
        }

        private bool RunDownload(string id, string name)
        {
            DownloadFile(id, name).GetAwaiter().GetResult();
            return true;
        }

        private async Task DownloadFile(string id, string name)
        {
            var fileStream = await this.httpClient.GetStreamAsync("https://api-explorer.alfresco.com/alfresco/api/-default-/public/alfresco/versions/1/nodes/4a2919e1-d82d-428b-86f7-dfb32ca2c413/content?attachment=true");
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                using (FileStream file = new FileStream($"{id}-{name}", FileMode.Create, FileAccess.Write)) {
                    memoryStream.WriteTo(file);
                }
            }
            
        }
    }
}