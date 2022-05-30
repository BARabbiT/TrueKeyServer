using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Inner
{
    public class Message
    {
        public DateTime dateCreate { get; set; }
        public string linkToObj { get; set; }
        public string msg { get; set; }
        public string title { get; set; }
        public string type { get; set; }
        public string imageSource { get; set; }
        public string messageCreator { get; set; }
        public string timeChange { get; set; }
    }
}
