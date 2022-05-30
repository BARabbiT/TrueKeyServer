using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueKeyServer.Models.Mobile.Inner
{
    public partial class CreateCommentResp
    {
        [JsonProperty("UUID", NullValueHandling = NullValueHandling.Ignore)]
        public string Uuid { get; set; }

        [JsonProperty("DateCreate", NullValueHandling = NullValueHandling.Ignore)]
        public string DateCreate { get; set; }
    }

    public partial class CreateCommentResp
    {
        public static CreateCommentResp FromJson(string json) => JsonConvert.DeserializeObject<CreateCommentResp>(json, TrueKeyServer.Models.Mobile.Converter.Settings);
    }
}
