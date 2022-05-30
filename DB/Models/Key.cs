using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.DB.Models
{
    public class Key
    {
        public Guid id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
    }
}
