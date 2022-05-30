using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class ELMAComment
    {
        public class Request
        {
            public string TaskId { get; set; }
            public string CommentId { get; set; }
            public Date DateCreate { get; set; }
            public string Author { get; set; }
            public string Name { get; set; }
            public string Message { get; set; }
        }
        public class Date
        {
            public Object expFormat { get; set; }
            public string zone { get; set; }
            public string ts { get; set; }
        }
    }
}
