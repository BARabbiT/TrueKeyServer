using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class InfoComment
    {
        public class Request
        {
            public string TaskId { get; set; }
            public string LastSync { get; set; }
            public string MobileId { get; set; }
        }
        public class Response
        {
            public List<Comment> Comments { get; set; }
            public string ErrorMessage { get; set; }

            public Response() => Comments = new List<Comment>();
        }
        public class Comment
        {
            public string TaskId { get; set; }
            public string UserUUID { get; set; }
            public string CommentId { get; set; }
            public string Message { get; set; }
            public string DateCreate { get; set; }
            public string Name { get; set; }
            public List<string> ImageSource { get; set; }

            public Comment() => ImageSource = new List<string>();
        }
    }
}
