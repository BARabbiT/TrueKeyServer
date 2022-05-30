using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace TrueKeyServer.Controllers.MonitoringControllersv1
{
    [ApiController]
    public class MonitoringFRDataController : ControllerBase
    {
        private readonly DB.MonitoringInterfaces.IDBWorkerMonitoringFR _IDBWorkerMonitoringFR;
        public MonitoringFRDataController(DB.MonitoringInterfaces.IDBWorkerMonitoringFR IDBWorkerMonitoringFR)
        {
            _IDBWorkerMonitoringFR = IDBWorkerMonitoringFR;
        }

        /// <summary>
        /// Запрос для сохранения данных по ФРам.
        /// </summary>
        ///
        /// <remarks>
        /// Записывает в БД данные по ФРам.
        /// - frDatas - массив данных по ФРам
        /// - - Owner - 1С guid компании-владельца ФР
        /// - - Inn - ИНН
        /// - - AdressPlaceOfS - Адрес места установки ФР
        /// - - NDS - НДС
        /// - - Model - Модель ФР
        /// - - FirmwareID - Версия прошивки
        /// - - SerialNumber - Серийниый номер
        /// - - RegisterNumber - Регистрационный номер
        /// - - NomberFN - Заводской номер
        /// - - VersConfigur - Версия конфигуратора
        /// - - VersBoot - Версия загрузчика
        /// - - VersionFn - Версия ФН
        /// - - OperatorFD - Оператор фискальных данных
        /// - - RegFnCount - Количество оставшихся регистраций
        /// - - AmountFnRereg - Количество проведенных регистраций
        /// - - CheckResource - Ресурс ФН
        /// - - FFD - Версия ФФД
        /// - - EndDataFN - Дата окончаня ФН
        /// - - FnExpireDays - Количество дней до окончания ФН
        /// - - QueueDocOFD - Количество документов не отправленных в ОФД
        /// - - NumFirstUnDoc - Номер первого не отправленного документа
        /// - - DateFirstUnDoc - Дата первого не отправленного документа
        /// - - StateInfoEx - Статус обмена с ОФД
        /// - - LastRegDateFN - Дата последней регистрации
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер возвращает метку успешности записи.
        /// - success - успешность сохранения пары логин\пароль
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("monitoring/apiv1/SetFRData")]
        [HttpPost]
        public Models.Monitoring.SetFRData.Response SetFRData(Models.Monitoring.SetFRData.Request request)
        {
            Models.Monitoring.SetFRData.Response response = new Models.Monitoring.SetFRData.Response();
            try
            {
                List<Models.Monitoring.InnerFRData> InnerFRData = new List<Models.Monitoring.InnerFRData>();
                foreach (Models.Monitoring.FRData OuterFRData in request.FRDatas)
                {
                    InnerFRData.Add(new Models.Monitoring.InnerFRData
                    {
                        Owner = OuterFRData.Owner,
                        Inn = OuterFRData.Inn,
                        AdressPlaceOfS = OuterFRData.AdressPlaceOfS,
                        NDS = OuterFRData.NDS,
                        Model = OuterFRData.Model,
                        FirmwareID = OuterFRData.FirmwareID,
                        SerialNumber = OuterFRData.SerialNumber,
                        RegisterNumber = OuterFRData.RegisterNumber,
                        NomberFN = OuterFRData.NomberFN,
                        VersConfigur = OuterFRData.VersConfigur,
                        VersBoot = OuterFRData.VersBoot,
                        VersionFn = OuterFRData.VersionFn,
                        OperatorFD = OuterFRData.OperatorFD,
                        RegFnCount = OuterFRData.RegFnCount,
                        AmountFnRereg = OuterFRData.AmountFnRereg,
                        CheckResource = OuterFRData.CheckResource,
                        FFD = OuterFRData.FFD,
                        EndDataFN = OuterFRData.EndDataFN,
                        FnExpireDays = OuterFRData.FnExpireDays,
                        QueueDocOFD = OuterFRData.QueueDocOFD,
                        NumFirstUnDoc = OuterFRData.NumFirstUnDoc,
                        DateFirstUnDoc = OuterFRData.DateFirstUnDoc,
                        StateInfoEx = OuterFRData.StateInfoEx,
                        LastModifiedDate = OuterFRData.LastModifiedDate,
                        LastRegDateFN = OuterFRData.LastRegDateFN
                    });
                }
                //response.Success = Protocols.NaumenExchange.SetFRData(InnerFRData);
                _IDBWorkerMonitoringFR.AddFRData(InnerFRData);
            }
            catch (Exception ex)
            {
                Program.Logger.Log("FRDataController.SetFRData", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
            }
            return response;
        }

        /// <summary>
        /// Запрос для получения данных по ФРам.
        /// </summary>
        ///
        /// <remarks>
        /// Запрашивает массив данных по ФР.
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер возвращает массив данных по ФРам.
        /// - frDatas - массив данных по ФРам
        /// - - Owner - 1С guid компании-владельца ФР
        /// - - Inn - ИНН
        /// - - AdressPlaceOfS - Адрес места установки ФР
        /// - - NDS - НДС
        /// - - Model - Модель ФР
        /// - - FirmwareID - Версия прошивки
        /// - - SerialNumber - Серийниый номер
        /// - - RegisterNumber - Регистрационный номер
        /// - - NomberFN - Заводской номер
        /// - - VersConfigur - Версия конфигуратора
        /// - - VersBoot - Версия загрузчика
        /// - - VersionFn - Версия ФН
        /// - - OperatorFD - Оператор фискальных данных
        /// - - RegFnCount - Количество оставшихся регистраций
        /// - - AmountFnRereg - Количество проведенных регистраций
        /// - - CheckResource - Ресурс ФН
        /// - - FFD - Версия ФФД
        /// - - EndDataFN - Дата окончаня ФН
        /// - - FnExpireDays - Количество дней до окончания ФН
        /// - - QueueDocOFD - Количество документов не отправленных в ОФД
        /// - - NumFirstUnDoc - Номер первого не отправленного документа
        /// - - DateFirstUnDoc - Дата первого не отправленного документа
        /// - - StateInfoEx - Статус обмена с ОФД
        /// - - LastRegDateFN - Дата последней регистрации
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("monitoring/apiv1/GetFRData")]
        [HttpGet]
        public Models.Monitoring.GetFRData.Response GetFRData()
        {
            Models.Monitoring.GetFRData.Response response = new Models.Monitoring.GetFRData.Response() { FRDatas = new List<Models.Monitoring.FRData>() };
            try
            {
                foreach (Models.Monitoring.InnerFRData FRData in _IDBWorkerMonitoringFR.GetFRData())
                {
                    response.FRDatas.Add(new Models.Monitoring.FRData
                    {
                        Owner = FRData.Owner,
                        Inn = FRData.Inn,
                        AdressPlaceOfS = FRData.AdressPlaceOfS,
                        NDS = FRData.NDS,
                        Model = FRData.Model,
                        FirmwareID = FRData.FirmwareID,
                        SerialNumber = FRData.SerialNumber,
                        RegisterNumber = FRData.RegisterNumber,
                        NomberFN = FRData.NomberFN,
                        VersConfigur = FRData.VersConfigur,
                        VersBoot = FRData.VersBoot,
                        VersionFn = FRData.VersionFn,
                        OperatorFD = FRData.OperatorFD,
                        RegFnCount = FRData.RegFnCount,
                        AmountFnRereg = FRData.AmountFnRereg,
                        CheckResource = FRData.CheckResource,
                        FFD = FRData.FFD,
                        EndDataFN = FRData.EndDataFN,
                        FnExpireDays = FRData.FnExpireDays,
                        QueueDocOFD = FRData.QueueDocOFD,
                        NumFirstUnDoc = FRData.NumFirstUnDoc,
                        DateFirstUnDoc = FRData.DateFirstUnDoc,
                        StateInfoEx = FRData.StateInfoEx,
                        LastModifiedDate = FRData.LastModifiedDate,
                        LastRegDateFN = FRData.LastRegDateFN
                    });
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("FRDataController.GetFRData", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = ex.Message;
            }
            return response;
        }
    }
}
