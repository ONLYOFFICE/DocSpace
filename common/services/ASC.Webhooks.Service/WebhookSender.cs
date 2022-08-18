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

using System.Text.Json.Serialization;

namespace ASC.Webhooks.Service;

[Singletone]
public class WebhookSender
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger _log;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public WebhookSender(ILoggerProvider options, IServiceScopeFactory scopeFactory, IHttpClientFactory clientFactory)
    {
        _log = options.CreateLogger("ASC.Webhooks.Core");
        _scopeFactory = scopeFactory;
        _clientFactory = clientFactory;
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = true,
            WriteIndented = true
        };
    }

    public async Task Send(WebhookRequest webhookRequest, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbWorker = scope.ServiceProvider.GetRequiredService<DbWorker>();

        var entry = await dbWorker.ReadFromJournal(webhookRequest.Id);
        var data = entry.Payload;

        var status = 0;
        string responsePayload = null;
        string responseHeaders = null;
        string requestHeaders = null;
        var delivery = DateTime.MinValue;

        try
        {
            var httpClient = _clientFactory.CreateClient("webhook");
            var request = new HttpRequestMessage(HttpMethod.Post, entry.Uri)
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("Secret", "SHA256=" + GetSecretHash(entry.SecretKey, data));
            requestHeaders = JsonSerializer.Serialize(request.Headers.ToDictionary(r => r.Key, v => v.Value), _jsonSerializerOptions);

            var response = await httpClient.SendAsync(request, cancellationToken);

            status = (int)response.StatusCode;
            responseHeaders = JsonSerializer.Serialize(response.Headers.ToDictionary(r => r.Key, v => v.Value), _jsonSerializerOptions);
            responsePayload = await response.Content.ReadAsStringAsync();
            delivery = DateTime.UtcNow;

            _log.DebugResponse(response);
        }
        catch (HttpRequestException e)
        {
            if (e.StatusCode.HasValue)
            {
                status = (int)e.StatusCode.Value;
            }
            responsePayload = e.Message;
            delivery = DateTime.UtcNow;

            _log.ErrorWithException(e);
        }
        catch (Exception e)
        {
            _log.ErrorWithException(e);
        }

        if (delivery != DateTime.MinValue)
        {
            await dbWorker.UpdateWebhookJournal(entry.Id, status, delivery, requestHeaders, responsePayload, responseHeaders);
        }
    }

    private string GetSecretHash(string secretKey, string body)
    {
        var secretBytes = Encoding.UTF8.GetBytes(secretKey);

        using (var hasher = new HMACSHA256(secretBytes))
        {
            var data = Encoding.UTF8.GetBytes(body);
            return BitConverter.ToString(hasher.ComputeHash(data));
        }
    }
}