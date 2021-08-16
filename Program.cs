using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace benchmark_akka_cluster
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {

            var port = Int32.Parse(Environment.GetEnvironmentVariable("PORT") ?? "5000");
            string url = String.Concat("http://0.0.0.0:", port);

            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // this will keep your other end points settings such as --urls parameter
                    webBuilder.ConfigureKestrel((options) =>
                    {

                        // trying to use Http1AndHttp2 causes http2 connections to fail with invalid protocol error
                        // according to Microsoft dual http version mode not supported in unencrypted scenario: https://docs.microsoft.com/en-us/aspnet/core/grpc/troubleshoot?view=aspnetcore-3.0
                        //options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http1AndHttp2);
                        options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http1AndHttp2);


                    });

                    webBuilder.UseStartup<Startup>().UseUrls(url);

                });
        }
    }
}
