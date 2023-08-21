using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var dataAccess = System.Reflection.Assembly.GetExecutingAssembly();

            builder.RegisterAssemblyTypes(dataAccess)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            //builder.RegisterType<Service.TerrariaService>().As<Service.Interface.ITerrariaService>().InstancePerLifetimeScope();
            //builder.RegisterType<Service.CreatureService>().As<Service.Interface.ICreatureService>().InstancePerLifetimeScope();
        }
    }
}
