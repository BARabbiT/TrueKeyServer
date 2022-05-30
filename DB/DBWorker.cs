using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using DBModels = TrueKeyServer.DB.Models;
using InnerModels = TrueKeyServer.Models.Mobile.Inner;
using MonitoringModels = TrueKeyServer.Models.Monitoring;

namespace TrueKeyServer.DB
{
    public class DBWorker : MobileInterfaces.IDBWorkerTaskComment, MobileInterfaces.IDBWorkerAuth, MonitoringInterfaces.IDBWorkerMonitoring, MonitoringInterfaces.IDBWorkerMonitoringFR
    {
        private readonly DBContext _Dbcontext;
        public DBWorker(DBContext Dbcontext) => _Dbcontext = Dbcontext;

        #region Запись в БД
        public void TaskAddAndUpdate(InnerModels.InnerTask innerTask)
        {
            try
            {
                List<DBModels.Task> DBtasks = _Dbcontext.Tasks.Where(tsk => tsk.TaskId == innerTask.TaskId).ToList();
                if (DBtasks.Count > 0)
                {
                    if (DBtasks[0].Status != innerTask.Status || DBtasks[0].Description != innerTask.Description || DBtasks[0].Subscribers != innerTask.Subscribers)
                    {
                        Program.Logger.Log("DBWorker.TaskAddAndUpdate", $"Found a task in DB. TaskUUID: {DBtasks[0].TaskId}");
                        if (DBtasks[0].Status != innerTask.Status)
                        {
                            List<string> userUuid = new List<string>() {innerTask.Author};
                            userUuid.AddRange(innerTask.GetSubscribers());
                            Program.FCM.Push(GetMobileIds(innerTask.Author, innerTask.GetSubscribers(), innerTask.MobileId), innerTask.Number, "task", innerTask.Status);
                            AddMessage("task", innerTask.Number, userUuid, innerTask.MobileId, new List<string>() { innerTask.Number, innerTask.Status, innerTask.TaskId});
                        }
                        DBtasks[0].Status = innerTask.Status;
                        DBtasks[0].Description = innerTask.Description;
                        DBtasks[0].LastModified = DateTimeOffset.Now.ToUnixTimeSeconds();
                        DBtasks[0].WhoModified = innerTask.MobileId;
                        DBtasks[0].Title = innerTask.Title;
                        DBtasks[0].Subscribers = innerTask.Subscribers;
                        Program.Logger.Log("DBWorker.TaskAddAndUpdate", $"Task changed successfully. TaskUUID: {DBtasks[0].TaskId}");
                        _Dbcontext.SaveChanges();
                    }
                    else
                    {
                        Program.Logger.Log("DBWorker.TaskAddAndUpdate", $"Task not need to update. TaskUUID: {DBtasks[0].TaskId}");
                        return;
                    }
                }
                else
                {
                    Program.Logger.Log("DBWorker.TaskAddAndUpdate", "Task wasn't found ib DB. Creating task.");
                    DBModels.Task DBTask = new DBModels.Task()
                    {
                        InnerId = Guid.NewGuid(),
                        Number = innerTask.Number,
                        TaskId = innerTask.TaskId,
                        OrgUUID = innerTask.OrgUUID,
                        DateCreate = innerTask.DateCreate,
                        Title = innerTask.Title,
                        Description = innerTask.Description,
                        Status = innerTask.Status,
                        Author = innerTask.Author,
                        Subscribers = innerTask.Subscribers,
                        WhoModified = innerTask.MobileId,
                        LastModified = DateTimeOffset.Now.ToUnixTimeSeconds()
                    };
                    if (innerTask.ImageSource != null)
                    {
                        foreach (string image in innerTask.ImageSource)
                        {
                            if (string.IsNullOrEmpty(DBTask.ImageSource))
                                DBTask.ImageSource += image;
                            else
                                DBTask.ImageSource += ';' + image;
                        }
                    }
                    _Dbcontext.Tasks.Add(DBTask);
                    _Dbcontext.SaveChanges();
                    Program.Logger.Log("DBWorker.TaskAddAndUpdate", $"Task successfully created with UUID: {DBTask.TaskId}");
                }
                //Program.FCM.Push(GetMobileIds(innerTask.Author, innerTask.GetSubscribers(), innerTask.MobileId), innerTask.Number, "task", innerTask.Status);
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.CreateTask", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
            }
        }
        public void CommentAddAndUpdate (InnerModels.InnerComment innerComment, bool selfComment = false)
        {
            try
            {
                List<DBModels.Task> tasks = _Dbcontext.Tasks.Where(task => task.TaskId == innerComment.TaskId).ToList();
                if (tasks.Count > 0 && _Dbcontext.Comments.Where(comm => comm.CommentId == innerComment.CommentId).ToList().Count == 0)
                {
                    Program.Logger.Log("DBWorker.CommentAddAndUpdate", $"Found a task to this comment in DB. TaskUUID: {innerComment.TaskId}");
                    DBModels.Comment DBComment = new DBModels.Comment()
                    {
                        InnerId = Guid.NewGuid(),
                        TaskId = innerComment.TaskId,
                        UserUUID = innerComment.UserUUID,
                        CommentId = innerComment.CommentId,
                        DateCreate = innerComment.DateCreate,
                        Message = innerComment.Message,
                        LastModified = DateTimeOffset.Now.ToUnixTimeSeconds(),
                        WhoModified = innerComment.MobileId,
                        Name = innerComment.Name
                    };
                    if (innerComment.ImageSource != null)
                    {
                        foreach (string image in innerComment.ImageSource)
                        {
                            if (string.IsNullOrEmpty(DBComment.ImageSource))
                                DBComment.ImageSource += image;
                            else
                                DBComment.ImageSource += ';' + image;
                        }
                    }
                    Program.Logger.Log("DBWorker.CommentAddAndUpdate", $"Changed data of last modify this task in DB. TaskUUID: {innerComment.TaskId}");
                    tasks[0].LastModified = DateTimeOffset.Now.ToUnixTimeSeconds();
                    tasks[0].WhoModified = innerComment.MobileId;
                    _Dbcontext.Comments.Add(DBComment);
                    _Dbcontext.SaveChanges();
                    Program.Logger.Log("DBWorker.CommentAddAndUpdate", $"Comment sucessfully write into DB. CommentUUID: {innerComment.CommentId}");

                    List<string> userUuids = new List<string>() { tasks[0].Author };
                    userUuids.AddRange(tasks[0].Subscribers != null ? tasks[0].Subscribers.Split(';').ToList() : new List<string>());

                    if (!selfComment)
                    {
                        Program.FCM.Push(GetMobileIds(tasks[0].Author, tasks[0].Subscribers != null ? tasks[0].Subscribers.Split(';').ToList() : new List<string>(), ""), tasks[0].Number, "comment", innerComment.Message, innerComment.CommentId);
                        AddMessage("comment", innerComment.CommentId, userUuids, innerComment.MobileId, new List<string>() { tasks[0].Number, innerComment.Message, tasks[0].TaskId});
                    }
                    Program.Logger.Log("DBWorker.PostComment", $"Comment successfully created with UUID: {innerComment.CommentId}");
                }
                else
                {
                    Program.Logger.Log("DBWorker.PostComment", $"Comment not need to add. CommentUUID: {innerComment.CommentId}");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.CommentAddAndUpdate", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
            }
        }
        public void FileAdd(List<string> files, string Uuid)
        {
            try
            {
                List<DBModels.Task> DBTasks = _Dbcontext.Tasks.Where(tsk => tsk.TaskId == Uuid).ToList();
                if (DBTasks.Count > 0)
                {
                    Program.Logger.Log("DBWorker.FileAttach", $"Found a task in DB. TaskUUID: {DBTasks[0].TaskId}");
                    foreach (string image in files)
                    {
                        if (string.IsNullOrEmpty(DBTasks[0].ImageSource))
                            DBTasks[0].ImageSource += image;
                        else
                            DBTasks[0].ImageSource += ';' + image;
                    }
                }
                else
                {
                    List<DBModels.Comment> DBComments = _Dbcontext.Comments.Where(com => com.CommentId == Uuid).ToList();
                    if (DBComments.Count > 0)
                    {
                        Program.Logger.Log("DBWorker.FileAttach", $"Found a comment in DB. TaskUUID: {DBComments[0].TaskId}");
                        foreach (string image in files)
                        {
                            if (string.IsNullOrEmpty(DBComments[0].ImageSource))
                                DBComments[0].ImageSource += image;
                            else
                                DBComments[0].ImageSource += ';' + image;
                        }
                    }
                }
                _Dbcontext.SaveChanges();
                Program.Logger.Log("DBWorker.FileAttach", "Files successfully added to task.");
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.FileAttach", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
            }
        }
        public void UserAddAndUpdate (InnerModels.InnerUser innerUser)
        {
            try
            {
                List<DBModels.User> users = _Dbcontext.Users.Where(usr => usr.LoginMp == innerUser.LoginMp && usr.PasswordMp == innerUser.PasswordMp).ToList();
                if (users.Count > 0)
                {
                    Program.Logger.Log("DBWorker.UserAdd", $"Update user in DB with UUID: {innerUser.UUID}");
                    if (string.IsNullOrEmpty(innerUser.Name)) users[0].Name = innerUser.Name;
                    if (string.IsNullOrEmpty(innerUser.Phone)) users[0].Phone = innerUser.Phone;
                    if (string.IsNullOrEmpty(innerUser.LoginMp)) users[0].LoginMp = innerUser.LoginMp;
                    if (string.IsNullOrEmpty(innerUser.PasswordMp)) users[0].PasswordMp = innerUser.PasswordMp;
                    if (string.IsNullOrEmpty(innerUser.LoginSd)) users[0].LoginSd = innerUser.LoginSd;
                    if (string.IsNullOrEmpty(innerUser.PasswordSd)) users[0].PasswordSd = innerUser.PasswordSd;
                    if (string.IsNullOrEmpty(innerUser.AuthKey)) users[0].AuthKey = innerUser.AuthKey;
                    if (innerUser.GetEmailMessage != null) users[0].GetEmailMessage = innerUser.GetEmailMessage;
                    if (innerUser.GetSubMessage !=null) users[0].GetSubMessage = innerUser.GetSubMessage;
                    foreach (string mobileid in innerUser.MobileIds)
                    {
                        if (string.IsNullOrEmpty(users[0].MobileIds))
                            users[0].MobileIds += mobileid;
                        else if (!users[0].MobileIds.Contains(mobileid))
                            users[0].MobileIds += ';' + mobileid;
                    }
                    if (users[0].UUID != innerUser.UUID)
                    {
                        users[0].UUID = innerUser.UUID;
                        foreach(var task in _Dbcontext.Tasks.Where(tsk => tsk.Author == users[0].UUID ))
                        {
                            task.Author = users[0].UUID;
                        }
                        foreach (var task in _Dbcontext.Tasks.Where(tsk => tsk.Subscribers.Contains(users[0].UUID)))
                        {
                            task.Subscribers += users[0].UUID + ';';
                        }
                        foreach (var commnet in _Dbcontext.Comments.Where(com => com.UserUUID == users[0].UUID))
                        {
                            commnet.UserUUID = users[0].UUID;
                        }
                    }
                }
                else
                {
                    Program.Logger.Log("DBWorker.UserAdd", $"Create user in DB with UUID: {innerUser.UUID}");
                    DBModels.User DBuser = new DBModels.User()
                    {
                        AuthKey = innerUser.AuthKey,
                        UUID = innerUser.UUID,
                        LoginMp = innerUser.LoginMp,
                        PasswordMp = innerUser.PasswordMp,
                        LoginSd = innerUser.LoginSd,
                        PasswordSd = innerUser.PasswordSd,
                        GetEmailMessage = innerUser.GetEmailMessage,
                        GetSubMessage = innerUser.GetSubMessage,
                        Name = innerUser.Name,
                        Phone = innerUser.Phone,
                        Id = Guid.NewGuid(),
                        MobileIds = string.Empty
                    };
                    if (innerUser.MobileIds != null)
                    {
                        foreach (string image in innerUser.MobileIds)
                        {
                            if (string.IsNullOrEmpty(image))
                                DBuser.MobileIds += image;
                            else if(!DBuser.MobileIds.Contains(image))
                                DBuser.MobileIds += ';' + image;
                        }
                    }
                    _Dbcontext.Users.Add(DBuser);
                }
                _Dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.UserAdd", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
            }
        }
        public void RemoveMobileIdData(string mobileId)
        {
            foreach (DBModels.User user in _Dbcontext.Users.Where(usr => usr.MobileIds.Contains(mobileId)).ToList())
            {
                if (mobileId.Length == user.MobileIds.Length)
                {
                    user.MobileIds = string.Empty;
                }
                else if (user.MobileIds.IndexOf(mobileId) + mobileId.Length == user.MobileIds.Length)
                {
                    user.MobileIds = user.MobileIds.Remove(user.MobileIds.IndexOf(mobileId) - 1, mobileId.Length + 1);
                }
                else
                {
                    user.MobileIds = user.MobileIds.Remove(user.MobileIds.IndexOf(mobileId), mobileId.Length + 1);
                }
            }
            _Dbcontext.SaveChanges();
        }
        public void AddKey(MonitoringModels.Innerkey innerKey)
        {
            if (_Dbcontext.Key.Where(key => key.login == innerKey.login && key.password == innerKey.password) != null)
            {
                Program.Logger.Log("DBWorker.AddKey", $"This {innerKey.login} - {innerKey.password} already in BD.");
            }
            else
            {
                DBModels.Key key = new Models.Key
                {
                    id = Guid.NewGuid(),
                    login = innerKey.login,
                    password = innerKey.password
                };
                _Dbcontext.Key.Add(key);
                _Dbcontext.SaveChanges();
                Program.Logger.Log("DBWorker.AddKey", $"This {innerKey.login} - {innerKey.password} successfully added in BD.");
            }
        }
        public void AddFRData(List<TrueKeyServer.Models.Monitoring.InnerFRData> listFRData)
        {
            try
            {
                foreach (TrueKeyServer.Models.Monitoring.InnerFRData innerFRData in listFRData)
                {
                    List<Models.FRData> FRData = _Dbcontext.FRData.Where(frd => frd.SerialNumber == innerFRData.SerialNumber).ToList();
                    if (FRData.Count > 0)
                    {
                        FRData[0].Owner = innerFRData.Owner;
                        FRData[0].Inn = innerFRData.Inn;
                        FRData[0].AdressPlaceOfS = innerFRData.AdressPlaceOfS;
                        FRData[0].NDS = innerFRData.NDS;
                        FRData[0].Model = innerFRData.Model;
                        FRData[0].FirmwareID = innerFRData.FirmwareID;
                        FRData[0].SerialNumber = innerFRData.SerialNumber;
                        FRData[0].RegisterNumber = innerFRData.RegisterNumber;
                        FRData[0].NomberFN = innerFRData.NomberFN;
                        FRData[0].VersConfigur = innerFRData.VersConfigur;
                        FRData[0].VersBoot = innerFRData.VersBoot;
                        FRData[0].VersionFn = innerFRData.VersionFn;
                        FRData[0].OperatorFD = innerFRData.OperatorFD;
                        FRData[0].RegFnCount = innerFRData.RegFnCount;
                        FRData[0].AmountFnRereg = innerFRData.AmountFnRereg;
                        FRData[0].CheckResource = innerFRData.CheckResource;
                        FRData[0].FFD = innerFRData.FFD;
                        FRData[0].EndDataFN = innerFRData.EndDataFN;
                        FRData[0].FnExpireDays = innerFRData.FnExpireDays;
                        FRData[0].QueueDocOFD = innerFRData.QueueDocOFD;
                        FRData[0].NumFirstUnDoc = innerFRData.NumFirstUnDoc;
                        FRData[0].DateFirstUnDoc = innerFRData.DateFirstUnDoc;
                        FRData[0].StateInfoEx = innerFRData.StateInfoEx;
                        FRData[0].LastModifiedDate = innerFRData.LastModifiedDate;
                        FRData[0].LastRegDateFN = innerFRData.LastRegDateFN;
                    }
                    else
                    {
                        _Dbcontext.FRData.Add(new DBModels.FRData
                        {
                            Owner = innerFRData.Owner,
                            Inn = innerFRData.Inn,
                            AdressPlaceOfS = innerFRData.AdressPlaceOfS,
                            NDS = innerFRData.NDS,
                            Model = innerFRData.Model,
                            FirmwareID = innerFRData.FirmwareID,
                            SerialNumber = innerFRData.SerialNumber,
                            RegisterNumber = innerFRData.RegisterNumber,
                            NomberFN = innerFRData.NomberFN,
                            VersConfigur = innerFRData.VersConfigur,
                            VersBoot = innerFRData.VersBoot,
                            VersionFn = innerFRData.VersionFn,
                            OperatorFD = innerFRData.OperatorFD,
                            RegFnCount = innerFRData.RegFnCount,
                            AmountFnRereg = innerFRData.AmountFnRereg,
                            CheckResource = innerFRData.CheckResource,
                            FFD = innerFRData.FFD,
                            EndDataFN = innerFRData.EndDataFN,
                            FnExpireDays = innerFRData.FnExpireDays,
                            QueueDocOFD = innerFRData.QueueDocOFD,
                            NumFirstUnDoc = innerFRData.NumFirstUnDoc,
                            DateFirstUnDoc = innerFRData.DateFirstUnDoc,
                            StateInfoEx = innerFRData.StateInfoEx,
                            LastModifiedDate = innerFRData.LastModifiedDate,
                            LastRegDateFN = innerFRData.LastRegDateFN,
                            Id = new Guid()
                        });
                    }
                    _Dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.AddFRData", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
            }
        }
        public void AddMessage(string typeOfPush, string linkToObj, List<string> userUuids, string messageCreator, List<string> subData)
        {
            //subData[0] - номер связанного объекта
            //subData[1] - связанный параметр объекта
            //subData[2] - идентификатор заявки для МП

            string title = String.Empty;
            string body = String.Empty;
            switch (typeOfPush)
            {
                case "task":
                    body = subData[1] switch
                    {
                        "InProgress" => $"Мы приступили к работе по заявке {subData[0]}.",
                        "WaitForResponse" => $"В заявке {subData[0]} требуется уточнение.",
                        "Resolved" => $"Заявка {subData[0]} выполнена. Если проблема сохраняется - дайте нам знать.",
                        _ => string.Empty,
                    };
                    title = $"Изменение статуса задачи {subData[0]}.";
                    break;
                case "comment":
                    title = $"Новый комментарий к заявке {subData[0]}.";
                    body = $"Комментарий:  {subData[1]}";
                    break;
            }
            if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(body))
            {
                Models.Message dbMessage = new DBModels.Message()
                {
                    Id = Guid.NewGuid(),
                    timeChange = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                    usersUuids = "",
                    dateCreate = DateTime.Now.ToUniversalTime(),
                    linkToObj = linkToObj,
                    msg = body,
                    title = title,
                    type = typeOfPush,
                    imageSource = "",
                    MessageCreator = messageCreator,
                    taskNumber = subData[2]
                };
                foreach (string userUuid in userUuids)
                {
                    dbMessage.usersUuids += userUuid + ';';
                }
                _Dbcontext.Messages.Add(dbMessage);
                _Dbcontext.SaveChanges();
            }
        }
        #endregion

        #region Чтение из БД
        public List<InnerModels.InnerTask> TaskGet(string orgId, string lastSync, string mobileid, bool nullRequest)
        {
            List<InnerModels.InnerTask> innertasks = new List<InnerModels.InnerTask>();
            try
            {
                DBModels.User user = _Dbcontext.Users.AsNoTracking().Where(usr => usr.MobileIds.Contains(mobileid)).FirstOrDefault();
                if (user != null)
                {
                    foreach (DBModels.Task DBtask in _Dbcontext.Tasks.AsNoTracking().Where(task => task.OrgUUID == orgId &&
                                                                           task.LastModified > Convert.ToInt64(lastSync) &&
                                                                           (nullRequest || task.WhoModified != mobileid) &&
                                                                           (task.Subscribers.Contains(user.UUID) || (task.Author == user.UUID))
                                                                  ).ToList())
                    {
                        Program.Logger.Log("MobileControllersV1.GetChange", $"Found a task with UUID: {DBtask.TaskId}");
                        InnerModels.InnerTask innerTask = new InnerModels.InnerTask()
                        {
                            TaskId = DBtask.TaskId,
                            Number = DBtask.Number,
                            Status = DBtask.Status,
                            Title = DBtask.Title,
                            Description = DBtask.Description,
                            DateCreate = DBtask.DateCreate,
                            OrgUUID = DBtask.OrgUUID,
                            Subscribers = DBtask.Subscribers,
                            Author = DBtask.Author
                        };
                        if (!string.IsNullOrEmpty(DBtask.ImageSource)) foreach (string image in DBtask.ImageSource.Split(';')) innerTask.ImageSource.Add(image);
                        foreach (DBModels.Comment DBcomment in _Dbcontext.Comments.AsNoTracking().Where(com => com.TaskId == innerTask.TaskId).ToList())
                        {
                            InnerModels.InnerComment innerComment = new InnerModels.InnerComment()
                            {
                                CommentId = DBcomment.CommentId,
                                TaskId = DBcomment.TaskId,
                                Message = DBcomment.Message,
                                DateCreate = DBcomment.DateCreate,
                                UserUUID = DBcomment.UserUUID,
                                ImageSource = new List<string>()
                            };
                            if (DBcomment.ImageSource != null) foreach (string image in DBcomment.ImageSource.Split(';')) innerComment.ImageSource.Add(image);
                            innerTask.Comments.Add(innerComment);
                        }
                        innertasks.Add(innerTask);
                    }
                }
                else
                {
                    Program.Logger.Log("DBWorker.TaskGet", "The user with this Mobileid was not found.");
                }
                return innertasks;
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.TaskGet", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
                return innertasks;
            }
        }
        public string TaskOrComment(string id)
        {
            try
            {
                if (_Dbcontext.Tasks.AsNoTracking().Where(tsk => tsk.TaskId == id).Count() > 0) return "task";
                else if (_Dbcontext.Comments.AsNoTracking().Where(com => com.CommentId == id).Count() > 0) return "comment";
            }
            catch(Exception ex)
            {
                Program.Logger.Log("DBWorker.TaskGet", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
            }
            return "void";
        }
        public List<InnerModels.InnerComment> CommentGet(string taskId, string lastSync, string mobileid, bool nullRequest)
        {
            List<InnerModels.InnerComment> innerComments = new List<InnerModels.InnerComment>();
            try
            {
                foreach (DBModels.Comment DBcomment in _Dbcontext.Comments.AsNoTracking().Where(com => com.TaskId == taskId && com.LastModified > Convert.ToInt64(lastSync) && (nullRequest || com.WhoModified != mobileid)).ToList())
                {
                    Program.Logger.Log("DBWorker.CommentGet", $"Found a comment to this task with UUID: {DBcomment.CommentId}");
                    InnerModels.InnerComment innerComment = new InnerModels.InnerComment()
                    {
                        CommentId = DBcomment.CommentId,
                        TaskId = DBcomment.TaskId,
                        Message = DBcomment.Message,
                        DateCreate = DBcomment.DateCreate,
                        UserUUID = DBcomment.UserUUID,
                        Name = DBcomment.Name,
                        ImageSource = new List<string>()
                    };
                    if (!string.IsNullOrEmpty(DBcomment.ImageSource)) foreach (string image in DBcomment.ImageSource.Split(';')) innerComment.ImageSource.Add(image);
                    innerComments.Add(innerComment);
                }
                return innerComments;
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.CommentGet", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
                return innerComments;
            }
        }
        public InnerModels.InnerUser UserGet(string login, string password)
        {
            try
            {
                List<DBModels.User> users = _Dbcontext.Users.AsNoTracking().Where(usr => usr.LoginMp == login && usr.PasswordMp == password).ToList();
                Program.Logger.Log("DBWorker.UserGet", $"Users found in the database: {users.Count}");
                if (users.Count > 0)
                {
                    Program.Logger.Log("DBWorker.UserGet", $"Found a user with UUID: {users[0].UUID}");
                    InnerModels.InnerUser innerUser = new InnerModels.InnerUser
                    {
                        AuthKey = users[0].AuthKey,
                        UUID = users[0].UUID,
                        LoginMp = users[0].LoginMp,
                        PasswordMp = users[0].PasswordMp,
                        LoginSd = users[0].LoginSd,
                        PasswordSd = users[0].PasswordSd,
                        Name = users[0].Name,
                        Phone = users[0].Phone,
                        GetEmailMessage = users[0].GetEmailMessage,
                        GetSubMessage = users[0].GetSubMessage
                    };
                    if (!string.IsNullOrEmpty(users[0].MobileIds)) foreach (string dbmobileId in users[0].MobileIds.Split(';')) innerUser.MobileIds.Add(dbmobileId);
                    return innerUser;
                }
                else
                {
                    Program.Logger.Log("DBWorker.UserGet", "The user with the following username\\password was not found.");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.UserGet", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
            }
            return new InnerModels.InnerUser();
        }
        public InnerModels.InnerUser UserGet(string mobileId)
        {
            try
            {
                List<DBModels.User> users = _Dbcontext.Users.AsNoTracking().Where(usr => usr.MobileIds.Contains(mobileId)).ToList();
                Program.Logger.Log("DBWorker.UserGet", $"Users found in the database: {users.Count}");
                if (users.Count > 0)
                {
                    Program.Logger.Log("DBWorker.UserGet", $"Found a user with UUID: {users[0].UUID}");
                    InnerModels.InnerUser innerUser = new InnerModels.InnerUser()
                    {
                        AuthKey = users[0].AuthKey,
                        UUID = users[0].UUID,
                        LoginMp = users[0].LoginMp,
                        PasswordMp = users[0].PasswordMp,
                        LoginSd = users[0].LoginSd,
                        PasswordSd = users[0].PasswordSd,
                        Name = users[0].Name,
                        Phone = users[0].Phone,
                        GetEmailMessage = users[0].GetEmailMessage,
                        GetSubMessage = users[0].GetSubMessage
                    };
                    foreach (string dbmobileId in users[0].MobileIds.Split(';')) innerUser.MobileIds.Add(dbmobileId);
                    return innerUser;
                }
                else
                {
                    Program.Logger.Log("DBWorker.UserGet", "The user with the following username\\password was not found.");
                    throw new Exception("The user with the following username\\password was not found.");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.UserGet", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
                return null;
            }
        }
        public InnerModels.InnerUser UserGetById(string Uuid)
        {
            try
            {
                List<DBModels.User> users = _Dbcontext.Users.AsNoTracking().Where(usr => usr.UUID.Contains(Uuid)).ToList();
                Program.Logger.Log("DBWorker.UserGet", $"Users found in the database: {users.Count}");
                if (users.Count > 0)
                {
                    Program.Logger.Log("DBWorker.UserGet", $"Found a user with UUID: {users[0].UUID}");
                    InnerModels.InnerUser innerUser = new InnerModels.InnerUser()
                    {
                        AuthKey = users[0].AuthKey,
                        UUID = users[0].UUID,
                        LoginMp = users[0].LoginMp,
                        PasswordMp = users[0].PasswordMp,
                        LoginSd = users[0].LoginSd,
                        PasswordSd = users[0].PasswordSd,
                        Name = users[0].Name,
                        Phone = users[0].Phone,
                        GetEmailMessage = users[0].GetEmailMessage,
                        GetSubMessage = users[0].GetSubMessage
                    };
                    foreach (string dbmobileId in users[0].MobileIds.Split(';')) innerUser.MobileIds.Add(dbmobileId);
                    return innerUser;
                }
                else
                {
                    Program.Logger.Log("DBWorker.UserGet", "The user with the following Uuid was not found.");
                    throw new Exception("The user with the following Uuid was not found.");
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.UserGet", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
                return null;
            }
        }
        public InnerModels.InnerUser UserGetOnPhone(string phone)
        {
            try
            {
                List<DBModels.User> users = _Dbcontext.Users.AsNoTracking().Where(usr => usr.LoginMp == phone).ToList();
                Program.Logger.Log("DBWorker.UserGet", $"Users found in the database: {users.Count}");
                if (users.Count > 0)
                {
                    Program.Logger.Log("DBWorker.UserGet", $"Found a user with UUID: {users[0].UUID}");
                    InnerModels.InnerUser innerUser = new InnerModels.InnerUser()
                    {
                        AuthKey = users[0].AuthKey,
                        UUID = users[0].UUID,
                        LoginMp = users[0].LoginMp,
                        PasswordMp = users[0].PasswordMp,
                        LoginSd = users[0].LoginSd,
                        PasswordSd = users[0].PasswordSd,
                        Name = users[0].Name,
                        Phone = users[0].Phone,
                        GetEmailMessage = users[0].GetEmailMessage,
                        GetSubMessage = users[0].GetSubMessage
                    };
                    foreach (string dbmobileId in users[0].MobileIds.Split(';')) innerUser.MobileIds.Add(dbmobileId);
                    return innerUser;
                }
                else
                {
                    users = _Dbcontext.Users.AsNoTracking().Where(usr => usr.Phone == phone).ToList();
                    Program.Logger.Log("DBWorker.UserGet", $"Users found in the database: {users.Count}");
                    if (users.Count > 0)
                    {
                        InnerModels.InnerUser innerUser = new InnerModels.InnerUser()
                        {
                            AuthKey = users[0].AuthKey,
                            UUID = users[0].UUID,
                            LoginMp = users[0].LoginMp,
                            PasswordMp = users[0].PasswordMp,
                            LoginSd = users[0].LoginSd,
                            PasswordSd = users[0].PasswordSd,
                            Name = users[0].Name,
                            Phone = users[0].Phone,
                            GetEmailMessage = users[0].GetEmailMessage,
                            GetSubMessage = users[0].GetSubMessage
                        };
                        foreach (string dbmobileId in users[0].MobileIds.Split(';')) innerUser.MobileIds.Add(dbmobileId);
                        return innerUser;
                    }
                    else
                    {
                        Program.Logger.Log("DBWorker.UserGet", "The user with the following Uuid was not found.");
                        throw new Exception("The user with the following Uuid was not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.UserGet", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
                return null;
            }
        }
        public List<MonitoringModels.Innerkey> GetKeys()
        {
            List<MonitoringModels.Innerkey> keys = new List<MonitoringModels.Innerkey>();
            try
            {
                foreach (DBModels.Key key in _Dbcontext.Key.AsNoTracking().ToList())
                {
                    keys.Add(new MonitoringModels.Innerkey()
                    {
                        login = key.login,
                        password = key.password
                    });
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.UserGet", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
            }
            return keys;
        }
        public List<TrueKeyServer.Models.Monitoring.InnerFRData> GetFRData()
        {
            List<TrueKeyServer.Models.Monitoring.InnerFRData> ListFRData = new List<MonitoringModels.InnerFRData>();
            foreach(Models.FRData FRData in _Dbcontext.FRData.AsNoTracking())
            {
                ListFRData.Add(new MonitoringModels.InnerFRData
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
            return ListFRData;
        }
        public List<InnerModels.Message> GetMessages(string timeLastSync, string mobileId)
        {
            List<InnerModels.Message> messages = new List<InnerModels.Message>();
            try
            {
                if (!string.IsNullOrEmpty(timeLastSync) && timeLastSync != "0")
                {
                    foreach (Models.Message message in _Dbcontext.Messages.Where(msg => Convert.ToInt64(msg.timeChange) > Convert.ToInt64(timeLastSync) && msg.usersUuids.Contains(UserGet(mobileId).UUID) && msg.MessageCreator != mobileId))
                    {
                        messages.Add(new InnerModels.Message()
                        {
                            dateCreate = message.dateCreate,
                            type = message.type,
                            msg = message.msg,
                            title = message.title,
                            linkToObj = message.linkToObj,
                            imageSource = message.imageSource
                        });
                    }
                }
                else
                {
                    foreach (var messageGroup in _Dbcontext.Messages.Where(msg => msg.linkToObj.Contains("serviceCall") && msg.usersUuids.Contains(UserGet(mobileId).UUID) && msg.MessageCreator != mobileId).AsEnumerable().GroupBy(msg => msg.linkToObj))
                    {
                        DB.Models.Message message = messageGroup.FirstOrDefault(msg => msg.timeChange == messageGroup.Max(msg => msg.timeChange));
                        if (message != default)
                        {
                            messages.Add(new InnerModels.Message()
                            {
                                dateCreate = message.dateCreate,
                                type = message.type,
                                msg = message.msg,
                                title = message.title,
                                linkToObj = message.linkToObj,
                                imageSource = message.imageSource
                            });
                        }
                    }
                    foreach (var messageGroup in _Dbcontext.Messages.Where(msg => msg.linkToObj.Contains("comment") && msg.usersUuids.Contains(UserGet(mobileId).UUID) && msg.MessageCreator != mobileId).AsEnumerable().GroupBy(msg => msg.taskNumber))
                    {
                        DB.Models.Message message = messageGroup.FirstOrDefault(msg => msg.timeChange == messageGroup.Max(msg => msg.timeChange));
                        if (message != default)
                        {
                            messages.Add(new InnerModels.Message()
                            {
                                dateCreate = message.dateCreate,
                                type = message.type,
                                msg = message.msg,
                                title = message.title,
                                linkToObj = message.linkToObj,
                                imageSource = message.imageSource
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("DBWorker.GetMessages", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace + '\n' + "InnerException: " + ex.InnerException, "ERROR");
            }
            return messages;
        }
        #endregion

        #region Служебные функции
        private List<string> GetMobileIds(string author, List<string> subscribers, string exclMobileId)
        {
            Program.Logger.Log("DBWorker.GetMobileIds", "GetMobileIds.");
            List<string> mobileIds = new List<string>();
            List<DBModels.User> users = new List<DBModels.User>();
            foreach (string userUuid in subscribers)
            {
                users.AddRange(_Dbcontext.Users.Where(usr => usr.UUID == userUuid && usr.GetSubMessage == false));
            }
            users.AddRange(_Dbcontext.Users.Where(usr => usr.UUID == author));

            foreach (DBModels.User user in users)
            {
                foreach (string mobileId in user.MobileIds.Split(';'))
                {
                    if (mobileId != exclMobileId && !mobileIds.Contains(mobileId))
                        mobileIds.Add(mobileId);
                }
            }
            return mobileIds;
        }
        #endregion
    }
}
