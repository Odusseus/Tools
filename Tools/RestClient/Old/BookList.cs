using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RestClient.Alfresco
{
    public class BookList
    {
        public Pagination pagination { get; set;}
        public Entries entries { get; set;}

        internal void AddEntry(dynamic element)
        {
            if(entries == null)
            {
                this.entries = new Entries();
                this.entries.entries = new List<Entry>();
            }

            Entry newEntry = new Entry
            {
                id = element.entry.id,
                isFile = element.entry.isFile,
                modifiedAt = element.entry.modifiedAt,
                name = element.entry.name
            };
            this.entries.entries.Add(newEntry);

            //this.entries.entries.Add(element.entry);
        }
    }
}
