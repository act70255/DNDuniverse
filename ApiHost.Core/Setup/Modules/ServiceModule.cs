using Autofac;

namespace ApiHost.Core.Setup.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var dataAccess = System.Reflection.Assembly.GetExecutingAssembly();

            //builder.RegisterAssemblyTypes(dataAccess)
            //    .Where(t => t.Name.EndsWith("Service"))
            //    .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(dataAccess)
            .Where(t => t.Name.EndsWith("Controller"))
            .AsImplementedInterfaces();
        }
    }
}
