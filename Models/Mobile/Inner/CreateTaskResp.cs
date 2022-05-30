using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueKeyServer.Models.Mobile.Inner
{
    public partial class CreateTaskResp
    {
        [JsonProperty("UUID", NullValueHandling = NullValueHandling.Ignore)]
        public string Uuid { get; set; }

        [JsonProperty("Number", NullValueHandling = NullValueHandling.Ignore)]
        public string Number { get; set; }

        [JsonProperty("DateCreate", NullValueHandling = NullValueHandling.Ignore)]
        public string DateCreate { get; set; }
    }

    public partial class CreateTaskResp
    {
        public static CreateTaskResp FromJson(string json) => JsonConvert.DeserializeObject<CreateTaskResp>(json, TrueKeyServer.Models.Mobile.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this CreateTaskResp self) => JsonConvert.SerializeObject(self, TrueKeyServer.Models.Mobile.Converter.Settings);
    }
}
