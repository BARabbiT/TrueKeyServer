using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.DB.Models
{
    public class Organisation
    {
        [Key]
        public Guid Id { get; set; }
        public long OrgId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
    }
}
