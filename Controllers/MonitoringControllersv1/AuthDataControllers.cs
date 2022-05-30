using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TrueKeyServer.Controllers.MonitoringControllersv1
{
    /// <summary>
    /// Контроллер для работы с мониторингом.
    /// </summary>
    [ApiController]
    public class MonitoringAuthDataControllers : ControllerBase
    {
        private readonly DB.MonitoringInterfaces.IDBWorkerMonitoring _IDBWorkerMonitoring;
        public MonitoringAuthDataControllers(DB.MonitoringInterfaces.IDBWorkerMonitoring IDBWorkerMonitoring)
        {
            _IDBWorkerMonitoring = IDBWorkerMonitoring;
        }

        /// <summary>
        /// Запрос на получения логиновэ\паролей.
        /// </summary>
        ///
        /// <remarks>
        /// Возвращает список стандартных пар лоигно\пароль для айко офиса.
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер возвращает массив пар логин\пароль.
        /// - login - логин
        /// - password - пароль
        /// </response>
        [Route("monitoring/apiv1/GetKey")]
        [HttpGet]
        public List<Models.Monitoring.OuterKey.Request> GetKey()
        {
            List<Models.Monitoring.OuterKey.Request> keys = new List<Models.Monitoring.OuterKey.Request>();
            try
            {
                foreach (Models.Monitoring.Innerkey key in _IDBWorkerMonitoring.GetKeys())
                {
                    keys.Add(new Models.Monitoring.OuterKey.Request()
                    {
                        Login = key.login,
                        Password = key.password
                    });
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("AuthData.Get", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
            }
            return keys;
        }

        /// <summary>
        /// Запрос для сохранения новой пары логин\пароль.
        /// </summary>
        ///
        /// <remarks>
        /// Записывает в БД новую пару логин\пароль длчя последующего использования мониторингами при авторизации в айко офисе.
        /// - login - логин
        /// - password - пароль
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер возвращает метку успешности записи.
        /// - success - успешность сохранения пары логин\пароль
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("monitoring/apiv1/PostKey")]
        [HttpPost]
        public Models.Monitoring.OuterKey.Response PostKey(Models.Monitoring.OuterKey.Request outerKey)
        {
            Models.Monitoring.OuterKey.Response response = new Models.Monitoring.OuterKey.Response();
            try
            {
                _IDBWorkerMonitoring.AddKey(new Models.Monitoring.Innerkey()
                {
                    login = outerKey.Login,
                    password = outerKey.Password
                });
                response.Success = true;
            }
            catch (Exception ex)
            {
                Program.Logger.Log("AuthData.Post", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.Success = false;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
    }
}
