using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Sufficit.Telephony.BlazorPanel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(server =>
                    {
                        server.ConfigureEndpointDefaults(listenOptions =>
                        {
                            listenOptions.Use(next => new ClearTextHttpMultiplexingMiddleware(next).OnConnectAsync);
                        });
                    });

                    webBuilder.UseIIS();
                    webBuilder.UseIISIntegration();

                    webBuilder.UseStartup<Startup>();
                });
    }
}