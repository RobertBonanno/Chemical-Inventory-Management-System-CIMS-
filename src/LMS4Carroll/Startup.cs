using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LMS4Carroll.Data;
using LMS4Carroll.Models;
using LMS4Carroll.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;
using NLog.Web;
using NLog.Extensions.Logging;
using NLog;
using NLog.Config;

namespace LMS4Carroll
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            /*
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.MSSqlServer(Configuration["Serilog:ConnectionString"], Configuration["Serilog:TableName"], autoCreateSqlTable: true)
                .CreateLogger();
            */
            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser,ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddMvc();
            services.AddScoped<LogFilter>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            /*
            services.AddSingleton<Serilog.ILogger>(x =>
            {
                return new LoggerConfiguration().WriteTo.MSSqlServer(Configuration["Serilog:ConnectionString"], Configuration["Serilog:TableName"], autoCreateSqlTable: true).CreateLogger();
            });
            */
            /*
            var columnOptions = new ColumnOptions
            {
                AdditionalDataColumns = new Collection<DataColumn>
                {
                    new DataColumn {DataType = typeof (string), ColumnName = "User"},
                }
            };

            columnOptions.Store.Add(StandardColumn.LogEvent);
            
            services.AddSingleton<Serilog.ILogger>
                     (x => new LoggerConfiguration()
                           .MinimumLevel.Information()
                           .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                           .MinimumLevel.Override("System", LogEventLevel.Error)
                           .WriteTo.MSSqlServer(Configuration["Serilog:ConnectionString"]
                           , Configuration["Serilog:TableName"]
                           , autoCreateSqlTable: true)
                           .CreateLogger());
                            //, LogEventLevel.Information, columnOptions: columnOptions)
                            
                           .WriteTo.Seq("http://localhost:5341")
                           .CreateLogger());
                           
                            
                            .WriteTo.MSSqlServer(Configuration["Serilog:ConnectionString"]
                            , Configuration["Serilog:TableName"]
                            , LogEventLevel.Information, columnOptions: columnOptions)
                            .CreateLogger());
                            */
            //services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ApplicationDbContext context)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();           
            //loggerFactory.AddNLog();
            //app.AddNLogWeb();
            //Configuration.GetConnectionString("ConnectionStrings:NLogDb")
            //LogManager.ConfigurationReloaded += updateConfig;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
                //app.UseExceptionHandler("/Home/Error");
            }


            StaticFileOptions option = new StaticFileOptions();
            FileExtensionContentTypeProvider contentTypeProvider = (FileExtensionContentTypeProvider)option.ContentTypeProvider;
           
            app.UseStaticFiles();
            app.UseIdentity();
            //loggerFactory.AddSerilog();


            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //DbInitializer.Initialize(context);
        }
        private void updateConfig(object sender, LoggingConfigurationReloadedEventArgs e)
        {
            LogManager.Configuration.Variables["connectionString"] = Configuration.GetConnectionString("NLogDb");
        }
    }
}
