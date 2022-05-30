using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.ELMA
{
    public static class Serialize
    {
        public static string ToJsonAddTask(this Models.Mobile.ELMA.AddTask.Request self) => JsonConvert.SerializeObject(self, Models.Mobile.Converter.Settings);
        public static string ToJsonAddComment(this Models.Mobile.ELMA.AddComment.Request self) => JsonConvert.SerializeObject(self, Models.Mobile.Converter.Settings);
        public static string ToJsonGetCategories(this Models.Mobile.ELMA.GetCategories.Request self) => JsonConvert.SerializeObject(self, Models.Mobile.Converter.Settings);
        public static string ToJsonGetUser(this Models.Mobile.ELMA.GetUser.Request self) => JsonConvert.SerializeObject(self, Models.Mobile.Converter.Settings);
        public static string ToJsonAddUpdateUser(this Models.Mobile.ELMA.AddUpdateUser.Request self) => JsonConvert.SerializeObject(self, Models.Mobile.Converter.Settings);
        public static string ToJsonGetUserOrg(this Models.Mobile.ELMA.GetOrgList.Request self) => JsonConvert.SerializeObject(self, Models.Mobile.Converter.Settings);
        public static string ToJsonGetContacts(this Models.Mobile.ELMA.GetContacts.Request self) => JsonConvert.SerializeObject(self, Models.Mobile.Converter.Settings);
    }
}
