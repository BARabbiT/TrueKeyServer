using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class NaumeChangePass
    {
        public class Request
        {
            public string Uuid { get; set; }
            public string Password { get; set; }
            public string Login { get; set; }
            public string GetSubMessage { get; set; }
            public string GetEmailMessage { get; set; }
        }
    }
}
