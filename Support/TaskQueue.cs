using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.EntityFrameworkCore;

namespace TrueKeyServer.Support
{
    public class TaskQueue
    {
        private readonly ObservableCollection<Models.Mobile.Inner.InnerTask> Tasks;
        private bool InWork = false;

        public TaskQueue()
        {
            Tasks = new ObservableCollection<TrueKeyServer.Models.Mobile.Inner.InnerTask>();
            Tasks.CollectionChanged += Tasks_CollectionChanged;
        }
        private async void Tasks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!InWork)
            {
                InWork = true;
                await QueueNyamNyam();
            }
        }
        public async Task QueueNyamNyam()
        {
            while (Tasks.Count != 0)
            {
                Models.Mobile.Inner.InnerTask task = Tasks.FirstOrDefault();
                if (task != default)
                {
                    using (DB.DBContext dbContext = new DB.DBContext(new DbContextOptionsBuilder<DB.DBContext>().Options))
                    {
                        DB.DBWorker dbWorker = new DB.DBWorker(dbContext);
                        Program.Logger.Log("TaskQueue.QueueNyamNyam", $"Try to write into DB. TaskID: {task.TaskId}");
                        dbWorker.TaskAddAndUpdate(task);
                    }
                    Tasks.Remove(task);
                }
            }
            InWork = false;
        }
        public Task AddNewWork(Models.Mobile.Inner.InnerTask task)
        {
            Tasks.Add(task);
            Program.Logger.Log("TaskQueue.AddNewWork", $"Add task to collection. TaskID: {task.TaskId}");
            return Task.CompletedTask;
        }
    }
}
