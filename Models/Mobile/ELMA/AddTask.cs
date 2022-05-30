using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.ELMA
{
    public static class AddTask
    {
        public class Request
        {
            [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
            public Context Context { get; set; }
        }
        public class Context
        {
            [JsonProperty("__name", NullValueHandling = NullValueHandling.Ignore)]
            public string Name { get; set; }

            [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
            public string Description { get; set; }

            [JsonProperty("referenceType", NullValueHandling = NullValueHandling.Ignore)]
            public List<ReferenceType> ReferenceType { get; set; }

            [JsonProperty("partner", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> Partner { get; set; }

            [JsonProperty("point", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> Point { get; set; }

            [JsonProperty("contactName", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> ContactName { get; set; }

            [JsonProperty("category", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> Category { get; set; }

            [JsonProperty("fails", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> files { get; set; }
        }
        public class ReferenceType
        {
            [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
            public string Code { get; set; }

            [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
            public string Name { get; set; }
        }
        public class Response
        {
            [JsonProperty("success", NullValueHandling = NullValueHandling.Ignore)]
            public bool Success { get; set; }

            [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
            public string Error { get; set; }

            [JsonProperty("item", NullValueHandling = NullValueHandling.Ignore)]
            public Item Item { get; set; }
            public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json, Converter.Settings);
        }
        public partial class Item
        {
            [JsonProperty("__createdAt", NullValueHandling = NullValueHandling.Ignore)]
            public DateTimeOffset? CreatedAt { get; set; }

            [JsonProperty("__id", NullValueHandling = NullValueHandling.Ignore)]
            public string Id { get; set; }

            [JsonProperty("__index", NullValueHandling = NullValueHandling.Ignore)]
            public long Index { get; set; }
        }
    }
}
