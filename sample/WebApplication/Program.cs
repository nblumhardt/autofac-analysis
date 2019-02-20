using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace WebApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();

            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The web host could not be built");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices(services => services.AddAutofac())
                .UseStartup<Startup>();
    }
}
