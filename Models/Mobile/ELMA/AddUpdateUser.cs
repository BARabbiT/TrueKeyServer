using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.ELMA
{
    public static class AddUpdateUser
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

            [JsonProperty("_phone", NullValueHandling = NullValueHandling.Ignore)]
            public List<Phone> Phone { get; set; }

            [JsonProperty("_companies", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> Companies { get; set; }

            [JsonProperty("passwordHash", NullValueHandling = NullValueHandling.Ignore)]
            public string Password { get; set; }
        }
        public class Phone
        {
            [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
            public string Type { get; set; }

            [JsonProperty("tel", NullValueHandling = NullValueHandling.Ignore)]
            public string Tel { get; set; }
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
            [JsonProperty("__id", NullValueHandling = NullValueHandling.Ignore)]
            public string Id { get; set; }

            [JsonProperty("__name", NullValueHandling = NullValueHandling.Ignore)]
            public string Name { get; set; }

            [JsonProperty("_companies", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> Companies { get; set; }

            [JsonProperty("_phone", NullValueHandling = NullValueHandling.Ignore)]
            public List<Phone> Phone { get; set; }

            [JsonProperty("passwordHash", NullValueHandling = NullValueHandling.Ignore)]
            public string Password { get; set; }
        }
    }
}
