using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.Extensions.Hosting;
using ApiHost.Core.Setup.Modules;
using MongoDB.Driver;

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