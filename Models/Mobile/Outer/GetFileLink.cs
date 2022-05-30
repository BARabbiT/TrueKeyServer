using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public class GetFileLink
    {
        public class Request
        {
            public List<string> FileIds { get; set; }
        }
        public class Response
        {
            public string ErrorMessage { get; set; }
            public List<FileLink> FileLinks { get; set; }
        }
        public class FileLink
        {
            public string id { get; set; }
            public string link { get; set; }
        }
    }
}
