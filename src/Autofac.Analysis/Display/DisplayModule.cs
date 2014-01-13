namespace Autofac.Analysis.Display
{
    class DisplayModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<EventWriter>().SingleInstance();
        }
    }
}
