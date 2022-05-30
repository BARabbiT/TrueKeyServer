using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class InfoRegister
    {
        public class Request
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public string OrgIds { get; set; }
            public bool NoQrReg { get; set; }
            public string MobileId { get; set; }
            public bool? GetEmailMessage { get; set; }
            public bool? GetSubMessage { get; set; }
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
