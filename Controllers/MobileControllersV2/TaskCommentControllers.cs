using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

using NaumenExchange = TrueKeyServer.Protocols.NaumenExchange;
using ELMAExchange = TrueKeyServer.Protocols.ELMAExchange;
using ELMAModels = TrueKeyServer.Models.Mobile.ELMA;
using InnerModels = TrueKeyServer.Models.Mobile.Inner;
using OuterModels = TrueKeyServer.Models.Mobile.Outer;

namespace TrueKeyServer.Controllers.MobileControllersV2
{
    /// <summary>
    /// Контроллер для создания и изменения задач и комментариев.
    /// </summary>
    [ApiController]
    public class MobileTaskCommentControllers : ControllerBase
    {
        private readonly DB.MobileInterfaces.IDBWorkerTaskComment _DBWorkerTaskComment;
        private readonly DB.MobileInterfaces.IDBWorkerAuth _IDBWorkerAuth;
        private readonly Support.TaskQueue _TaskQueue;
        public MobileTaskCommentControllers(DB.MobileInterfaces.IDBWorkerTaskComment DBWorkerTaskComment, DB.MobileInterfaces.IDBWorkerAuth IDBWorkerAuth, Support.TaskQueue TaskQueue)
        {
            _DBWorkerTaskComment = DBWorkerTaskComment;
            _TaskQueue = TaskQueue;
            _IDBWorkerAuth = IDBWorkerAuth;
        }

