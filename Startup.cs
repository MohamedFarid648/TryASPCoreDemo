using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TryCoreDemo.Models;

namespace TryCoreDemo
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()//in Microsoft.Extensions.Configuration;
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            //services.AddTransient<TryCoreDemoContext>(); Short life span,An instance will be created every time one is requested
            //services.AddScoped<TryCoreDemoContext>();//Single instance will be created for each scope(current web request)
            //services.AddSingleton<TryCoreDemoContext>();//Single instance for the entire application(Data shared between all classes)



            services.AddDbContext<TryCoreDemoContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("TryCoreDemoContext")));

 

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,TryCoreDemoContext context)
        {
            /*Middle ware : it in Configure method and it will put any  app.AnyMethod() to stack
             
             Asp.Net Core will create object from pipe line and put these methods on it
             */
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();/*Page for Developers 
                (Change it by rightClick on your Solution=>properties=>Debug Tab=>Environment Variable =>value=Development 
                change Development to any value and you will see "/Home/Error" and don't forget   app.UseExceptionHandler you should write it)*/
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();//Enable static files serving for the current request path

            app.UseFileServer();//You can create your demo with index.html and put it in wwwroot and application will run it 
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

           
        }

    }
}
