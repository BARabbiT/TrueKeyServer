using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class ELMATask
    {
        public class Request
        {
            public string TaskId { get; set; }
            public string Number { get; set; }
            public string OrgUUID { get; set; }
            public Date DateCreate { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public List<String> ImageSource { get; set; }
            public string Status { get; set; }
            public string Author { get; set; }
            public string Subscribers { get; set; }
        }
        public class Date
        {
            public Object expFormat { get; set; }
            public string zone { get; set; }
            public string ts { get; set; }
        }
    }
}