using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Microsoft.Extensions.PlatformAbstractions;

namespace TrueKeyServer
{
    public static class Program
    {
        public static Support.Logger Logger = new Support.Logger("KeyServer.log");
        public static Support.FCM FCM = new Support.FCM();
        public static void Main(string[] args)
        {
            try
            {
                //string addres = System.Configuration.ConfigurationManager.AppSettings["NaumenTestModuleAddres"];
                Logger.Log("Main", "START");

                Support.QartzScheduler quartz = new Support.QartzScheduler();
                quartz.Start();
                quartz.AddJob();

                DB.DBContext AC = new DB.DBContext(new DbContextOptionsBuilder<DB.DBContext>().Options);
                AC.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Log("Main", "Error: " + ex.Message + '\n' + "StackTrace: " + ex.StackTrace, "ERROR");
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://0.0.0.0:80");
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSentry();
                });
    }
}
