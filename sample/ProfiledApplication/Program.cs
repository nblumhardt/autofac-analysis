using System;
using Autofac;
using Autofac.Analysis;
using Autofac.Features.OwnedInstances;
using Serilog;

// ReSharper disable UnusedTypeParameter, UnusedParameter.Local

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

    class G<T1,T2>
    {        
    }

    class Program
    {
        static void Main()
        {
            using (var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .Destructure.ToMaximumDepth(100) // Override the default limit of 5
                .CreateLogger())
            {
                var builder = new ContainerBuilder();
                builder.RegisterModule(new AnalysisModule(logger));
                builder.RegisterType<A>().SingleInstance();
                builder.RegisterType<B>().PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
                builder.RegisterType<C>().WithMetadata("M", 42).WithMetadata("N", "B!");
                builder.RegisterType<D>().SingleInstance();
                builder.RegisterGeneric(typeof(G<,>));

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
}
