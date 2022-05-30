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

namespace TrueKeyServer.Controllers.ELMAComtrollersV1
{
    [ApiController]
    public class TaskCommentControllers : Controller
    {
        private readonly DB.MobileInterfaces.IDBWorkerTaskComment _DBWorkerTaskComment;
        private readonly DB.MobileInterfaces.IDBWorkerAuth _IDBWorkerAuth;
        private readonly Support.TaskQueue _TaskQueue;
        public TaskCommentControllers(DB.MobileInterfaces.IDBWorkerTaskComment DBWorkerTaskComment, DB.MobileInterfaces.IDBWorkerAuth IDBWorkerAuth, Support.TaskQueue TaskQueue)
        {
            _DBWorkerTaskComment = DBWorkerTaskComment;
            _TaskQueue = TaskQueue;
            _IDBWorkerAuth = IDBWorkerAuth;
        }

        /// <summary>
        /// Запрос для создания или изменения задачи.
        /// </summary>
        ///
        /// <remarks>
        /// Создает новую задачу из полученных данных, если таковая задача уже имеется, то вносит в нее изменения.
        /// - taskId - идентификатор заявки (только для редактирования)
        /// - number - номер заявки
        /// - orgUUID - UUID организации из наумен на которую открыта заявка
        /// - dateCreate - дата и время создания заявки
        /// - title - заголовок задачи (категория заявки)
        /// - description - описание заявки
        /// - imageSource - массив ссылок на прикрепленные файлы к заявке
        /// - status - статус заявки, один из массив:
        /// - - New - новая заявка
        /// - - InProgress - в работе
        /// - - WaitForResponse - ожидает ответа
        /// - - Resolved - выполнена
        /// - - Closed - закрыта
        /// - author - UUID автора заявки
        /// - subscribers - строка с UUID всех подписчиков на заявку, разделенных символом ';'
        /// </remarks>
        ///
        /// <response code="200">
        /// Возвращает SUCESS, при успешном создании.
        /// </response>
        [Route("ELMA/v1/AddTask")]
        [HttpPost]
        public void AddTask(OuterModels.ELMATask.Request outerTask)
        {
            try
            {
                Program.Logger.Log("ELMAControllerV1.AddTask", $"Received an addtask request from ELMA. TaskID: {outerTask.TaskId}, TaskNumber: {outerTask.Number}");
                InnerModels.InnerTask innerTask = new InnerModels.InnerTask(outerTask);
                _TaskQueue.AddNewWork(innerTask);
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAControllerV1.AddTask", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
            }
        }

        /// <summary>
        /// Запрос для добавления комментария.
        /// </summary>
        ///
        /// <remarks>
        /// Создает новый комментарий и пркрепляет его к заявке.
        /// - taskId - идентификатор заявки
        /// - commentId - идентификатор комментария
        /// - dateCreate - дата и время создания комментария
        /// - author - UUID автора заявки
        /// - name - имя автора заявки
        /// - message - текст комментария
        /// </remarks>
        ///
        /// <response code="200">
        /// Возвращает SUCESS, при успешном создании.
        /// </response>
        [Route("ELMA/v1/AddComment")]
        [HttpPost]
        public void AddComment(OuterModels.ELMAComment.Request outerComment)
        {
            try
            {
                Program.Logger.Log("ELMAControllerV1.AddComment", $"Received an addcomment request from ELMA. CommentID: {outerComment.CommentId}");
                InnerModels.InnerComment innerComment = new InnerModels.InnerComment(outerComment);
                _DBWorkerTaskComment.CommentAddAndUpdate(innerComment);
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAControllerV1.AddComment", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
            }
        }

        /// <summary>
        /// Запрос для добавления файлов к заявке\комментарию.
        /// </summary>
        ///
        /// <remarks>
        /// Добавляет массив ссылок на файлы в комментаий\заявку.
        /// - uuid - идентификатор сущности к которой прикреплять файлы
        /// - imageSource - массив ссылок на файлы для прикрепления
        /// </remarks>
        ///
        /// <response code="200">
        /// Возвращает SUCESS, при успешном выполнении.
        /// </response>
        [Route("ELMA/v1/AddFiles")]
        [HttpPost]
        public void AddFiles(OuterModels.ELMAFilesAdd.Request files)
        {
            try
            {
                Program.Logger.Log("ELMAControllerV1.AddFiles", $"Received an addfiles request from ELMA. SubjectUUID: {files.Uuid}");
                _DBWorkerTaskComment.FileAdd(files.ImageSource, files.Uuid);
            }
            catch (Exception ex)
            {
                Program.Logger.Log("ELMAControllerV1.AddFiles", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
            }
        }
    }
}
