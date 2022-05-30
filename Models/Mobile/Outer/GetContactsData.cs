using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class GetContactsData
    {
        public class Request
        {
            public string uuid { get; set; }
        }
        public class Response
        {
            public List<Contact> Contacts { get; set; }

            public string ErrorMessage { get; set; }
        }
        public class Contact
        {
            public string role { get; set; }
            public string phone { get; set; }
            public string name { get; set; }
            public string image { get; set; }
        }
    }
}
