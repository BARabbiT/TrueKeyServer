using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.DB.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        public string timeChange { get; set; }
        public string usersUuids { get; set; }
        public string MessageCreator { get; set; }
        public DateTime dateCreate { get; set; }
        public string linkToObj { get; set; }
        public string msg { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string imageSource { get; set; }
        public string taskNumber { get; set; }
    }
}
