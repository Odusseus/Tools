// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var book = Book.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Book
    {
        [JsonProperty("list", NullValueHandling = NullValueHandling.Ignore)]
        public List List { get; set; }
    }

    public partial class List
    {
        [JsonProperty("pagination", NullValueHandling = NullValueHandling.Ignore)]
        public Pagination Pagination { get; set; }

        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public Context Context { get; set; }

        [JsonProperty("entries", NullValueHandling = NullValueHandling.Ignore)]
        public List<EntryElement> Entries { get; set; }
    }

    public partial class Context
    {
    }

    public partial class EntryElement
    {
        [JsonProperty("entry", NullValueHandling = NullValueHandling.Ignore)]
        public EntryEntry Entry { get; set; }
    }

    public partial class EntryEntry
    {
        [JsonProperty("isFile", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsFile { get; set; }

        [JsonProperty("createdByUser", NullValueHandling = NullValueHandling.Ignore)]
        public EdByUser CreatedByUser { get; set; }

        [JsonProperty("modifiedAt", NullValueHandling = NullValueHandling.Ignore)]
        public string ModifiedAt { get; set; }

        [JsonProperty("nodeType", NullValueHandling = NullValueHandling.Ignore)]
        public string NodeType { get; set; }

        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public Content Content { get; set; }

        [JsonProperty("parentId", NullValueHandling = NullValueHandling.Ignore)]
        public string ParentId { get; set; }

        [JsonProperty("aspectNames", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> AspectNames { get; set; }

        [JsonProperty("createdAt", NullValueHandling = NullValueHandling.Ignore)]
        public string CreatedAt { get; set; }

        [JsonProperty("isFolder", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsFolder { get; set; }

        [JsonProperty("search", NullValueHandling = NullValueHandling.Ignore)]
        public Search Search { get; set; }

        [JsonProperty("modifiedByUser", NullValueHandling = NullValueHandling.Ignore)]
        public EdByUser ModifiedByUser { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public string Location { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
        public Properties Properties { get; set; }
    }

    public partial class Content
    {
        [JsonProperty("mimeType", NullValueHandling = NullValueHandling.Ignore)]
        public string MimeType { get; set; }

        [JsonProperty("mimeTypeName", NullValueHandling = NullValueHandling.Ignore)]
        public string MimeTypeName { get; set; }

        [JsonProperty("sizeInBytes", NullValueHandling = NullValueHandling.Ignore)]
        public long? SizeInBytes { get; set; }

        [JsonProperty("encoding", NullValueHandling = NullValueHandling.Ignore)]
        public string Encoding { get; set; }
    }

    public partial class EdByUser
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("displayName", NullValueHandling = NullValueHandling.Ignore)]
        public string DisplayName { get; set; }
    }

    public partial class Properties
    {
        [JsonProperty("cm:versionLabel", NullValueHandling = NullValueHandling.Ignore)]
        public string CmVersionLabel { get; set; }

        [JsonProperty("cm:lastThumbnailModification", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> CmLastThumbnailModification { get; set; }

        [JsonProperty("cm:versionType", NullValueHandling = NullValueHandling.Ignore)]
        public string CmVersionType { get; set; }
    }

    public partial class Search
    {
        [JsonProperty("score", NullValueHandling = NullValueHandling.Ignore)]
        public double? Score { get; set; }
    }

    public partial class Pagination
    {
        [JsonProperty("count", NullValueHandling = NullValueHandling.Ignore)]
        public long? Count { get; set; }

        [JsonProperty("hasMoreItems", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasMoreItems { get; set; }

        [JsonProperty("totalItems", NullValueHandling = NullValueHandling.Ignore)]
        public long? TotalItems { get; set; }

        [JsonProperty("skipCount", NullValueHandling = NullValueHandling.Ignore)]
        public long? SkipCount { get; set; }

        [JsonProperty("maxItems", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxItems { get; set; }
    }

    public partial class Book
    {
        public static Book FromJson(string json) => JsonConvert.DeserializeObject<Book>(json, QuickType.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Book self) => JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
