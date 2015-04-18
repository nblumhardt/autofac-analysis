using System;
using Autofac;
using Autofac.Configuration;
using Autofac.Features.OwnedInstances;
using Serilog;

namespace ProfiledApplication
{
    class A
    {
    }

    class B
    {
        public B(A a)
        {
        }

        public D D { get; set; }
    }

    class C
    {
        public C(B b)
        {
        }
    }

    class D
    {        
    }

    class G<T,U>
    {        
    }

    class Program
    {
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .Destructure.ToMaximumDepth(100) // Hmm
                .CreateLogger();

            var builder = new ContainerBuilder();
            builder.RegisterModule(new ConfigurationSettingsReader());
            builder.RegisterType<A>().SingleInstance();
            builder.RegisterType<B>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
            builder.RegisterType<C>().WithMetadata("M", 42).WithMetadata("N", "B!");
            builder.RegisterType<D>().SingleInstance();
            builder.RegisterGeneric(typeof (G<,>));

            using (var container = builder.Build())
            {
                using (var ls1 = container.BeginLifetimeScope())
                {
                    ls1.Resolve<C>();
                }

                System.Threading.Thread.Sleep(5000);

                using (var ls2 = container.BeginLifetimeScope())
                {
                    ls2.Resolve<C>();
                    ls2.Resolve<G<int, string>>();
                    ls2.Resolve<Owned<C>>();
                }

                Console.ReadKey(true);
            }
        }
    }
}
