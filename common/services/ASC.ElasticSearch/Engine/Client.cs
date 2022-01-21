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


using System;
using System.Text;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.ElasticSearch.Service;

using Elasticsearch.Net;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Nest;

namespace ASC.ElasticSearch
{
    [Singletone]
    public class Client
    {
        private static volatile ElasticClient client;
        private static readonly object Locker = new object();

        private ILog Log { get; }
        private Settings Settings { get; }
        private IServiceProvider ServiceProvider { get; }

        public Client(IOptionsMonitor<ILog> option, IServiceProvider serviceProvider, Settings settings)
        {
            Log = option.Get("ASC.Indexer");
            Settings = settings;
            ServiceProvider = serviceProvider;
        }

        public ElasticClient Instance
        {
            get
            {
                if (client != null) return client;

                lock (Locker)
                {
                    if (client != null) return client;

                    using var scope = ServiceProvider.CreateScope();
                    var CoreConfiguration = ServiceProvider.GetService<CoreConfiguration>();
                    var launchSettings = CoreConfiguration.GetSection<Settings>(Tenant.DEFAULT_TENANT) ?? Settings;

                    var uri = new Uri(string.Format("{0}://{1}:{2}", launchSettings.Scheme, launchSettings.Host, launchSettings.Port));
                    var settings = new ConnectionSettings(new SingleNodeConnectionPool(uri))
                        .RequestTimeout(TimeSpan.FromMinutes(5))
                        .MaximumRetries(10)
                        .ThrowExceptions();

                    if (Log.IsTraceEnabled)
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
                                Log.TraceFormat("Response: {0}", Encoding.UTF8.GetString(r.ResponseBodyInBytes));
                            }
                        });
                    }

                    try
                    {
                        if (Ping(new ElasticClient(settings)))
                        {
                            client = new ElasticClient(settings);

                            client.Ingest.PutPipeline("attachments", p => p
                            .Processors(pp => pp
                                .Attachment<Attachment>(a => a.Field("document.data").TargetField("document.attachment"))
                            ));
                        }

                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }



                    return client;
                }
            }
        }

        public bool Ping()
        {
            return Ping(Instance);
        }

        private bool Ping(ElasticClient elasticClient)
        {
            if (elasticClient == null) return false;

            var result = elasticClient.Ping(new PingRequest());

            Log.DebugFormat("Ping {0}", result.DebugInformation);

            return result.IsValid;
        }
    }
}
