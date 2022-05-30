using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class InfoCategories
    {
        public class Response
        {
            public List<string> Categories { get; set; }
            public string ErrorMessage { get; set; }

            public Response() => Categories = new List<string>();
        }
    }
}
