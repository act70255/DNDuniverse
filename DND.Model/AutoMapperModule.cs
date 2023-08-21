using Autofac;
using AutoMapper;
using DND.Model.ApiContent;
using DND.Model.Entity;

namespace DND.Model
{
    public class AutoMapperModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => new MapperConfiguration(cfg =>
            {
                //Register Mapper Profile
                cfg.AddProfile<AutoMapperProfile>();
            }
            )).AsSelf().SingleInstance();

            builder.Register(c =>
            {
                var context = c.Resolve<IComponentContext>();
                var config = context.Resolve<MapperConfiguration>();
                return config.CreateMapper(context.Resolve);
            })
            .As<IMapper>()
            .InstancePerLifetimeScope();
        }
    }

    class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Creature, CreatureRequest>()
                .ReverseMap();
        }
    }
}
