using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace UIForm
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //register autofac
            var builder = new ContainerBuilder();
            //RegisterModule
            builder.RegisterModule(new DND.Model.AutoMapperModule());
            builder.RegisterModule(new DND.Repository.RepositoryModule());
            builder.RegisterModule(new DND.Domain.ServiceModule());

            //register IConfiguration
            builder.RegisterInstance(new ConfigurationBuilder()
                    .Build())
                    .As<IConfiguration>()
                    .SingleInstance();
            #region MongoDB
            builder.Register(context =>
            {
                var client = new MongoClient("mongodb://localhost:27017/DND");
                return client.GetDatabase("DND");
            }).As<IMongoDatabase>().SingleInstance();
            #endregion

            builder.RegisterType<Form1>();
            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var form = scope.Resolve<Form1>();
                Application.Run(form);
            }

        }
    }
}