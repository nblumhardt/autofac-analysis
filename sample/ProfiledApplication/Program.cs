using System;
using Autofac;
using Autofac.Configuration;
using Autofac.Features.OwnedInstances;

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
            Console.WriteLine("Started.");

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
                    var o1 = ls1.Resolve<C>();
                    Console.WriteLine("Resolved a {0}.", o1);
                }

                Console.WriteLine("Taking a nap...");
                System.Threading.Thread.Sleep(5000);

                using (var ls2 = container.BeginLifetimeScope())
                {
                    var o = ls2.Resolve<C>();
                    Console.WriteLine("Resolved a {0}.", o);

                    var g = ls2.Resolve<G<int, string>>();
                    Console.WriteLine("Resolved a {0}.", g);

                    var ov = ls2.Resolve<Owned<C>>();
                    Console.WriteLine("Resolved an {0}", ov);
                }
            }

            Console.WriteLine("Done. Press any key...");
            Console.ReadKey(true);
        }
    }
}
