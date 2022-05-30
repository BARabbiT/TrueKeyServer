using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class InfoExchange
    {
        public class Request
        {
            public List<string> OrgUUID { get; set; }
            public string LastSync { get; set; }
            public string Mobileid { get; set; }
        }
        public class Response
        {
            public List<Task> Tasks { get; set; }
            public string ErrorMessage { get; set; }
            public Response() => Tasks = new List<Task>();
        }
        public class Task
        {
            public string Number { get; set; }
            public string TaskId { get; set; }
            public string OrgUUID { get; set; }
            public string DateCreate { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Status { get; set; }
            public List<string> ImageSource { get; set; }
            public List<Comment> Comments { get; set; }
            public Task()
            {
                ImageSource = new List<string>();
                Comments = new List<Comment>();
            }
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
