using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Monitoring
{
    public static class SetFRData
    {
        public class Request
        {
            public List<FRData> FRDatas { get; set; }
        }
        public class Response
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
    }

    public static class GetFRData
    {
        public class Response
        {
            public List<FRData> FRDatas { get; set; }
            public string ErrorMessage { get; set; }
        }
    }

    public class FRData
    {
        public string Owner { get; set; }
        public string Inn { get; set; }
        public string AdressPlaceOfS { get; set; }
        public string NDS { get; set; }
        public string Model { get; set; }
        public string FirmwareID { get; set; }
        public string SerialNumber { get; set; }
        public string RegisterNumber { get; set; }
        public string NomberFN { get; set; }
        public string VersConfigur { get; set; }
        public string VersBoot { get; set; }
        public string VersionFn { get; set; }
        public string OperatorFD { get; set; }
        public string RegFnCount { get; set; }
        public string AmountFnRereg { get; set; }
        public string CheckResource { get; set; }
        public string FFD { get; set; }
        public string EndDataFN { get; set; }
        public string FnExpireDays { get; set; }
        public string QueueDocOFD { get; set; }
        public string NumFirstUnDoc { get; set; }
        public string DateFirstUnDoc { get; set; }
        public string StateInfoEx { get; set; }
        public string LastModifiedDate { get; set; }
        public string LastRegDateFN { get; set; }
    }
}
