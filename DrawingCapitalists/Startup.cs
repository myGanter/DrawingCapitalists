using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ninject;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using Core.Services.VueApp;
using Core.Services.DB;
using Core.Services.DB.MsSql;
using Core.Services.DB.Actions;
using Core.Expansions;
using Core.Services.DI;
using Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DrawingCapitalists
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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => 
                {
                    options.LoginPath = new PathString("/");
                });

            var diConf = new NinjectConfigure();

            //todo
            string dbConnection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDBContext, MsSqlApplicationContext>(o => o.UseSqlServer(dbConnection));
            diConf.AddTypeConfig(c => c.Bind<AppDBContext>().ToMethod(c => 
            new MsSqlApplicationContext(
                new DbContextOptionsBuilder<MsSqlApplicationContext>()
                .UseSqlServer(dbConnection)
                .Options)));

            services.AddControllersWithViews().AddRazorRuntimeCompilation(); 

            services.AddSignalR();

            services.AddScoped<ActionsBuilder>();

            services.AddSingleton<VueTemplateService>();

            services.AddSingleton<IKernel>(sp => new StandardKernel(diConf));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IKernel di, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDBLogger();

            if (env.IsDevelopment()) 
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "vue",    
                    pattern: "/{page?}",
                    defaults: new { controller = "Vue", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}");
            });

            //init db
            using (var dbinit = di.Get<DBInitializer>())
            {
                dbinit.Initialize();
            }

            di.Get<Core.Services.Process.ProcessController>().StartProcessor();            
        }
    }
}