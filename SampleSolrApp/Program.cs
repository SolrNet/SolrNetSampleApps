using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.OutputConsumers;
using DotNet.Testcontainers.Containers.WaitStrategies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleSolrApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await using var _ = Solr.Value;
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static readonly Lazy<TestcontainersContainer> Solr = new(() =>
        {
            Console.WriteLine("Starting Solr...");

            using var consumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());
            var container = new TestcontainersBuilder<TestcontainersContainer>()
                .WithName("SampleSolrApp-Solr-" + Guid.NewGuid())
                .WithImage("solr:8.8.2")
                .WithPortBinding(8983, assignRandomHostPort: true)
                .WithCommand("/opt/docker-solr/scripts/solr-precreate", "techproducts")
                .WithOutputConsumer(consumer)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(consumer.Stdout, "Registered new searcher"))
                .Build();
            
            container.StartAsync().Wait();
            
            Console.WriteLine("Setting up Solr data...");
            
            var exitcode = container.ExecAsync(new[] {"/bin/bash", "-xc", "/opt/solr/bin/post -out yes -host localhost -c techproducts $(find /opt/solr . | grep exampledocs | grep xml | grep -v manufacturers)"}).GetAwaiter().GetResult();
            if (exitcode != 0)
                throw new Exception("Solr data setup failed!");

            return container;
        });


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}