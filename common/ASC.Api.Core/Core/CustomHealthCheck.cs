using System;
using System.Linq;

using ASC.Common.Caching;
using ASC.Common.Utils;

using Confluent.Kafka;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ASC.Api.Core.Core
{
    public static class CustomHealthCheck
    {
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            var configurationExtension = new ConfigurationExtension(configuration);

            var connectionString = configurationExtension.GetConnectionStrings("default");

            if (string.Equals(connectionString.ProviderName, "MySql.Data.MySqlClient"))
            {
                hcBuilder.AddMySql(connectionString.ConnectionString,
                                   name: "mysqldb",
                                   tags: new string[] { "mysqldb" }
                                  );
            }

            if (string.Equals(connectionString.ProviderName, "Npgsql"))
            {
                hcBuilder.AddNpgSql(connectionString.ConnectionString,
                                   name: "postgredb",
                                   tags: new string[] { "postgredb" }
                                  );
            }

            var kafkaSettings = configurationExtension.GetSetting<KafkaSettings>("kafka");

            if (kafkaSettings != null && !string.IsNullOrEmpty(kafkaSettings.BootstrapServers))
            {
                var clientConfig = new ClientConfig { BootstrapServers = kafkaSettings.BootstrapServers };

                hcBuilder.AddKafka(new ProducerConfig(clientConfig),
                               name: "kafka",
                               tags: new string[] { "kafka" });

            }


            var elasticSettings = configuration.GetSection("elastic");

            if (elasticSettings != null && elasticSettings.GetChildren().Any())
            {
                var host = elasticSettings.GetSection("Host").Value ?? "localhost";
                var scheme = elasticSettings.GetSection("Scheme").Value ?? "http";
                var port = elasticSettings.GetSection("Port").Value ?? "9200";
                var elasticSearchUri = $"{scheme}://{host}:{port}";

                if (Uri.IsWellFormedUriString(elasticSearchUri, UriKind.Absolute))
                {
                    hcBuilder.AddElasticsearch(elasticSearchUri,
                                               name: "elasticsearch",
                                               tags: new string[] { "elasticsearch" });
                }
            }

            return services;
        }
    }
}