using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using InnerModels = TrueKeyServer.Models.Mobile.Inner;

namespace TrueKeyServer.DB
{
    public static class MobileInterfaces
    {
        public interface IDBWorkerTaskComment
        {
            #region Запись в БД
            void TaskAddAndUpdate(InnerModels.InnerTask outerTask);
            void CommentAddAndUpdate(InnerModels.InnerComment outerComment, bool selfComment = false);
            void FileAdd(List<string> files, string Uuid);
            string TaskOrComment(string id);
            #endregion

            #region Чтение из БД
            List<InnerModels.InnerTask> TaskGet(string orgId, string lastSync, string mobileid, bool nullRequest);
            List<InnerModels.InnerComment> CommentGet(string taskId, string lastSync, string mobileid, bool nullRequest);
            public List<InnerModels.Message> GetMessages(string timeLastSync, string uuid);
            #endregion
        }
        public interface IDBWorkerAuth
        {
            #region Запись в БД
            void UserAddAndUpdate(InnerModels.InnerUser outerUser);
            void RemoveMobileIdData(string mobileId);
            #endregion

            #region Чтение из БД
            InnerModels.InnerUser UserGet(string login, string password);
            InnerModels.InnerUser UserGet(string mobileId);
            InnerModels.InnerUser UserGetById(string Uuid);
            InnerModels.InnerUser UserGetOnPhone(string phone);
            #endregion
        }
    }
}
