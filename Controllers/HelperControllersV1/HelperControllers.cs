using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using NaumenExchange = TrueKeyServer.Protocols.NaumenExchange;

namespace TrueKeyServer.Controllers.HelperControllersV1
{
    /// <summary>
    /// Контроллер для хелпера.
    /// </summary>
    [ApiController]
    public class HelperControllers : ControllerBase
    {
        /// <summary>
        /// Запрос для получения идентификатора организации.
        /// </summary>
        /// 
        /// <remarks>
        /// Возвращает идентифи катор организации в наумане (UUID) по идентификатору организации из 1С.
        /// - orgId - идентификатор организации из 1С
        /// </remarks>
        /// 
        /// <response code="200">
        /// Возвращает идентификатор орагнизации, если таковая есть в наумане.
        /// - uuid - UUID организации из наумана
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("helper/apiv1/GetUUIDByOrgId")]
        [HttpPost]
        public Models.Helper.InfoOrg.Response GetUUIDByOrgId(Models.Helper.InfoOrg.Request orgInfo)
        {
            Models.Helper.InfoOrg.Response response = new Models.Helper.InfoOrg.Response();
            try
            {
                response.Uuid = NaumenExchange.GetUuidByOrgId(orgInfo.OrgId).Split('$')[1];
                if (response.Uuid == null) response.ErrorMessage = "не найдено организации с таким OrgId";
            }
            catch (Exception ex)
            {
                Program.Logger.Log("NaumenExchange.GetUser", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
    }
}
