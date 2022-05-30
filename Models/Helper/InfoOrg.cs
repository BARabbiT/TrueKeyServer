using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TrueKeyServer.Models.Helper
{
    public static class InfoOrg
    {
        public class Request
        {
            public string OrgId { get; set; }
        }
        public class Response
        {
            public string Uuid { get; set; }
            public string ErrorMessage { get; set; }
            public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json, TrueKeyServer.Models.Mobile.Converter.Settings);
        }
    }
}