        /// <summary>
        /// Запрос для создания или изменения заявки.
        /// </summary>
        ///
        /// <remarks>
        /// Создает новую заявку из полученных данных, если таковая заявка уже имеется, то вносит в нее изменения.
        /// - taskId - идентификатор заявки (только для редактирования)
        /// - orgUUID - UUID организации из наумен на которую открыта заявка
        /// - title - заголовок заявки (категория заявки)
        /// - description - описание заявки
        /// - mobileId  - мобильный идентификатор с которого создаетя\меняется заявка
        /// - authkey  - авторизационный ключ для наумана
        /// - status - статус заявки, один из массива:
        /// - - New - новая заявка
        /// - - InProgress - в работе
        /// - - WaitForResponse - ожидает ответа
        /// - - Resolved - выполнена
        /// - - Closed - закрыта
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер в ответ возвращает данные по заявке в наумане и описание ошибки, если она возникает.
        /// - uuid - идентифкатор заявки в наумане
        /// - number  - номер заявки
        /// - dateCreate  - дата и время создания заявки
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/AddTaskMobile")]
        [HttpPost]
        public OuterModels.MobileTask.Response AddTaskMobile(OuterModels.MobileTask.Request outerTask)
        {
            Program.Logger.Log("MobileControllersV2.AddTaskMobile", $"Request to add a task was received from MP with Mobileid: {outerTask.MobileId}");
            OuterModels.MobileTask.Response response = new OuterModels.MobileTask.Response();
            try
            {
                InnerModels.InnerTask innerTask = new InnerModels.InnerTask(outerTask)
                {
                    Author = _IDBWorkerAuth.UserGet(outerTask.MobileId)?.UUID
                };
                if (ELMAExchange.TryAddTask(ref innerTask))
                {
                    response.Uuid = innerTask.TaskId;
                    response.Number = innerTask.Number;
                    response.DateCreate = innerTask.DateCreate;
                    //_DBWorkerTaskComment.TaskAddAndUpdate(innerTask);
                    _TaskQueue.AddNewWork(innerTask);
                }
                else
                {
                    Program.Logger.Log("MobileControllersV2.AddTaskMobile", "Task was not successfully added\\changed in Naumen.");
                    response.ErrorMessage = "Error.Task not added to Naumen.";
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV2.AddTaskMobile", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            return response;
        }

        /// <summary>
        /// Запрос для добавления комментария.
        /// </summary>
        ///
        /// <remarks>
        /// Создает новый комментарий и прикрепляет его к заявке.
        /// - taskId - идентификатор заявки
        /// - message - текст комментария
        /// - mobileId - идентифкатор мобильного приложения
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер в ответ возвращает данные по заявке в наумане и описание ошибки, если она возникает.
        /// - uuid - идентифкатор заявки в наумане
        /// - number  - нмоер заявки
        /// - dateCreate  - дата и время создания заявки
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/AddCommentMobile")]
        [HttpPost]
        public OuterModels.MobileComment.Response AddCommentMobile(OuterModels.MobileComment.Request outerComment)
        {
            Program.Logger.Log("MobileControllersV1.AddCommentMobile", $"Request to add a task was received from MP with Mobileid: {outerComment.MobileId}");
            OuterModels.MobileComment.Response response = new OuterModels.MobileComment.Response();
            try
            {
                InnerModels.InnerUser user = _IDBWorkerAuth.UserGet(outerComment.MobileId);
                InnerModels.InnerComment innerComment = new InnerModels.InnerComment(outerComment)
                {
                    UserUUID = user.UUID,
                    Name = user.Name
                };
                if (ELMAExchange.TryAddComment(ref innerComment))
                {
                    response.Uuid = innerComment.CommentId;
                    response.DateCreate = innerComment.DateCreate;
                    _DBWorkerTaskComment.CommentAddAndUpdate(innerComment, true);
                }
                else
                {
                    Program.Logger.Log("MobileControllersV1.AddCommentMobile", "Comment was not successfully added\\changed in Naumen.");
                    response.ErrorMessage = "Error.Comment not added to Naumen.";
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV1.PostComment", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            return response;
        }

        /// <summary>
        /// Запрос для добавления файлов к заявке\комментарию.
        /// </summary>
        ///
        /// <remarks>
        /// <para>Добавляет файл к указанной сущности. Запрос должен содержать полезную нагрузку типа form-data.</para>
        /// <para>
        ///     {
        ///     "uuid" - "string"
        ///     "file1" - file
        ///     "file2" - file
        ///     ...
        ///     "fileN" - file
        ///     }
        /// </para>
        /// <para>
        /// - uuid - идентификатор сущности к которой прикрепляются файлы (комментарий или заявка)
        /// - file1..fileN - сами файлы
        /// </para>
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер в ответ возвращает метку успешности изменения данных:
        /// - success - успешность изменения данных
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/AttachFiles")]
        [HttpPost]
        public OuterModels.ResponseAttachFile AttachFiles()
        {
            OuterModels.ResponseAttachFile response = new OuterModels.ResponseAttachFile();
            try
            {
                List<string> filesUrl = new List<string>();
                bool isComment = false;
                Request.Form.TryGetValue("uuid", out var uuid);
                Program.Logger.Log("MobileControllersV1.AttachFiles", $"Request to add a files to subject with UUID: {uuid}");
                switch (_DBWorkerTaskComment.TaskOrComment(uuid))
                {
                    case "task":
                        isComment = false;
                        break;
                    case "comment":
                        isComment = true;
                        break;
                    case "void":
                        throw new Exception($"No entityes with this uuid: {uuid}");
                }
                filesUrl = ELMAExchange.AddFiles(uuid.ToString(), Request.Form.Files, isComment);
                _DBWorkerTaskComment.FileAdd(filesUrl, uuid);
                response.Success = true;
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV1.PostFiles", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            return response;
        }

        /// <summary>
        /// Запрос для получения ссылки на файл из ЕЛМЫ.
        /// </summary>
        ///
        /// <remarks>
        /// Запрашивает ссылки по указанным айди файлов из ЕЛМЫ
        /// - FileIds - массив идентификаторов файлов 
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер в ответ возвращает массив сопоставлений, типа ид файла - его ссылка.
        /// - fileLinks - массив сопоставлений
        /// - - id  - идентификатор файла
        /// - - link - ссылка на загрузку
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/GetFilesLinks")]
        [HttpPost]
        public OuterModels.GetFileLink.Response GetFilesLinks(OuterModels.GetFileLink.Request outerFiles)
        {
            OuterModels.GetFileLink.Response response = new OuterModels.GetFileLink.Response() { FileLinks = new List<OuterModels.GetFileLink.FileLink>() };
            try
            {
                foreach (string id in outerFiles.FileIds)
                {
                    string link = ELMAExchange.GetFileLink(id);
                    if (!string.IsNullOrEmpty(link)) response.FileLinks.Add(new OuterModels.GetFileLink.FileLink() { id = id, link = link});
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV1.PostComment", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            return response;
        }

        /// <summary>
        /// Запрос на получение списка категорий для заявок.
        /// </summary>
        ///
        /// <remarks>
        /// Запрос для получения актуального списка категорияй заявок.
        /// </remarks>
        ///
        /// <response code="200">
        /// Возвращает массив категорий или текст ошибки.
        /// - categories - массив категорий
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/GetCategories")]
        [HttpGet]
        public OuterModels.InfoCategories.Response GetCategories()
        {
            OuterModels.InfoCategories.Response response = new OuterModels.InfoCategories.Response();
            try
            {
                InnerModels.InnerCategories innerCategories = ELMAExchange.GetCategories();
                if (innerCategories != null) response.Categories = innerCategories.Categories;
                else response.ErrorMessage = "categories not found";
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV1.GetCategories", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            return response;
        }

        /// <summary>
        /// Запрос для запроса звонка.
        /// </summary>
        ///
        /// <remarks>
        /// Создает запрос на обратный звонок в сд.
        /// - ModileId - мобильный идентификатор пользователя
        /// </remarks>
        ///
        /// <response code="200">
        /// Сервер в ответ возвращает метку успешности изменения данных:
        /// - success - успешность изменения данных
        /// - errorMessage - строка с ошибкой, если таковая возникнет
        /// </response>
        [Route("mobile/apiv1/CreateMissedCall")]
        [HttpPost]
        public OuterModels.CreateMissedCall.Response CreateMissedCall(OuterModels.CreateMissedCall.Request request)
        {
            OuterModels.CreateMissedCall.Response response = new OuterModels.CreateMissedCall.Response();
            try
            {
                InnerModels.InnerUser user = _IDBWorkerAuth.UserGet(request.MobileId);
                if (user != null) response.Success = NaumenExchange.TryCreateMissedCall(user.Phone);
                else response.ErrorMessage = "user with this mobileId nor found in DB";
            }
            catch (Exception ex)
            {
                Program.Logger.Log("MobileControllersV1.PostMissedCall", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
                response.ErrorMessage = "Error message: " + ex.Message + '\n' + "StackTrcae: " + ex.StackTrace;
            }
            return response;
        }
    }
}
