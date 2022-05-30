using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.DB.Models
{
    public class Comment
    {
        [Key]
        public Guid InnerId { get; set; }
        public string TaskId { get; set; }
        public string DateCreate { get; set; }
        public long LastModified { get; set; }
        public string WhoModified { get; set; }
        public string Message { get; set; }
        public string Name { get; set; }
        public string ImageSource { get; set; }
        public string CommentId { get; set; }
        public string UserUUID { get; set; }
    }
}