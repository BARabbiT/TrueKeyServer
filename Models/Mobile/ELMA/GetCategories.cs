using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.ELMA
{
    public static class GetCategories
    {
        public class Request
        {
            [JsonProperty("active", NullValueHandling = NullValueHandling.Ignore)]
            public bool Active { get; set; }

            [JsonProperty("from", NullValueHandling = NullValueHandling.Ignore)]
            public long From { get; set; }

            [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
            public long Size { get; set; }
        }
        public class Response
        {
            [JsonProperty("success", NullValueHandling = NullValueHandling.Ignore)]
            public bool Success { get; set; }

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
        }
    }
}
