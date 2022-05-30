using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class InfoAuth
    {
        public class Request
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string MobileId { get; set; }
        }
        public class Response
        {
            public string Uuid { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string AuthKey { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
