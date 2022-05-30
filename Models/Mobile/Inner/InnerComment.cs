using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Inner
{
    public class InnerComment
    {
        public string TaskId { get; set; }
        public string CommentId { get; set; }
        public string UserUUID { get; set; }
        public string DateCreate { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public List<String> ImageSource { get; set; }
        public string AuthKey { get; set; }
        public string MobileId { get; set; }

        public InnerComment() { }
        public InnerComment(Outer.MobileComment.Request comment)
        {
            TaskId = comment.TaskId;
            UserUUID = comment.UserUUID;
            Message = comment.Message;
            MobileId = comment.MobileId;
        }
        public InnerComment(Outer.ELMAComment.Request comment)
        {
            TaskId = comment.TaskId;
            UserUUID = comment.Author;
            Message = comment.Message;
            CommentId = comment.CommentId;
            DateCreate = comment.DateCreate.ts;
            Name = comment.Name;
        }
    }
}
