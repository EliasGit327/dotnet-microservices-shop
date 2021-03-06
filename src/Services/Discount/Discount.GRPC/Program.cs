using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discount.Grpc.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Discount.Grpc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.MigrateDatabase<Program>();
            host.Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", true, true)
                            .Build();

                        var disableTLS = configuration.GetValue<bool>("disableTLS");
                        if (disableTLS)
                        {
                            Console.WriteLine("___TLS IS DISABLED!___");
                            options.ListenLocalhost(5003, o => o.Protocols = HttpProtocols.Http2);
                        }
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
