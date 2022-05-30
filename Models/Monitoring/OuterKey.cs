using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Monitoring
{
    public static class OuterKey
    {
        public class Request
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }
        public class Response
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
