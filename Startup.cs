using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using Swashbuckle.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace TrueKeyServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.MemoryBufferThreshold = Int32.MaxValue;
            });
            services.AddDbContext<DB.DBContext>();
            services.AddTransient<DB.MobileInterfaces.IDBWorkerAuth, DB.DBWorker>();
            services.AddTransient<DB.MobileInterfaces.IDBWorkerTaskComment, DB.DBWorker>();
            services.AddSingleton<Support.TaskQueue>();

            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddLogging();
            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
                {
                    options.IncludeXmlComments("help.xml");
                    options.CustomSchemaIds(x => x.FullName);
                    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()});
                }
            );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
