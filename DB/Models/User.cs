using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TrueKeyServer.DB.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string UUID { get; set; }
        public string LoginMp { get; set; }
        public string PasswordMp { get; set; }
        public string LoginSd { get; set; }
        public string PasswordSd { get; set; }
        public string AuthKey { get; set; }
        public string MobileIds { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public bool? GetSubMessage { get; set; }
        public bool? GetEmailMessage { get; set; }
    }
}
