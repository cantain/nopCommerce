using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.IO;

namespace Nop.Web
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(System.AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.RollingFile(Path.GetFullPath("logs/log-{Date}.txt"), Serilog.Events.LogEventLevel.Debug)
                .WriteTo.RollingFile(Path.GetFullPath("logs/error-{Date}.txt"), Serilog.Events.LogEventLevel.Warning)
                .CreateLogger();
            var host = WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => options.AddServerHeader = false)
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();

            host.Run();
        }
    }
}
