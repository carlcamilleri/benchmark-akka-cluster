using Akka.Actor;
using Akka.Bootstrap.Docker;
using Akka.Cluster;
using Akka.Cluster.Sharding;
using Akka.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace benchmark_akka_cluster
{
    public class Startup
    {
        private readonly IWebHostEnvironment _currentEnvironment;
        private IActorRef _shardRegion;
        private IActorRef _actorRef;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _currentEnvironment = env;

        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            AddActorSystem(services, _currentEnvironment.ContentRootPath);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.MaxServicePoints = int.MaxValue;
            ServicePointManager.Expect100Continue = false;

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("healthy");
                });

                endpoints.MapGet("/{entityId}", async context =>
                {
                    var entityId = context.Request.RouteValues["entityId"].ToString();


                    var resPong = await _shardRegion.Ask<MsgPong>(new ShardEnvelope<MsgPing>(entityId, new MsgPing()));
                    //var resPong = await _actorRef.Ask<MsgPong>(new ShardEnvelope<MsgPing>(entityId, new MsgPing()));
                    await context.Response.WriteAsync(resPong.Message);
                });
            });
        }


        public void AddActorSystem(IServiceCollection services, string contentRootPath)
        {

            var configRoot = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
            



            var config = ConfigurationFactory.ParseString(File.ReadAllText("akka.hocon")).BootstrapFromDocker();




            // Diagnostic logging
            Console.WriteLine($"[Docker-Bootstrap] IP={config.GetString("akka.remote.dot-netty.tcp.public-hostname")}");
            Console.WriteLine($"[Docker-Bootstrap] PORT={config.GetString("akka.remote.dot-netty.tcp.port")}");
            var seeds = string.Join(",", config.GetStringList("akka.cluster.seed-nodes").Select(s => $"\"{s}\""));
            Console.WriteLine($"[Docker-Bootstrap] SEEDS=[{seeds}]");

             

            ActorSystem system = ActorSystem.Create("ping-pong-cluster-system", config);
            var sharding = ClusterSharding.Get(system);

            int maxNumberOfNodes = 3;
            _shardRegion = sharding.Start(
                    typeName: nameof(PingPongActor),
                    entityProps: Props.Create<PingPongActor>(), 
                    settings: ClusterShardingSettings.Create(system),
                    messageExtractor: new MessageExtractor(maxNumberOfNodes * 10)
                );

            _actorRef = system.ActorOf(Props.Create<PingPongActor>(), "PingPongActor");
 
            var cluster = Cluster.Get(system);             

            cluster.RegisterOnMemberRemoved(async () =>
            {
                await CoordinatedShutdown.Get(system).Run(CoordinatedShutdown.ClrExitReason.Instance);
                Environment.Exit(100);

            });             
        }

    }
}
