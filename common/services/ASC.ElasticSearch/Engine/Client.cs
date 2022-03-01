/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.ElasticSearch;

[Singletone]
public class Client
{
    public ElasticClient Instance
    {
        get
        {
            if (_client != null)
            {
                return _client;
            }

            lock (_locker)
            {
                if (_client != null)
                {
                    return _client;
                }

                using var scope = _serviceProvider.CreateScope();
                var CoreConfiguration = _serviceProvider.GetService<CoreConfiguration>();
                var launchSettings = CoreConfiguration.GetSection<Settings>(Tenant.DefaultTenant) ?? _settings;

                var uri = new Uri(string.Format("{0}://{1}:{2}", launchSettings.Scheme, launchSettings.Host, launchSettings.Port));
                var settings = new ConnectionSettings(new SingleNodeConnectionPool(uri))
                    .RequestTimeout(TimeSpan.FromMinutes(5))
                    .MaximumRetries(10)
                    .ThrowExceptions();

                if (_logger.IsTraceEnabled)
                {
                    settings.DisableDirectStreaming().PrettyJson().EnableDebugMode(r =>
                    {
                            //Log.Trace(r.DebugInformation);

                            //if (r.RequestBodyInBytes != null)
                            //{
                            //    Log.TraceFormat("Request: {0}", Encoding.UTF8.GetString(r.RequestBodyInBytes));
                            //}

                            if (r.HttpStatusCode != null && (r.HttpStatusCode == 403 || r.HttpStatusCode == 500) && r.ResponseBodyInBytes != null)
                        {
                            _logger.TraceFormat("Response: {0}", Encoding.UTF8.GetString(r.ResponseBodyInBytes));
                        }
                    });
                }

                try
                {
                    if (Ping(new ElasticClient(settings)))
                    {
                        _client = new ElasticClient(settings);

                        _client.Ingest.PutPipeline("attachments", p => p
                        .Processors(pp => pp
                            .Attachment<Attachment>(a => a.Field("document.data").TargetField("document.attachment"))
                        ));
                    }

                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }

                return _client;
            }
        }
    }

    private static volatile ElasticClient _client;
    private static readonly object _locker = new object();
    private readonly ILog _logger;
    private readonly Settings _settings;
    private readonly IServiceProvider _serviceProvider;

    public Client(IOptionsMonitor<ILog> option, IServiceProvider serviceProvider, Settings settings)
    {
        _logger = option.Get("ASC.Indexer");
        _settings = settings;
        _serviceProvider = serviceProvider;
    }

    public bool Ping()
    {
        return Ping(Instance);
    }

    private bool Ping(ElasticClient elasticClient)
    {
        if (elasticClient == null)
        {
            return false;
        }

        var result = elasticClient.Ping(new PingRequest());

        _logger.DebugFormat("Ping {0}", result.DebugInformation);

        return result.IsValid;
    }
}
