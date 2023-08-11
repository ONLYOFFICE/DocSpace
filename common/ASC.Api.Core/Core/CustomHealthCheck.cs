// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Api.Core.Core;

public static class CustomHealthCheck
{
    public static bool Running { get; set;}

    static CustomHealthCheck()
    {
        Running = true;
    }
    
    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => Running ? HealthCheckResult.Healthy()
                                    : HealthCheckResult.Unhealthy())
                 .AddDatabase(configuration)
                 .AddDistibutedCache(configuration)
                 .AddMessageQueue(configuration)
                 .AddSearch(configuration);

        return services;
    }

    public static IHealthChecksBuilder AddDistibutedCache(
        this IHealthChecksBuilder hcBuilder, IConfiguration configuration)
    {
        var redisConfiguration = configuration.GetSection("Redis").Get<RedisConfiguration>();

        if (redisConfiguration != null)
        {
            //  https://github.com/imperugo/StackExchange.Redis.Extensions/issues/513
            if (configuration.GetSection("Redis").GetValue<string>("User") != null)
            {
                redisConfiguration.ConfigurationOptions.User = configuration.GetSection("Redis").GetValue<string>("User");
            }


            hcBuilder.AddRedis(redisConfiguration.ConfigurationOptions.ToString(),
                               name: "redis",
                               tags: new string[] { "redis", "services" },
                               timeout: new TimeSpan(0, 0, 15));
        }

        return hcBuilder;
    }



    public static IHealthChecksBuilder AddSearch(
   this IHealthChecksBuilder hcBuilder, IConfiguration configuration)
    {
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
                                          tags: new string[] { "elasticsearch", "services" },
                                          timeout: new TimeSpan(0, 0, 15));
            }
        }

        return hcBuilder;
    }

    public static IHealthChecksBuilder AddDatabase(
       this IHealthChecksBuilder hcBuilder, IConfiguration configuration)
    {
        var configurationExtension = new ConfigurationExtension(configuration);

        var connectionString = configurationExtension.GetConnectionStrings("default");

        if (string.Equals(connectionString.ProviderName, "MySql.Data.MySqlClient"))
        {
            hcBuilder.AddMySql(connectionString.ConnectionString,
                               name: "mysqldb",
                               tags: new string[] { "mysqldb", "services" },
                               timeout: new TimeSpan(0, 0, 15));
        }
        else if (string.Equals(connectionString.ProviderName, "Npgsql"))
        {
            hcBuilder.AddNpgSql(connectionString.ConnectionString,
                               name: "postgredb",
                               tags: new string[] { "postgredb", "services" },
                               timeout: new TimeSpan(0, 0, 15));
        }

        return hcBuilder;
    }


    public static IHealthChecksBuilder AddMessageQueue(
               this IHealthChecksBuilder hcBuilder, IConfiguration configuration)
    {
        var rabbitMQConfiguration = configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();

        if (rabbitMQConfiguration != null)
        {
            hcBuilder.AddRabbitMQ(x => x.ConnectionFactory = rabbitMQConfiguration.GetConnectionFactory(),
                              name: "rabbitMQ",
                              tags: new string[] { "rabbitMQ", "services" },
                              timeout: new TimeSpan(0, 0, 15));
        }
        else
        {
            var configurationExtension = new ConfigurationExtension(configuration);
            var kafkaSettings = configurationExtension.GetSetting<KafkaSettings>("kafka");

            if (kafkaSettings != null && !string.IsNullOrEmpty(kafkaSettings.BootstrapServers))
            {
                var clientConfig = new ClientConfig { BootstrapServers = kafkaSettings.BootstrapServers };

                hcBuilder.AddKafka(new ProducerConfig(clientConfig),
                               name: "kafka",
                               tags: new string[] { "kafka", "services" },
                               timeout: new TimeSpan(0, 0, 15)
                               );

            }
        }

        return hcBuilder;
    }


}