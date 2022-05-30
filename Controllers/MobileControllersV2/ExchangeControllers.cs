using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using InnerModels = TrueKeyServer.Models.Mobile.Inner;
using OuterModels = TrueKeyServer.Models.Mobile.Outer;

namespace TrueKeyServer.Controllers.MobileControllersV2
{
    /// <summary>
    /// Контроллер для работы с получением задач\комментариев.
    /// </summary>
    [ApiController]
    public class MobileExchangeControllers : ControllerBase
    {
        private readonly DB.MobileInterfaces.IDBWorkerTaskComment _DBWorkerTaskComment;
        public MobileExchangeControllers(DB.MobileInterfaces.IDBWorkerTaskComment DBWorkerTaskComment)
        {
            _DBWorkerTaskComment = DBWorkerTaskComment;
        }

        /// <summary>
        /// Запрос задач и коментариев для мобилки.
        /// </summary>
        /// 
        /// <remarks>
        /// Возвращает новые данные в ответ на запрос от МП. Если присылается 0 в поле LastSync, то присылает все задачи не зависимо от даты изменений в них.
        /// - orgUUID - массив идентификаторов организаций в наумен по которым будут возвращаться задачи
        /// - lastSync - время последней успешной синхронизации(время в UNIX формате)
        /// - mobileId - идентификатор мобильного приложения
        /// </remarks>
        /// 
        /// <response code="200">
        /// Сервер возвращает массив задач и комментариев или ошибку, если таковая произойдет.
        /// - taskId - идентификатор задачи
        /// - number - номер задачи
        /// - orgUUID - идетификатор орагнизации
        /// - title - заголовок задачи(предположительно категория)
        /// - description - описание задачи
        /// - dateCreate - дата создания задачи
        /// - imageSource - массив прикрепленных файлов(содержит url адреса для загрузки файлов)
        /// - status - статус заявки, берется из массива:
        /// - - NotSailed - не доставлена до наумана
        /// - - New - доставлена до наумана
        /// - - InProgress - в работе
        /// - - WaitForResponse - ожидает ответа
        /// - - Resolved - выполнена
        /// - - Closed - закрыта
        /// - comments - массив комментариев:
        ///  - - taskId - идентификатор заявки
        ///  - - userUUID - автор комментария
        ///  - - name - имя автора комментария
        ///  - - commentId - идентификатор комментария в наумане
        ///  - - dateCreate - дата\время создания комментария
        ///  - - message - текст комментария
        ///  - - imageSource - массив прикрепленных файлов(содержит url адреса для загрузки файлов)
        /// </response>
        [Route("mobile/apiv1/GetExchangeTaskMobile")]
        [HttpPost]
        public OuterModels.InfoExchange.Response GetExchangeTaskMobile(OuterModels.InfoExchange.Request exchInfo)
        {
            Program.Logger.Log("MobileExchangeControllers.GetExchangeTaskMobile", $"Received an GetExchangeTaskMobile request. Mobileid: {exchInfo.Mobileid}, LastSync: {exchInfo.LastSync}");
            OuterModels.InfoExchange.Response response = new OuterModels.InfoExchange.Response();
            try
            {
                foreach (string orgUUid in exchInfo.OrgUUID)
                {
                    foreach (InnerModels.InnerTask innerTask in _DBWorkerTaskComment.TaskGet(orgUUid, exchInfo.LastSync, exchInfo.Mobileid, exchInfo.LastSync == "0"))
                    {
                        OuterModels.InfoExchange.Task outerTask = new OuterModels.InfoExchange.Task()
                        {
                            TaskId = innerTask.TaskId,
                            OrgUUID = innerTask.OrgUUID,
                            Number = innerTask.Number,
                            DateCreate = innerTask.DateCreate,
                            Title = innerTask.Title,
                            Description = innerTask.Description,
                            Status = innerTask.Status,
                            ImageSource = innerTask.ImageSource
                        };
                        foreach (InnerModels.InnerComment innerComment in _DBWorkerTaskComment.CommentGet(innerTask.TaskId, exchInfo.LastSync, exchInfo.Mobileid, exchInfo.LastSync == "0").ToList())
                        {
                            outerTask.Comments.Add(new OuterModels.InfoExchange.Comment()
                            {
                                CommentId = innerComment.CommentId,
                                DateCreate = innerComment.DateCreate,
                                TaskId = innerComment.TaskId,
                                Message = innerComment.Message,
                                UserUUID = innerComment.UserUUID,
                                ImageSource = innerComment.ImageSource,
                                Name = innerComment.Name
                            });
                        }
                        response.Tasks.Add(outerTask);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV1.GetChange", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            return response;
        }

        /// <summary>
        /// Запрос комментариев по задаче.
        /// </summary>
        /// 
        /// <remarks>
        /// Сервер вернет список комментариев по ид заявки, измененых или добавленых после времени последней синхронизации. Если присылается 0 в поле LastSync, то присылает все комментарии не зависимо от даты изменений в них.
        /// - taskId - идентификатор заявки
        /// - lastSync - время последней успешной синхронизации (время в UNIX формате)
        /// - mobileId - идентификатор мобильного приложения
        /// </remarks>
        /// 
        /// <response code="200">
        /// Сервер в ответ возвращает массив комментариев или описание ошибки:
        /// - comments - массив комментариев:
        /// - taskId - идентификатор заявки
        /// - userUUID - автор комментария
        /// - name - имя автора комментария
        /// - commentId - идентификатор комментария в наумане
        /// - dateCreate - дата\время создания комментария
        /// - message - текст комментария
        /// - imageSource - массив прикрепленных файлов(содержит url адреса для загрузки файлов)
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/GetCommentsMobile")]
        [HttpPost]
        public OuterModels.InfoComment.Response GetCommentsMobile(OuterModels.InfoComment.Request commentInfo)
        {
            OuterModels.InfoComment.Response response = new OuterModels.InfoComment.Response();
            try
            {
                foreach (InnerModels.InnerComment innerComment in _DBWorkerTaskComment.CommentGet(commentInfo.TaskId, commentInfo.LastSync, commentInfo.MobileId, commentInfo.LastSync == "0").ToList())
                {
                    response.Comments.Add(new OuterModels.InfoComment.Comment()
                    {
                        CommentId = innerComment.CommentId,
                        DateCreate = innerComment.DateCreate,
                        TaskId = innerComment.TaskId,
                        Message = innerComment.Message,
                        UserUUID = innerComment.UserUUID,
                        ImageSource = innerComment.ImageSource,
                        Name = innerComment.Name
                    });
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV1.GetComments", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            return response;
        }

        [Route("mobile/apiv1/GetNotifications")]
        [HttpPost]
        public OuterModels.GetNotificationData.Response GetNotifications(OuterModels.GetNotificationData.Request request)
        {
            OuterModels.GetNotificationData.Response response = new OuterModels.GetNotificationData.Response() { Notifications = new List<OuterModels.GetNotificationData.Message>()};
            try
            {
                foreach(InnerModels.Message innerMessage in _DBWorkerTaskComment.GetMessages(request.timeLastSync, request.mobileId))
                {
                    response.Notifications.Add(new OuterModels.GetNotificationData.Message()
                    {
                        dateCreate = innerMessage.dateCreate,
                        type = innerMessage.type,
                        msg = innerMessage.msg,
                        title = innerMessage.title,
                        linkToObj = innerMessage.linkToObj,
                        imageSource = innerMessage.imageSource
                    });
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV1.GetMessages", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            return response;
        }
    }
}
