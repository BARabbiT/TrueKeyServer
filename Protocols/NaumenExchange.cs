using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RestSharp;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

using InnerModels = TrueKeyServer.Models.Mobile.Inner;

namespace TrueKeyServer.Protocols
{
    public static class NaumenExchange
    {
        private static readonly string key = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["ApiNaumanKey"];
        private static readonly string addres = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["NaumenModuleAddres"];
        private static readonly string loginAddres = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["NaumenLoginAddres"];
        private static readonly string addFileAddres = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["NaumenAddFileAddres"];

        #region Методы для работы с задачами\комментами\файлами\звонками
        public static bool TryCreateMissedCall(string Phone)
        {
            try
            {
                string connectAddr = $"{addres}.CreateMissedCall&params=requestContent&accessKey=";
                var client = new RestClient(connectAddr + key);
                var request = new RestRequest(Method.POST);
                request.AddJsonBody(new { Phone });
                string resp = client.Execute(request).Content;
                Program.Logger.Log("NaumenExchange.TryCreateMissedCall", "Response from namen: " + resp);
                return resp.Contains("interaction");
            }
            catch (Exception ex)
            {
                Program.Logger.Log("NaumenExchange.TryCreateMissedCall", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                return false;
            }
        }
        #endregion

        #region Специальные методы
        public static string GetUuidByOrgId(string OrgId)
        {
            try
            {
                string connectAddr = $"{addres}.ApiFindOrgBy1Cid&params=requestContent&accessKey=";
                var client = new RestClient(connectAddr + key);
                var request = new RestRequest(Method.POST);
                request.AddJsonBody(new { OrgId });
                IRestResponse response = client.Execute(request);
                if (response.Content.Contains("Error"))
                    return null;
                else
                    return response.Content;
            }
            catch (Exception ex)
            {
                Program.Logger.Log("NaumenExchange.GetOrg", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                return null;
            }
        }
        #endregion
    }
}
