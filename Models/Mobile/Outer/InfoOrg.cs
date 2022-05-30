using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class InfoOrg
    {
        public class Request
        {
            public string UserUUID { get; set; }
        }
        public class Response
        {
            public List<Organisation> Organisations { get; set; }
            public string ErrorMessage { get; set; }
            public Response() => Organisations = new List<Organisation>();
        }
        public class Organisation
        {
            public string Uuid { get; set; }
            public string Name { get; set; }
            public bool Partner { get; set; }
        }
    }
}
