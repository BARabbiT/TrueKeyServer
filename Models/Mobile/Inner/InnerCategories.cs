using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Inner
{
    public class InnerCategories
    {
        [JsonProperty("Categories", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Categories { get; set; }
        public static InnerCategories FromJson(string json) => JsonConvert.DeserializeObject<InnerCategories>(json, Converter.Settings);

    }
}
