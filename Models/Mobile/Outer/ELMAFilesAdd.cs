using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Outer
{
    public static class ELMAFilesAdd
    {
        public class Request
        {
            public string Uuid { get; set; }
            public List<string> ImageSource { get; set; }
        }
    }
}
