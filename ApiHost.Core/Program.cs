using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.Extensions.Hosting;
using ApiHost.Core.Setup.Modules;
using MongoDB.Driver;
using Serilog;
using Autofac.Integration.WebApi;
using Serilog.Events;

namespace ApiHost.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule(new ServiceModule());
                builder.RegisterModule(new DND.Model.AutoMapperModule());
                builder.RegisterModule(new DND.Repository.RepositoryModule());
                builder.RegisterModule(new DND.Domain.ServiceModule());
            });

            #region MongoDB
            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var mongoDbSettings = builder.Configuration.GetSection("MongoDB");
            var mongoDbConnectionString = mongoDbSettings["ConnectionString"];
            var mongoDbDatabaseName = mongoDbSettings["DatabaseName"];
            builder.Services.AddSingleton<IMongoDatabase>(s => new MongoClient(mongoDbConnectionString).GetDatabase(mongoDbDatabaseName));
            #endregion

            #region Log
            builder.Services.AddLogging(builder => builder
                .AddSerilog(new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            //.Filter.ByIncludingOnly(f => f.Level == LogEventLevel.Debug || f.Level == LogEventLevel.Error)
                            .WriteTo.Console()
                            .WriteTo.File("logs\\Debug.txt", LogEventLevel.Debug, rollingInterval: RollingInterval.Day)
                            .WriteTo.File("logs\\Info.txt", LogEventLevel.Information, rollingInterval: RollingInterval.Day)
                            .CreateLogger()));
            #endregion
            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHealthChecks();

            var app = builder.Build();
            
            app.MapControllers();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.MapHealthChecks("/health");
            app.UseAuthorization();
            app.MapControllers();
            
            app.Run();
        }
    }
}