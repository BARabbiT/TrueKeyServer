using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class MobileComment
    {
        public class Request
        {
            public string TaskId { get; set; }
            public string UserUUID { get; set; }
            public string Message { get; set; }
            public string MobileId { get; set; }
        }
        public class Response
        {
            public string Uuid { get; set; }
            public string DateCreate { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
