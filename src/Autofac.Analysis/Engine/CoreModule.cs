using Autofac.Analysis.Engine.Application;
using Autofac.Analysis.Engine.Session;

namespace Autofac.Analysis.Engine
{
    public sealed class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            var thisAssembly = GetType().Assembly;

            builder.RegisterType<ProfilerSession>()
                .As<IProfilerSession>()
                .InstancePerProfilerSession();

            builder.RegisterType<MessageDispatcher>()
                .As<IMessageDispatcher>();

            builder.RegisterType<ApplicationEventDispatcher>()
                .As<IApplicationEventQueue, IApplicationEventDispatcher, IApplicationEventBus>()
                .InstancePerProfilerSession();

            builder.RegisterAssemblyTypes(thisAssembly)
                .AsClosedTypesOf(typeof(IUpdateHandler<>))
                .InstancePerProfilerSession();

            builder.RegisterAssemblyTypes(thisAssembly)
                .AsClosedTypesOf(typeof(IApplicationEventHandler<>))
                .InstancePerProfilerSession();

            builder.RegisterGeneric(typeof(ActiveItemRepository<>))
                .As(typeof (IActiveItemRepository<>))
                .InstancePerProfilerSession();
        }
    }
}
