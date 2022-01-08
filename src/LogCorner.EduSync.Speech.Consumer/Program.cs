using LogCorner.EduSync.Notification.Common;
using LogCorner.EduSync.Speech.Command.SharedKernel;
using LogCorner.EduSync.Speech.ElasticSearch;
using LogCorner.EduSync.Speech.Projection;
using LogCorner.EduSync.Speech.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace LogCorner.EduSync.Speech.Consumer
{
    public class Program
    {
        private static IConfigurationRoot _configuration;

        public static void Main(string[] args)
        {
            ConfigureEnvironment();
            CreateHostBuilder(args).Build().Run();
        }

        private static void ConfigureEnvironment()
        {
            string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environment}.json", false)
                .AddEnvironmentVariables();
            _configuration = builder.Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var kafkaUrl = _configuration["kafkaUrl"];
                    var hubUrl = _configuration["hubUrl"];
                    var elasticSearchUrl = _configuration["elasticSearchUrl"];
                    services.AddSingleton<IClusterManager, KafkaClusterManager>(
                        ctx => new KafkaClusterManager(_configuration)
                        );
                    services.AddServiceBus(kafkaUrl);
                    services.AddSingleton<IConsumerService, ConsumerService>();
                    services.AddHostedService<ConsumerHostedService>();
                    services.AddSignalRServices($"{hubUrl}?clientName=LogCorner.EduSync.Speech.Consumer", _configuration);
                    services.AddSharedKernel();

                    services.AddElasticSearch<SpeechProjection>(elasticSearchUrl, "speech");
                });
    }
}