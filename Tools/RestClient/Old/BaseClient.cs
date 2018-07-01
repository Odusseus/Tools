using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuickType;
using RestClient.Alfresco;

namespace RestClient.Old
{
    public class BaseClient 
    {
        private readonly HttpClient httpClient;
        private BookList bookList;
        private Book book;

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
            var authenticationBytes = Encoding.ASCII.GetBytes("admin:admin");

            this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                                                                      "Basic",
                                                                      Convert.ToBase64String(authenticationBytes));

            try
            {
                string message = await PostBookAsync();
                if (message == null)
                {
                    //foreach(Entry entry in this.bookList.entries.entries)
                    //{
                    //Console.WriteLine($"id {entry.id} name {entry.name} modifiedAt {entry.modifiedAt}");
                    //}

                    foreach (QuickType.EntryElement entry in this.book.List.Entries)
                    {
                        Console.WriteLine($"id {entry.Entry.Id} name {entry.Entry.Name} modifiedAt {entry.Entry.ModifiedAt}");
                    }
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        private async Task<string> PostBookAsync()
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
                // JObject responseObject = JObject.Parse(responseContent);

                // string totalItems = (string)responseObject["list"]["pagination"]["totalItems"];
                // //JArray entries = (JArray)responseObject["list"]["entries"];

                //// BookList bookList = JsonConvert.DeserializeObject<BookList>(responseContent);

                // dynamic responseObjects = JObject.Parse(responseContent);

                // //var xx = responseObjects.list.pagination.totalItems;

                // this.bookList = new BookList();

                // foreach(dynamic entry in responseObjects.list.entries)
                // {
                //     this.bookList.AddEntry(entry);
                // }

                this.book = JsonConvert.DeserializeObject<Book>(responseContent);
            }
            return null;
        }
    }
}