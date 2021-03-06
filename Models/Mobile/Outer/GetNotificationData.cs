using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class GetNotificationData
    {
        public class Request
        {
            public string timeLastSync { get; set; }
            public string mobileId { get; set; }
        }
        public class Response
        {
            public List<Message> Notifications { get; set; }
            public string ErrorMessage { get; set; }
        }
        public class Message
        {
            public DateTime dateCreate { get; set; }
            public string linkToObj { get; set; }
            public string msg { get; set; }
            public string title { get; set; }
            public string type { get; set; }
            public string imageSource { get; set; }
        }
    }
}
