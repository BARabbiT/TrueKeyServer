using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TrueKeyServer.DB
{
    public class DBContext : DbContext
    {
        public DbSet<Models.Key> Key { get; set; }
        public DbSet<Models.FRData> FRData { get; set; }

        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Models.Comment> Comments { get; set; }

        public DbSet<Models.User> Users { get; set; }
        public DbSet<Models.Organisation> Organisations { get; set; }

        public DbSet<Models.Message> Messages { get; set; }

        public DBContext(DbContextOptions<DBContext> options) : base(options) => Database.Migrate();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            }
            else
            {
                optionsBuilder.UseNpgsql(configuration.GetConnectionString("ReleaseConnection"));
            }
        }
    }
}
