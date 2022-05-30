using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using ELMAExchange = TrueKeyServer.Protocols.ELMAExchange;
using InnerModels = TrueKeyServer.Models.Mobile.Inner;
using OuterModels = TrueKeyServer.Models.Mobile.Outer;

namespace TrueKeyServer.Controllers.MobileControllersV2
{
    [ApiController]
    public class AuthControllers : Controller
    {
        private readonly DB.MobileInterfaces.IDBWorkerAuth _IDBWorkerAuth;
        public AuthControllers(DB.MobileInterfaces.IDBWorkerAuth IDBWorkerAuth)
        {
            _IDBWorkerAuth = IDBWorkerAuth;
        }

        /// <summary>
        /// Запрос для регистрации пользоватетей.
        /// </summary>
        ///
        /// <remarks>
        /// Регистрирует пользователя МП. Фактически, связывает пользователя МП с пользователем в наумане.
        /// Если в наумане его нет, то создает его.
        /// - Login - логин для дальнейшей авторизации в МП
        /// - Password - пароль для дальнейшей авторизации в МП
        /// - Name - имя пользователя
        /// - OrgIds - идентификатор организации пользователя из 1C или же UUID организации из науман.Если указывается именно UUID, то следующее поле должно быть true
        /// - NoQrReg - булево поле обозначающее регистрацию без QR и то, что в OrgIds передается UUID из наумана
        /// - MobileId - идентификатор установки(идентифкатор МП) используется для пушей
        /// </remarks>
        ///
        /// <response code="200">
        /// Возвращает объект пользователя при успешной регистрации.
        /// - userUUID - UUID пользователя в наумен
        /// - name - имя пользователя в наумен
        /// - phone - телефонный номер пользователя в наумен
        /// - authKey - авторизационный ключ, для использования при создании\редактировании задач.
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/RegisterMobile")]
        [HttpPost]
        [ProducesResponseType(typeof(OuterModels.InfoRegister.Response), 200)]
        public OuterModels.InfoRegister.Response RegisterMobile(OuterModels.InfoRegister.Request infoRegister)
        {
            Program.Logger.Log("MobileControllersV1.Register", $"Received an registration request. Login: {infoRegister.Login}, Password: {infoRegister.Password}, MobileId: {infoRegister.MobileId}");
            OuterModels.InfoRegister.Response response = new OuterModels.InfoRegister.Response();
            try
            {
                InnerModels.InnerUser innerUser = _IDBWorkerAuth.UserGet(infoRegister.Login, infoRegister.Password);
                if (innerUser.LoginMp == null)
                {
                    innerUser.LoginMp = infoRegister.Login;
                    innerUser.PasswordMp = infoRegister.Password;
                    innerUser.Name = infoRegister.Name;
                    innerUser.LoginSd = infoRegister.Login;
                    innerUser.PasswordSd = infoRegister.Password;
                    innerUser.OrgIds = infoRegister.OrgIds;
                    innerUser.NoQrReg = infoRegister.NoQrReg;
                    innerUser.GetSubMessage = infoRegister.GetSubMessage;
                    innerUser.GetEmailMessage = infoRegister.GetEmailMessage;
                    innerUser.Id = Guid.NewGuid();
                }

                if (ELMAExchange.TryRegisterUser(ref innerUser))
                {
                    _IDBWorkerAuth.UserAddAndUpdate(innerUser);

                    _IDBWorkerAuth.RemoveMobileIdData(infoRegister.MobileId);
                    innerUser.AddMobileId(infoRegister.MobileId);
                    _IDBWorkerAuth.UserAddAndUpdate(innerUser);

                    response.Uuid = innerUser.UUID;
                    response.Name = innerUser.Name;
                    response.Phone = innerUser.Phone;
                    response.AuthKey = Guid.NewGuid().ToString();
                }
                else
                {
                    throw new Exception("Registration to ELMA failed.");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV1.Register", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            return response;
        }

        /// <summary>
        /// Запрос авторизации пользователя.
        /// </summary>
        ///
        /// <remarks>
        /// Авторизует пользователя, если авторизация идет на новом устройстве, то добавляет это устройство в список получающих пуши, так же обновляет авторизационный ключ.
        /// - Login - логин авторизации в МП
        /// - Password - пароль авторизации в МП
        /// - MobileId - идентификатор установки(идентифкатор МП) используется для пушей
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер в ответ возвращает UUID пользователя и авторизационный ключ:
        /// - userUUID - UUID пользователя в наумен
        /// - name - имя пользователя в наумен
        /// - phone - телефонный номер пользователя в наумен
        /// - authKey - авторизационный ключ, для использования при создании\редактировании задач.
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/AuthMobile")]
        [HttpPost]
        public OuterModels.InfoAuth.Response AuthMobile(OuterModels.InfoAuth.Request authInfo)
        {
            Program.Logger.Log("MobileControllersV2.Auth", $"Received an authorization request. Login: {authInfo.Login}, Password: {authInfo.Password}");
            OuterModels.InfoAuth.Response response = new OuterModels.InfoAuth.Response();
            try
            {
                InnerModels.InnerUser innerUser = _IDBWorkerAuth.UserGet(authInfo.Login.ToString(), authInfo.Password);
                if (innerUser.UUID == null) throw new Exception("The user with the following username\\password was not found.");
                if (ELMAExchange.TryAuthUser(ref innerUser))
                {
                    _IDBWorkerAuth.RemoveMobileIdData(authInfo.MobileId);
                    innerUser.AddMobileId(authInfo.MobileId);
                    _IDBWorkerAuth.UserAddAndUpdate(innerUser);

                    response.Uuid = innerUser.UUID;
                    response.Name = innerUser.Name;
                    response.Phone = innerUser.Phone;
                    response.AuthKey = Guid.NewGuid().ToString();
                }
                else
                {
                    throw new Support.WarnException("Authorization to ELMA failed.");
                }
                if (innerUser.UUID == null) throw new Support.WarnException("Authorization to failed.");
            }
            catch (Support.WarnException ex)
            {
                Program.Logger.Log("MobileControllersV2.Auth", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace);
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV2.Auth", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            return response;
        }

        /// <summary>
        /// Запрос дял разлогина пользователя.
        /// </summary>
        ///
        /// <remarks>
        /// Используется при разлогине пользователя. Фактически отвязываем мобильный идентификатор устройства, что бы на него не шли больше пуши.
        /// - MobileId - mobileid пользователя
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер в ответ возвращает идентификатор успешности запроса:
        /// - success - идентификатор успешности запроса
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/LogOutMobile")]
        [HttpPost]
        public OuterModels.InfoLogOut.Response LogOutMobile(OuterModels.InfoLogOut.Request authInfo)
        {
            OuterModels.InfoLogOut.Response response = new OuterModels.InfoLogOut.Response();
            try
            {
                _IDBWorkerAuth.RemoveMobileIdData(authInfo.MobileId);
                response.Success = true;
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV2.LogOut", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            return response;
        }

        /// <summary>
        /// Запрос для получения списка организаций по пользователю.
        /// </summary>
        ///
        /// <remarks>
        /// Используется для полученяи массива организация к которым привязан пользователь.
        /// - UserUUID - UUID пользователя в наумен
        /// </remarks>
        ///
        /// /// <response code="200">
        /// Сервер в ответ массив организаций для данного пользователя:
        /// - organisations - массив организаций, содержит:
        /// - uuid - UUID организации из наумен
        /// - name - наименование организации в наумен
        /// - partner - метка партнера, обозначает, что эта организация партнер и ее надо отображать в профиле, но на нее нельзя заводить заявки
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/GetOrgMobile")]
        [HttpPost]
        public OuterModels.InfoOrg.Response GetOrgMobile(OuterModels.InfoOrg.Request infoOrg)
        {
            OuterModels.InfoOrg.Response resp = new OuterModels.InfoOrg.Response();
            try
            {
                Program.Logger.Log("MobileControllersV2.Getorg", $"Received an GetOrg request. UserUUID: {infoOrg.UserUUID}");
                Program.Logger.Log("MobileControllersV2.Getorg", "Requesting organisations from nauman.");
                List < (string, string, bool)> listorg = ELMAExchange.GetUserOrg(infoOrg.UserUUID);
                if (listorg != null)
                {
                    foreach ((string, string, bool) orgR in listorg)
                    { 
                        Program.Logger.Log("MobileControllersV1.Getorg", $"Found a organisation with OrgUUID: {orgR.Item2}");
                        OuterModels.InfoOrg.Organisation org = new OuterModels.InfoOrg.Organisation()
                        {
                            Name = orgR.Item1,
                            Uuid = orgR.Item2,
                            Partner = orgR.Item3
                        };
                        resp.Organisations.Add(org);
                    }
                }
                return resp;
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV2.Getorg", $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}", "ERROR");
                resp.ErrorMessage = ex.Message;
                return resp;
            }
        }

        /// <summary>
        /// Запрос для редактирования данных пользователя.
        /// </summary>
        ///
        /// <remarks>
        /// Используется для редактирования данных пользователя.
        /// - userUUID - UUID пользователя в наумен
        /// - name - имя пользователя в наумен
        /// - phone - телефонный номер пользователя в наумен
        /// - login - новый логин для пользователя
        /// - password - новый пароль для пользователя
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер в ответ возвращает метку успешности изменения данных:
        /// - success - успешность изменения данных
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/ChangeUserData")]
        [HttpPost]
        public OuterModels.ChangeMobileUser.Response ChangeUserData(OuterModels.ChangeMobileUser.Request request)
        {
            Program.Logger.Log("MobileControllersV2.ChangeData", $"Received an change user data request. Uuid: {request.Uuid}");
            OuterModels.ChangeMobileUser.Response response = new OuterModels.ChangeMobileUser.Response();
            try
            {
                InnerModels.InnerUser user = _IDBWorkerAuth.UserGetById(request.Uuid);
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(request.Name)) user.Name = request.Name;
                    if (!string.IsNullOrEmpty(request.Phone)) user.Phone = request.Phone;
                    if (!string.IsNullOrEmpty(request.Login)) user.LoginMp = request.Login;
                    if (!string.IsNullOrEmpty(request.Password)) user.PasswordMp = request.Password;
                    if (request.GetEmailMessage != null) user.GetEmailMessage = request.GetEmailMessage;
                    if (request.GetSubMessage != null) user.GetSubMessage = request.GetSubMessage;
                    if (ELMAExchange.TryAddUpdateUser(ref user, true))
                    {
                        _IDBWorkerAuth.UserAddAndUpdate(user);
                        response.Success = true;
                    }
                    else
                    {
                        throw new Support.WarnException("ELMA user was not changed.");
                    }
                }
                else
                {
                    throw new Support.WarnException("The user with the following UUID was not found");
                }
            }
            catch (Support.WarnException ex)
            {
                Program.Logger.Log("MobileControllersV2.ChangeData", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace);
                response.ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV2.ChangeData", $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}", "ERROR");
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Запрос для запроса данных пользователя.
        /// </summary>
        ///
        /// <remarks>
        /// Возвращает данные пользователя.
        /// - MobilePhone - номер телефона пользователя
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер в ответ возвращает данны пользователя:
        /// - userUUID - UUID пользователя в наумен
        /// - name - имя пользователя в наумен
        /// - phone - телефонный номер пользователя в наумен
        /// - login - логин для входа в МП.
        /// - authKey - авторизационный ключ, для использования при создании\редактировании задач.
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/GetUserData")]
        [HttpPost]
        public OuterModels.GetMobileUser.Response GetUserData(OuterModels.GetMobileUser.Request request)
        {
            Program.Logger.Log("MobileControllersV2.GetData", $"Received an get user data request. Phone number : {request.MobilePhone}");
            OuterModels.GetMobileUser.Response response = new OuterModels.GetMobileUser.Response();
            try
            {
                InnerModels.InnerUser user = _IDBWorkerAuth.UserGetOnPhone(request.MobilePhone);
                if (user != null)
                {
                    response.Name = user.Name;
                    response.Login = user.LoginMp;
                    response.Phone = user.Phone;
                    response.Uuid = user.UUID;
                    response.AuthKey = Guid.NewGuid().ToString();
                    response.GetEmailMessage = user.GetEmailMessage.ToString();
                    response.GetSubMessage = user.GetSubMessage.ToString();
                }
                else
                {
                    response.ErrorMessage = "The user with the following Phone number was not found";
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV2.ChangeData", $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}", "ERROR");
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// Запрос для получения данных по контактным лицам, ответственным за партнера).
        /// </summary>
        ///
        /// <remarks>
        /// Возвращает данные по контактным лицам по ораганизации к которой относиться данный пользователь:
        /// - uuid - идентификатор пользователя в системе науман
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер в ответ возвращает массив контактных лиц:
        /// - contacts - массив контактных лиц
        /// - - name - имя пользователя в наумен
        /// - - phone - телефонный номер пользователя в наумен
        /// - - role - должность контактного лица
        /// - - image - адрес фото
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/GetContacts")]
        [HttpPost]
        public OuterModels.GetContactsData.Response GetContacts(OuterModels.GetContactsData.Request request)
        {
            Program.Logger.Log("MobileControllersV1.GetContacts", $"Received an get contacts data request. User uuid: {request.uuid}");
            OuterModels.GetContactsData.Response response = new OuterModels.GetContactsData.Response() { Contacts = new List<OuterModels.GetContactsData.Contact>() };
            try
            {
                List<(string name, string phone, string role)> contactsData = ELMAExchange.GetContacts(request.uuid);
                foreach (var innerContact in contactsData)
                {
                    OuterModels.GetContactsData.Contact outerContact = new OuterModels.GetContactsData.Contact()
                    {
                        role = innerContact.role,
                        name = innerContact.name,
                        phone = innerContact.phone
                    };
                    response.Contacts.Add(outerContact);
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV2.GetContacts", $"Error: {ex.Message}\nStackTrace: {ex.StackTrace}", "ERROR");
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
    }
}
