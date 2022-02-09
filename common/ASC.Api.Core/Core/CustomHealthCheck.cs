﻿namespace ASC.Api.Core.Core
{
    public static class CustomHealthCheck
    {
        public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            var configurationExtension = new ConfigurationExtension(configuration);

            var connectionString = configurationExtension.GetConnectionStrings("default");

            if (String.Compare(connectionString.ProviderName, "MySql.Data.MySqlClient") == 0)
            {
                hcBuilder.AddMySql(connectionString.ConnectionString,
                                   name: "mysqldb",
                                   tags: new string[] { "mysqldb" }
                                  );
            }

            if (String.Compare(connectionString.ProviderName, "Npgsql") == 0)
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