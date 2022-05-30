using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueKeyServer.Models.Mobile.Inner
{
    public class InnerTask
    {
        public string TaskId { get; set; }
        public string Number { get; set; }
        public string OrgUUID { get; set; }
        public string DateCreate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<String> ImageSource { get; set; }
        public string Status { get; set; }
        public List<InnerComment> Comments { get; set; }
        public string MobileId { get; set; }
        public string AuthKey { get; set; }
        public string Author { get; set; }
        public string Subscribers { get; set; }

        public InnerTask()
        {
            Comments = new List<InnerComment>();
            ImageSource = new List<string>();
        }
        public InnerTask(Outer.MobileTask.Request task)
        {
            TaskId = task.TaskId;
            OrgUUID = task.OrgUUID;
            Title = task.Title;
            Description = task.Description;
            Status = task.Status;
            MobileId = task.MobileId;
            Comments = new List<InnerComment>();
            ImageSource = new List<string>();
        }
        public InnerTask(Outer.ELMATask.Request task)
        {
            TaskId = task.TaskId;
            Number = task.Number;
            OrgUUID = task.OrgUUID;
            DateCreate = task.DateCreate.ts;
            Title = task.Title;
            Description = task.Description;
            Status = task.Status;
            Author = task.Author;
            Subscribers = task.Subscribers;
            Comments = new List<InnerComment>();
            ImageSource = new List<string>();
        }

        public List<string> GetSubscribers()
        {
            List<string> result = new List<string>();
            if (string.IsNullOrEmpty(Subscribers)) return result;
            foreach (string str in Subscribers.Split(';'))
                result.Add(str);
            return result;
        }
        public void AddSubscribers(string subscriber)
        {
            if (string.IsNullOrEmpty(Subscribers))
                Subscribers += subscriber;
            else
                Subscribers += ';' + subscriber;
        }
    }
}
