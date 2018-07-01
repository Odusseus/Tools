﻿using System;
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
                bool isSucceful = await PostBookAsync();
                if (isSucceful)
                {
                    foreach (QuickType.EntryElement entry in this.book.List.Entries)
                    {
                        this.outputWriter.WriteLine($"id {entry.Entry.Id} name {entry.Entry.Name} modifiedAt {entry.Entry.ModifiedAt}");
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
    }
}