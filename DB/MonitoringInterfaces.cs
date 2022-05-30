using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MonitoringModels = TrueKeyServer.Models.Monitoring;

namespace TrueKeyServer.DB
{
    public static class MonitoringInterfaces
    {
        public interface IDBWorkerMonitoring
        {
            #region Запись в БД
            void AddKey(MonitoringModels.Innerkey innerkey);
            #endregion

            #region Чтение из БД
            List<MonitoringModels.Innerkey> GetKeys();
            #endregion
        }
        public interface IDBWorkerMonitoringFR
        {
            #region Запись в БД
            void AddFRData(List<TrueKeyServer.Models.Monitoring.InnerFRData> listFRData);
            #endregion

            #region Чтение из БД
            List<TrueKeyServer.Models.Monitoring.InnerFRData> GetFRData();
            #endregion
        }
    }
}
