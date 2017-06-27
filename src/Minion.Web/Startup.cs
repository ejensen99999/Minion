using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minion.Configuration;
using Minion.Core.ServiceModel;
using Minion.Inject;
using Minion.Inject.Middleware;
using Minion.Web.TestObjs;
using System;
using Minion.Web.Domain;

namespace Minion.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
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
            services
                .AddDistributedMemoryCache()
                .AddMinionInject()
                .AddConfiguration<Settings>(Configuration)
                .AddConfiguration<ActiveDirectorySettings>(Configuration)
                .AddConfiguration<BusSettings>(Configuration)
                .AddConfiguration<CacheSettings>(Configuration)
                .AddConfiguration<ConnectionSettings>(Configuration)
                .AddConfiguration<CronSettings>(Configuration)
                .AddConfiguration<EmailSettings>(Configuration)
                .AddConfiguration<FaxSettings>(Configuration)
                .AddConfiguration<FileSettings>(Configuration)
                .AddConfiguration<FTPSettings>(Configuration)
                .AddConfiguration<PrintSettings>(Configuration)
                .AddSingleton<ICoreConfiguration, CoreConfiguration>()
                .AddTransient<IBusinessLogic, BusinessLogic>()
                .AddTransient<IRespository, Repository>()
                .AddScoped<ITest, Test>()
                .AddService(typeof(ApiService<>), typeof(ApiService<>), ServiceLifetime.Transient, null, null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory, IOptionsMonitor<Settings> monitor)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app
                .UseMinionInject()
                .UseMvc();

            monitor.OnChange(
                vals =>
                {
                    loggerFactory
                        .CreateLogger<IOptionsMonitor<Settings>>()
                        .LogDebug($"Config changed: {string.Join(", ", vals)}");
                });
        }
    }
}
