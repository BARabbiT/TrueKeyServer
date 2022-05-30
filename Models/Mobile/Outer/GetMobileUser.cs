using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class GetMobileUser
    {
        public class Request
        {
            public string MobilePhone { get; set; }
        }
        public class Response
        {
            public string Uuid { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Login { get; set; }
            public string AuthKey { get; set; }
            public string GetSubMessage { get; set; }
            public string GetEmailMessage { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
