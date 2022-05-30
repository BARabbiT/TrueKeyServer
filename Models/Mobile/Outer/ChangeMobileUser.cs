using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class ChangeMobileUser
    {
        public class Request
        {
            public string Uuid { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public bool GetAPush { get; set; }
            public string MobileId { get; set; }
            public bool? GetEmailMessage { get; set; }
            public bool? GetSubMessage { get; set; }
        }
        public class Response
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
