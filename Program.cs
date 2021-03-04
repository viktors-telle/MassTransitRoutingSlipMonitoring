using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace MassTransitRoutingSlipMonitoring
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((host, log) =>
                {
                    if (host.HostingEnvironment.IsProduction())
                        log.MinimumLevel.Information();
                    else
                        log.MinimumLevel.Debug();

                    log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
                    log.WriteTo.Console();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}