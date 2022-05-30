using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.ELMA
{
    public class AddFiles
    {
        public class Request
        {

        }

        public class Response
        {
            [JsonProperty("success", NullValueHandling = NullValueHandling.Ignore)]
            public bool Success { get; set; }

            [JsonProperty("error", NullValueHandling = NullValueHandling.Ignore)]
            public string Error { get; set; }

            [JsonProperty("Link", NullValueHandling = NullValueHandling.Ignore)]
            public string Link { get; set; }

            [JsonProperty("file", NullValueHandling = NullValueHandling.Ignore)]
            public ELMAFile File { get; set; }

            public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json, Converter.Settings);
        }
        public partial class ELMAFile
        {
            [JsonProperty("__id", NullValueHandling = NullValueHandling.Ignore)]
            public string Id { get; set; }
        }
    }
}
