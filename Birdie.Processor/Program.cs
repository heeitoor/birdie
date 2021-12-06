using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.IO;

namespace Birdie.Processor
{
    public class Program
    {
        public static string ENV { get => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"); }

        public static string SECRET_KEY { get => "secrets"; }

        public static string PORT { get => Environment.GetEnvironmentVariable("PORT"); }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{ENV}.json", optional: true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = configurationBuilder.Build();

            string passwordStoreUrl = $"{configuration["RedisPass"]}@{configuration["RedisHost"]}";

            using (var redisManager = new RedisManagerPool(passwordStoreUrl))
            {
                using (var redisClient = redisManager.GetClient())
                {
                    var secretData = redisClient.Get<Dictionary<string, string>>(SECRET_KEY);

                    var secretConfiguration = new ConfigurationBuilder()
                        .AddInMemoryCollection(secretData)
                        .Build();

                    configuration = configurationBuilder
                        .AddConfiguration(secretConfiguration)
                        .Build();
                }
            }

            IWebHostBuilder builder = new WebHostBuilder();

            if (!string.IsNullOrEmpty(PORT))
            {
                builder = builder.UseUrls($"http://+:{PORT}");
            }
            else
            {
                builder = builder.UseUrls($"http://0.0.0.0:6000");
            }

            return builder
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .UseConfiguration(configuration)
                .UseKestrel()
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory());
        }
    }
}
