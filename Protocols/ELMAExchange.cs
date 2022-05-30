using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InnerModels = TrueKeyServer.Models.Mobile.Inner;
using ELMAModels = TrueKeyServer.Models.Mobile.ELMA;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace TrueKeyServer.Protocols
{
    public static class ELMAExchange
    {
        private static readonly string ELMAtoken = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["ELMAToken"];
        private static readonly string ELMAaddres = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AppSettings")["ELMAAddres"];

        #region Методы для работы с задачами\комментами\файлами\звонками
        public static bool TryAddTask(ref InnerModels.InnerTask innerTask)
        {
            try
            {
                var body = new ELMAModels.AddTask.Request()
                {
                    Context = new ELMAModels.AddTask.Context()
                    {
                        Name = innerTask.Title,
                        Description = innerTask.Description,
                        Point = new List<string>(),
                        ContactName = new List<string>(),
                        Category = new List<string>(),
                        ReferenceType = new List<ELMAModels.AddTask.ReferenceType>()
                        {
                               new ELMAModels.AddTask.ReferenceType()
                               {
                                   Name = "Лемма.Онлайн",
                                   Code = "online"
                               }
                        }
                    }
                };
                body.Context.Point.Add(innerTask.OrgUUID);
                body.Context.ContactName.Add(innerTask.Author);
                body.Context.Category.Add("53f604c7-d8fb-411c-81f3-e0728af881a3");
                string resp = RequestToELMA(ELMAModels.Serialize.ToJsonAddTask(body), "create", "service_desk", "handling");
                if (!string.IsNullOrEmpty(resp))
                {
                    ELMAModels.AddTask.Response ELMATask = ELMAModels.AddTask.Response.FromJson(resp);
                    innerTask.TaskId = ELMATask.Item.Id;
                    innerTask.Number = ELMATask.Item.Index.ToString();
                    innerTask.DateCreate = ELMATask.Item.CreatedAt.ToString();
                    return true;
                }
                else
                {
                    throw new Exception("Error on ELMA request. Look previos step");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAExchange.TryAddComment", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
                return false;
            }
        }
        public static bool TryAddComment(ref InnerModels.InnerComment innerComment)
        {
            try
            {
                var body = new ELMAModels.AddComment.Request()
                {
                    Context = new ELMAModels.AddComment.Context()
                    {
                        Soobshenie = innerComment.Message,
                        Vneshnii = true,
                        kontact = new List<string>(),
                        Obrashenie = new List<string>()
                    }
                };
                body.Context.kontact.Add(innerComment.UserUUID);
                body.Context.Obrashenie.Add(innerComment.TaskId);
                string resp = RequestToELMA(ELMAModels.Serialize.ToJsonAddComment(body), "create", "_system_catalogs", "vneshnie_kommentarii");
                if (!string.IsNullOrEmpty(resp))
                {
                    ELMAModels.AddComment.Response ELMAComment = ELMAModels.AddComment.Response.FromJson(resp);
                    innerComment.CommentId = ELMAComment.Item.Id;
                    innerComment.DateCreate = ELMAComment.Item.CreatedAt.ToString();
                    return true;
                }
                else
                {
                    throw new Exception("Error on ELMA request. Look previos step");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAExchange.TryAddComment", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
                return false;
            }
        }
        public static List<string> AddFiles(string uuid, IFormFileCollection files, bool comment = false)
        {
            List<string> filesId = new List<string>();
            List<string> filesUrl = new List<string>();
            try
            {
                foreach (IFormFile file in files)
                {
                    string url = $"{ELMAaddres}/pub/v1/disk/directory/719ac4fe-ce59-45f8-ba26-844469d8ad76/upload";
                    RestClient client = new RestClient(url);
                    RestRequest request = new RestRequest(Method.POST);
                    request.AddParameter(new Parameter("hash", Guid.NewGuid(), ParameterType.QueryString));
                    request.AddHeader("Authorization", "Bearer fa4191cf-e94d-48e6-99ae-dc4bc603c23a");
                    byte[] fileByteArray = null;
                    using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                    {
                        fileByteArray = binaryReader.ReadBytes((int)file.Length);
                    }
                    request.AddFileBytes("file", fileByteArray, file.FileName, file.ContentType);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Models.Mobile.ELMA.AddFiles.Response resp = Models.Mobile.ELMA.AddFiles.Response.FromJson(response.Content);
                        if (resp.Success)
                        {
                            filesId.Add(resp.File.Id);
                        }
                        else
                        {
                            Program.Logger.Log("ELMAExchange.TryAddFiles", $"Error from upload file to ELMA: {resp.Error}", "ERROR");
                        }
                    }
                    else
                    {
                        Program.Logger.Log("ELMAExchange.TryAddFiles", $"Error from upload file to ELMA: {response.StatusCode}", "ERROR");
                    }
                }
                if (filesId.Count > 0)
                {
                    foreach (string id in filesId)
                    {
                        string url = $"{ELMAaddres}/pub/v1/disk/file/{id}/get-link";
                        RestClient client = new RestClient(url);
                        RestRequest request = new RestRequest(Method.POST);
                        request.AddHeader("Authorization", "Bearer fa4191cf-e94d-48e6-99ae-dc4bc603c23a");
                        IRestResponse response = client.Execute(request);
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            Models.Mobile.ELMA.AddFiles.Response resp = Models.Mobile.ELMA.AddFiles.Response.FromJson(response.Content);
                            if (resp.Success)
                            {
                                filesUrl.Add(resp.Link);
                            }
                            else
                            {
                                Program.Logger.Log("ELMAExchange.TryAddFiles", $"Error from get file link from ELMA: {resp.Error}", "ERROR");
                            }
                        }
                    }
                    if (comment)
                    {
                        var body = new ELMAModels.AddComment.Request()
                        {
                            Context = new ELMAModels.AddComment.Context()
                            {
                                files = filesId
                            }
                        };
                        string resp = RequestToELMA(ELMAModels.Serialize.ToJsonAddComment(body), "update", "_system_catalogs", "vneshnie_kommentarii", uuid);
                        if (string.IsNullOrEmpty(resp) || !ELMAModels.AddComment.Response.FromJson(resp).Success)
                        {
                            throw new Exception("Error on ELMA request. Look previos step");
                        }
                    }   
                    else
                    {
                        var body = new ELMAModels.AddTask.Request()
                        {
                            Context = new ELMAModels.AddTask.Context()
                            {
                                files = filesId
                            }
                        };
                        string resp = RequestToELMA(ELMAModels.Serialize.ToJsonAddTask(body), "update", "service_desk", "handling", uuid);
                        if (string.IsNullOrEmpty(resp) || !ELMAModels.AddTask.Response.FromJson(resp).Success)
                        {
                            throw new Exception("Error on ELMA request. Look previos step");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("NaumenExchange.PostFile", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
            }
            return filesUrl;
        }
        public static string GetFileLink (string fileId)
        {
            string fileLink = string.Empty;
            try
            {
                string resp = RequestToELMA(string.Empty, "getFileLink", "", "", fileId);
                if (!string.IsNullOrEmpty(resp))
                {
                    ELMAModels.GetLink.Response response = ELMAModels.GetLink.Response.FromJson(resp);
                    if (response.Success)
                    {
                        fileLink = response.Link;
                    }
                    else
                    {
                        Program.Logger.Log("ELMAExchange.GetFileLink", $"Error from get file link from file with id {fileId}. Error: {response.Error}", "ERROR");
                    }
                }
                else
                {
                    throw new Exception("Error on ELMA request. Look previos step");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAExchange.GetFileLink", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");

            }
            return fileLink;
        }
        public static InnerModels.InnerCategories GetCategories()
        {
            InnerModels.InnerCategories categories = new InnerModels.InnerCategories() { Categories = new List<string>() };
            try
            {
                int skip = 0;
                int count = 100;
                int returnCount = 100;
                while (returnCount == count)
                {
                    var body = new ELMAModels.GetCategories.Request()
                    {
                        Active = true,
                        Size = 100,
                        From = skip
                    };
                    string response = RequestToELMA(ELMAModels.Serialize.ToJsonGetCategories(body), "list", "_system_catalogs", "requestCategories");
                    if (!string.IsNullOrEmpty(response))
                    {
                        ELMAModels.GetCategories.Response returnData = ELMAModels.GetCategories.Response.FromJson(response);
                        returnCount = returnData.Result.Result.Count;
                        skip += returnCount;
                        foreach (var date in returnData.Result.Result) categories.Categories.Add(date.Name);
                    }
                    else
                    {
                        Program.Logger.Log("ELMAExchange.GetCategories", "Request is fault.", "ERROR");
                        returnCount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAExchange.GetCategories", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
            }
            return categories;
        }
        #endregion

        #region Методы для работы с авторизацией\регистрацией
        public static bool TryRegisterUser(ref InnerModels.InnerUser user)
        {
            try
            {
                if (TryAddUpdateUser(ref user, TryGetUser(ref user, true))) return true;
                else
                {
                    throw new Exception("Error in registration to ELMA. See previus step");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAExchange.TryRegisterUser", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                return false;
            }
        }
        public static bool TryAuthUser(ref InnerModels.InnerUser user)
        {
            try
            {
                if (TryGetUser(ref user, user.UUID == null))
                {
                    return true;
                }
                else if (TryGetUser(ref user, true))
                {
                    return true;
                }
                else
                {
                    throw new Support.WarnException("Authorization to ELMA failed.");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAExchange.TryAuthUser", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                return false;
            }
        }
        public static bool TryGetUser(ref InnerModels.InnerUser user, bool firstAuth = false)
        {
            try
            {
                ELMAModels.GetUser.Tf tf = new ELMAModels.GetUser.Tf();
                if (firstAuth)
                {
                    tf.Phone = user.LoginMp;
                }
                else
                {
                    tf.Phone = user.LoginSd;
                }
                var body = new ELMAModels.GetUser.Request()
                {
                    Active = true,
                    From = 0,
                    Size = 1,
                    Filter = new ELMAModels.GetUser.Filter()
                    {
                        Tf = tf
                    }
                };
                string resp = RequestToELMA(ELMAModels.Serialize.ToJsonGetUser(body), "list", "_clients", "_contacts");
                if (!string.IsNullOrEmpty(resp))
                {
                    ELMAModels.GetUser.Response ELMAUser = ELMAModels.GetUser.Response.FromJson(resp);
                    if (ELMAUser.Result.Result != null && ELMAUser.Result.Result.Count > 0)
                    {
                        user.UUID = ELMAUser.Result.Result[0].Id;
                        user.LoginSd = ELMAUser.Result.Result[0].Phone[0].Tel;
                        user.PasswordSd = ELMAUser.Result.Result[0].Password;
                        user.Name = ELMAUser.Result.Result[0].Name;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new Exception("Error on ELMA request. Look previos step");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAExchange.GetUser", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
                return false;
            }
        }
        public static bool TryAddUpdateUser(ref InnerModels.InnerUser user, bool update = false)
        {
            try
            {
                var body = new ELMAModels.AddUpdateUser.Request()
                {
                    Context = new ELMAModels.AddUpdateUser.Context()
                    {
                        Name = user.Name,
                        Password = user.PasswordMp,
                        Phone = new List<ELMAModels.AddUpdateUser.Phone>()
                    }
                };
                if (!string.IsNullOrEmpty(user.OrgIds))
                {
                    string orgId = GetOrgByMobileId(user.OrgIds);
                    if (!string.IsNullOrEmpty(orgId)) body.Context.Companies = new List<string>() { orgId };
                }
                body.Context.Phone.Add(new ELMAModels.AddUpdateUser.Phone()
                {
                    Tel = user.LoginMp,
                    Type = "mobile"
                });
                string resp = RequestToELMA(ELMAModels.Serialize.ToJsonAddUpdateUser(body), update ? "update" : "create", "_clients", "_contacts", user.UUID);
                if (!string.IsNullOrEmpty(resp))
                {
                    ELMAModels.AddUpdateUser.Response ELMAUser = ELMAModels.AddUpdateUser.Response.FromJson(resp);
                    user.UUID = ELMAUser.Item.Id;
                    user.LoginSd = ELMAUser.Item.Phone[0].Tel;
                    user.PasswordSd = ELMAUser.Item.Password;
                    user.Phone = ELMAUser.Item.Phone[0].Tel;
                    user.Name = ELMAUser.Item.Name;
                    return true;
                }
                else
                {
                    throw new Exception("Error on ELMA request. Look previos step");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAExchange.AddUpdateUser", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
                return false;
            }
        }
        public static List<(string, string, bool)> GetUserOrg(string id)
        {
            List<(string, string, bool)> response = new List<(string, string, bool)>();
            try
            {
                var body = new ELMAModels.GetOrgList.Request()
                {
                    Active = true,
                    From = 0,
                    Size = 100,
                    Filter = new ELMAModels.GetOrgList.Filter()
                    {
                        Tf = new ELMAModels.GetOrgList.Tf()
                        {
                            KontaktnyeLica = new List<string>() { id }
                        }
                    }
                };
                string resp = RequestToELMA(ELMAModels.Serialize.ToJsonGetUserOrg(body), "list", "_system_catalogs", "clientPoints");
                if (!string.IsNullOrEmpty(resp))
                {
                    foreach (var org in ELMAModels.GetOrgList.Response.FromJson(resp).Result.Result)
                    {
                        response.Add((org.Name, org.Id, false));
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAExchange.GetUser", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
            }
            return response;
        }
        public static string GetOrgByMobileId(string mobileId)
        {
            string result = string.Empty;
            try
            {
                var body = new ELMAModels.GetOrgList.Request()
                {
                    Active = true,
                    From = 0,
                    Size = 100,
                    Filter = new ELMAModels.GetOrgList.Filter()
                    {
                        Tf = new ELMAModels.GetOrgList.Tf()
                        {
                            MobileId = mobileId
                        }
                    }
                };
                string resp = RequestToELMA(ELMAModels.Serialize.ToJsonGetUserOrg(body), "list", "_system_catalogs", "clientPoints");
                if (!string.IsNullOrEmpty(resp))
                {
                    var orglist = ELMAModels.GetOrgList.Response.FromJson(resp);
                    if (orglist.Success && orglist.Result.Result.Count > 0) result = orglist.Result.Result[0].Id;
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAExchange.GetOrgByMobileId", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
            }
            return result;
        }
        public static List<(string name,string phone ,string role)> GetContacts(string userId)
        {
            List<(string name, string phone, string role)> contactList = new List<(string name, string phone, string role)>();
            try
            {
                var body = new ELMAModels.GetContacts.Request()
                {
                    Active = true,
                    From = 0,
                    Size = 100,
                    Filter = new ELMAModels.GetContacts.Filter()
                    {
                        Tf = new ELMAModels.GetContacts.Tf()
                        {
                            KontaktnyeLica = new List<string>() { userId }
                        }
                    }
                };
                string resp = RequestToELMA(ELMAModels.Serialize.ToJsonGetContacts(body), "list", "_system_catalogs", "clientPoints");
                if (!string.IsNullOrEmpty(resp))
                {
                    ELMAModels.GetContacts.Response responsePoint = ELMAModels.GetContacts.Response.FromJson(resp);
                    if (responsePoint.Result.Result.Count > 0)
                    {
                        body.Filter.Tf = new ELMAModels.GetContacts.Tf()
                        {
                            KontaktnyeLica = new List<string>() { userId },
                            Id = responsePoint.Result.Result[0].Partner[0]
                        };
                    }
                    else
                    {
                        body.Filter.Tf = new ELMAModels.GetContacts.Tf()
                        {
                            KontaktnyeLica = new List<string>() { userId }
                        };
                    }
                    resp = RequestToELMA(ELMAModels.Serialize.ToJsonGetContacts(body), "list", "_clients", "_companies");
                    if (!string.IsNullOrEmpty(resp))
                    {
                        var responsePertner = ELMAModels.GetContacts.Response.FromJson(resp);
                        if (responsePertner.Result.Result.Count>0)
                        {
                            if (responsePertner.Result.Result[0].Manager != null && responsePertner.Result.Result[0].Manager.Count > 0)
                            {
                                body = new ELMAModels.GetContacts.Request()
                                {
                                    Ids = new List<string>() { responsePertner.Result.Result[0].Manager[0] },
                                    Active = true,
                                    Size = 100,
                                    From = 0
                                };
                                resp = RequestToELMA(ELMAModels.Serialize.ToJsonGetContacts(body), "listUser", "", "");
                                if (!string.IsNullOrEmpty(resp))
                                {
                                    var response = ELMAModels.GetContacts.Response.FromJson(resp);
                                    if (response.Result.Result.Count > 0)
                                    {
                                        string name = string.Empty;
                                        string phone = string.Empty;
                                        if (!string.IsNullOrEmpty(response.Result.Result[0].Name)) name = response.Result.Result[0].Name;
                                        if (response.Result.Result[0].Phone != null && response.Result.Result[0].Phone.Count > 0) phone = response.Result.Result[0].Phone[0].Tel;
                                        contactList.Add((name, phone, "KAM"));
                                    }
                                }
                            }
                            

                            if (responsePertner.Result.Result[0].ClientManager != null && responsePertner.Result.Result[0].ClientManager.Count > 0)
                            {
                                body = new ELMAModels.GetContacts.Request()
                                {
                                    Ids = new List<string>() { responsePertner.Result.Result[0].ClientManager[0] },
                                    Active = true,
                                    Size = 100,
                                    From = 0
                                };
                                resp = RequestToELMA(ELMAModels.Serialize.ToJsonGetContacts(body), "listUser", "", "");
                                if (!string.IsNullOrEmpty(resp))
                                {
                                    var response = ELMAModels.GetContacts.Response.FromJson(resp);
                                    if (response.Result.Result.Count > 0)
                                    {
                                        string name = string.Empty;
                                        string phone = string.Empty;
                                        if (!string.IsNullOrEmpty(response.Result.Result[0].Name)) name = response.Result.Result[0].Name;
                                        if (response.Result.Result[0].Phone != null && response.Result.Result[0].Phone.Count > 0) phone = response.Result.Result[0].Phone[0].Tel;
                                        contactList.Add((name, phone, "KM"));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAExchange.GetContacts", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
            }
            return contactList;
        }
        #endregion

        #region Служебные методы
        private static string RequestToELMA(string jsonBody, string typeRequest, string nameSpace, string appName, string id = "")
        {
            string responseText = string.Empty;
            try
            {
                string url = $"{ELMAaddres}/pub/v1/app/{nameSpace}/{appName}/{typeRequest}";
                if (typeRequest == "listUser") url = $"{ELMAaddres}/pub/v1/user/list";
                if (typeRequest == "update") url = $"{ELMAaddres}/pub/v1/app/{nameSpace}/{appName}/{id}/{typeRequest}";
                if (typeRequest == "getFileLink") url = $"{ELMAaddres}/pub/v1/disk/file/{id}/get-link";
                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest(Method.POST);
                request.AddHeader("X-Token", ELMAtoken);
                //request.AddHeader("Authorization", $"Bearer {ELMAtoken}");
                if (!string.IsNullOrEmpty(jsonBody)) request.AddJsonBody(jsonBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    responseText = response.Content;
                }
                else
                {
                    Program.Logger.Log("ELMAExchange.RequestToELMA", $"Request return status code is {response.StatusCode}", "ERROR");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAExchange.RequestToELMA", ex.ToString(), "ERROR");
            }
            return responseText;
        }
        #endregion
    }
}