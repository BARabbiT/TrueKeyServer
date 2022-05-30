using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.ELMA
{
    public static class AddComment
    {
        public class Request
        {
            [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
            public Context Context { get; set; }
        }
        public class Context
        {
            [JsonProperty("vneshnii", NullValueHandling = NullValueHandling.Ignore)]
            public bool Vneshnii { get; set; }

            [JsonProperty("soobshenie", NullValueHandling = NullValueHandling.Ignore)]
            public string Soobshenie { get; set; }

            [JsonProperty("kontakt", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> kontact { get; set; }

            [JsonProperty("obrashenie", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> Obrashenie { get; set; }

            [JsonProperty("faily", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> files { get; set; }
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
        public class Item
        {
            [JsonProperty("__createdAt", NullValueHandling = NullValueHandling.Ignore)]
            public DateTimeOffset CreatedAt { get; set; }

            [JsonProperty("__id", NullValueHandling = NullValueHandling.Ignore)]
            public string Id { get; set; }
        }

    }
}
