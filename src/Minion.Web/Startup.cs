using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Minion.Configuration;
using Minion.Ioc;
using Minion.Ioc.Middleware;
using Minion.Web.Models;
using Minion.Web.TestObjs;

namespace Minion.Web
{
    public class Startup
    {
        private readonly Container _container;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

           _container = ContainerManager.GetContainer();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddMinionActivator(_container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMinionThreadedIoc(_container);
            app.UseMvc();

            InitializeContainer(app, _container);
        }

        private void InitializeContainer(IApplicationBuilder app, Container container)
        {
            // Add application presentation components:
            container
                .RegisterComponents(app)
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
                .AddTransient<IRespository, Respository>()
                .AddThreadAsync<ITest, Test>();
        }
    }
}
