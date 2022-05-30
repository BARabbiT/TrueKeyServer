using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Impl;

namespace TrueKeyServer.Support
{
    public class QartzScheduler
    {
        private IScheduler scheduler;
        public async void Start()
        {
            scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
        }
        public async void AddJob()
        {
            IJobDetail jobNom = JobBuilder.Create<Job>()
                .SetJobData(new JobDataMap())
                .Build();
            ITrigger triggerNom = TriggerBuilder.Create()
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever())
            .Build();
            await scheduler.ScheduleJob(jobNom, triggerNom);
        }
    }
    public class Job : IJob
    {
        private void ClearDB()
        {
            try
            {
                using (DB.DBContext AC = new DB.DBContext(new DbContextOptionsBuilder<DB.DBContext>().Options))
                {
                    FixDB.ClearOldMessages(AC);
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Log("QartzScheduler.ClearDB", ex.Message + ex.StackTrace, "ERROR");
            }
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => ClearDB());
        }
    }
}
