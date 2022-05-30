using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.ELMA
{
    public static class GetUser
    {
        public class Request
        {
            [JsonProperty("active", NullValueHandling = NullValueHandling.Ignore)]
            public bool Active { get; set; }

            [JsonProperty("filter", NullValueHandling = NullValueHandling.Ignore)]
            public Filter Filter { get; set; }

            [JsonProperty("from", NullValueHandling = NullValueHandling.Ignore)]
            public long From { get; set; }

            [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
            public long Size { get; set; }
        }
        public class Filter
        {
            [JsonProperty("tf", NullValueHandling = NullValueHandling.Ignore)]
            public Tf Tf { get; set; }
        }
        public class Tf
        {
            [JsonProperty("_phone", NullValueHandling = NullValueHandling.Ignore)]
            public string Phone { get; set; }

            [JsonProperty("passwordHash", NullValueHandling = NullValueHandling.Ignore)]
            public string Password { get; set; }
        }

        public class Response
        {
            [JsonProperty("success", NullValueHandling = NullValueHandling.Ignore)]
            public bool? Success { get; set; }

            [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
            public string Error { get; set; }

            [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
            public ResponseResult Result { get; set; }
            public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json, Converter.Settings);
        }
        public class ResponseResult
        {
            [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
            public List<ResultElement> Result { get; set; }

            [JsonProperty("total", NullValueHandling = NullValueHandling.Ignore)]
            public long Total { get; set; }
        }
        public class ResultElement
        {
            [JsonProperty("__id", NullValueHandling = NullValueHandling.Ignore)]
            public string Id { get; set; }
            [JsonProperty("__name", NullValueHandling = NullValueHandling.Ignore)]
            public string Name { get; set; }

            [JsonProperty("_phone", NullValueHandling = NullValueHandling.Ignore)]
            public List<Phone> Phone { get; set; }

            [JsonProperty("passwordHash", NullValueHandling = NullValueHandling.Ignore)]
            public string Password { get; set; }

            [JsonProperty("_companies", NullValueHandling = NullValueHandling.Ignore)]
            public List<string> Companies { get; set; }
        }
        public class Phone
        {
            [JsonProperty("tel", NullValueHandling = NullValueHandling.Ignore)]
            public string Tel { get; set; }

            [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
            public string Type { get; set; }
        }
    }
}
