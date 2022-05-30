using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.DB.Models
{
    public class Task
    {
        [Key]
        public Guid InnerId { get; set; }
        public string Number { get; set; }
        public string TaskId { get; set; }
        public string OrgUUID { get; set; }
        public string DateCreate { get; set; }
        public long LastModified { get; set; }
        public string WhoModified { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageSource { get; set; }
        public string Status { get; set; }
        public string Author { get; set; }
        public string Subscribers { get; set; }
    }
}
