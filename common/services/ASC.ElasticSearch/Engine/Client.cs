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

namespace ASC.ElasticSearch;

[Singletone]
public class Client
{
    private static volatile ElasticClient _client;
    private static readonly object _locker = new object();
    private readonly ILogger _logger;
    private readonly Settings _settings;

    public Client(ILoggerProvider option, Settings settings)
    {
        _logger = option.CreateLogger("ASC.Indexer");
        _settings = settings;
    }

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

                if (string.IsNullOrEmpty(_settings.Scheme) || string.IsNullOrEmpty(_settings.Host) || !_settings.Port.HasValue)
                {
                    return null;
                }

                var uri = new Uri(string.Format("{0}://{1}:{2}", _settings.Scheme, _settings.Host, _settings.Port));
                var settings = new ConnectionSettings(new SingleNodeConnectionPool(uri))
                    .RequestTimeout(TimeSpan.FromMinutes(5))
                    .MaximumRetries(10)
                    .ThrowExceptions();

                if (_settings.Authentication != null)
                {
                    settings.BasicAuthentication(_settings.Authentication.Username, _settings.Authentication.Password);
                }

                if (_settings.ApiKey != null)
                {
                    settings.ApiKeyAuthentication(_settings.ApiKey.Id, _settings.ApiKey.Value);
                }

                if (_logger.IsEnabled(LogLevel.Trace))
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
                            _logger.TraceResponse(Encoding.UTF8.GetString(r.ResponseBodyInBytes));
                        }
                    });
                }

                try
                {
                    var client = new ElasticClient(settings);
                    if (Ping(client))
                    {
                        _client = client;

                        _client.Ingest.PutPipeline("attachments", p => p
                        .Processors(pp => pp
                            .Attachment<Attachment>(a => a.Field("document.data").TargetField("document.attachment"))
                        ));
                    }

                }
                catch (Exception e)
                {
                    _logger.ErrorClient(e);
                }

                return _client;
            }
        }
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

        _logger.DebugPing(result.DebugInformation);

        return result.IsValid;
    }
}
