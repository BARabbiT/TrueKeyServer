using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Inner
{
    public class InnerUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UUID { get; set; }
        public string OrgIds { get; set; }
        public bool NoQrReg { get; set; }
        public string AuthKey { get; set; }
        public string Phone { get; set; }
        public string LoginMp { get; set; }
        public string PasswordMp { get; set; }
        public string LoginSd { get; set; }
        public string PasswordSd { get; set; }
        public bool? GetSubMessage { get; set; }
        public bool? GetEmailMessage { get; set; }
        public List<string> MobileIds { get; set; }
        public InnerUser() => MobileIds = new List<string>();

        public void AddMobileId(string mobileId)
        {
            if (!string.IsNullOrEmpty(mobileId) && !MobileIds.Contains(mobileId)) MobileIds.Add(mobileId);
        }
    }

}
