using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class MobileTask
    {
        public class Request
        {
            public string TaskId { get; set; }
            public string OrgUUID { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string MobileId { get; set; }
            public string AuthKey { get; set; }
            public string Status { get; set; }
        }
        public class Response
        {
            public string Uuid { get; set; }
            public string Number { get; set; }
            public string DateCreate { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
