using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TrueKeyServer.Support
{
    public static class FixDB
    {
        public static void UseFix(DB.DBContext AC)
        {
            foreach (DB.Models.Task task in AC.Tasks.AsNoTracking().ToList())
            {
                Program.Logger.Log("Main", "Task " + task.TaskId);
                List<DB.Models.Task> copyTasks = AC.Tasks.AsNoTracking().Where(tsk => tsk.TaskId == task.TaskId).ToList();
                if (copyTasks.Count > 1)
                {
                    Program.Logger.Log("Main", "Task " + task.TaskId + " COPY = " + copyTasks.Count);
                    DB.Models.Task goodTask = new DB.Models.Task();

                    List<DB.Models.Task> toRemoove = copyTasks.Where(tsk => tsk.LastModified < copyTasks.Max(tsk => tsk.LastModified)).ToList();
                    if (toRemoove.Count > 0) copyTasks.Except(toRemoove);

                    goodTask = AC.Tasks.Where(tsk => tsk.Status == "Closed").FirstOrDefault();
                    if (goodTask == default)
                    {
                        goodTask = AC.Tasks.Where(tsk => tsk.Status == "Resolved").FirstOrDefault();
                        if (goodTask == default)
                        {
                            goodTask = AC.Tasks.Where(tsk => tsk.Status == "WaitForResponse").FirstOrDefault();
                            if (goodTask == default)
                            {
                                goodTask = AC.Tasks.Where(tsk => tsk.Status == "New").FirstOrDefault();
                                if (goodTask == default)
                                {
                                    goodTask = copyTasks.FirstOrDefault();
                                }
                            }
                        }
                    }
                    AC.Tasks.RemoveRange(AC.Tasks.AsNoTracking().Where(tsk => tsk.TaskId == task.TaskId && tsk.InnerId != goodTask.InnerId));
                }
            }
            AC.SaveChanges();
        }
        public static void ClearOldMessages(DB.DBContext AC)
        {
            long timeEnd = DateTimeOffset.Now.AddMonths(-2).ToUnixTimeSeconds();
            foreach (var message in AC.Messages.Where(msg=>Convert.ToInt64(msg.timeChange) < timeEnd).ToList())
            {
                DB.Models.Task task;
                if (message.linkToObj.Contains("comment"))
                {
                    task = AC.Tasks.AsNoTracking().FirstOrDefault(tsk => tsk.TaskId == AC.Comments.AsNoTracking().FirstOrDefault(com => com.CommentId == message.linkToObj).TaskId);
                }
                else if (message.linkToObj.Contains("serviceCall"))
                {
                    task = AC.Tasks.AsNoTracking().FirstOrDefault(tsk => tsk.TaskId == message.linkToObj);
                }
                else
                {
                    return;
                }
                if (task != default && (task.Status == "Closed" || task.Status == "Resolved")) AC.Messages.Remove(message);
            }
            AC.SaveChanges();
        }
        public static void FixImagePath(DB.DBContext AC)
        {
            foreach(DB.Models.Task task in AC.Tasks.ToList())
            {
                task.ImageSource = task.ImageSource?.Replace("support.lemma.ru", "lemma-test.itsm365.com");
            }
            foreach (DB.Models.Comment comment in AC.Comments.ToList())
            {
                comment.ImageSource = comment.ImageSource?.Replace("support.lemma.ru", "lemma-test.itsm365.com");
            }
            AC.SaveChanges();
        }
    }
}
