using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class CreateMissedCall
    {
        public class Request
        {
            public string MobileId { get; set; }
        }
        public class Response
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
