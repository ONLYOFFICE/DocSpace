﻿namespace ASC.Webhooks.Service
{
    [Singletone]
    public class WebhookSender
    {
        public int? RepeatCount { get; }
        private static readonly HttpClient httpClient = new HttpClient();
        private IServiceProvider ServiceProvider { get; }
        private ILog Log { get; }

        public WebhookSender(IOptionsMonitor<ILog> options, IServiceProvider serviceProvider, Settings settings)
        {
            Log = options.Get("ASC.Webhooks.Core");
            ServiceProvider = serviceProvider;
            RepeatCount = settings.RepeatCount;
        }

        public async Task Send(WebhookRequest webhookRequest, CancellationToken cancellationToken)
        {
            using var scope = ServiceProvider.CreateScope();
            var dbWorker = scope.ServiceProvider.GetService<DbWorker>();

            var entry = dbWorker.ReadFromJournal(webhookRequest.Id);
            var id = entry.Id;
            var requestURI = entry.Uri;
            var secretKey = entry.SecretKey;
            var data = entry.Payload;

            HttpResponseMessage response = new HttpResponseMessage();
            HttpRequestMessage request = new HttpRequestMessage();

            for (int i = 0; i < RepeatCount; i++)
            {
                try
                {
                    request = new HttpRequestMessage(HttpMethod.Post, requestURI);
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("Secret", "SHA256=" + GetSecretHash(secretKey, data));

                    request.Content = new StringContent(
                        data,
                        Encoding.UTF8,
                        "application/json");

                    response = await httpClient.SendAsync(request, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        UpdateDb(dbWorker, id, response, request, ProcessStatus.Success);
                        Log.Debug("Response: " + response);
                        break;
                    }
                    else if (i == RepeatCount - 1)
                    {
                        UpdateDb(dbWorker, id, response, request, ProcessStatus.Failed);
                        Log.Debug("Response: " + response);
                    }
                }
                catch (Exception ex)
                {
                    if (i == RepeatCount - 1)
                    {
                        UpdateDb(dbWorker, id, response, request, ProcessStatus.Failed);
                    }

                    Log.Error(ex.Message);
                    continue;
                }
            }
        }

        private string GetSecretHash(string secretKey, string body)
        {
            string computedSignature;
            var secretBytes = Encoding.UTF8.GetBytes(secretKey);

            using (var hasher = new HMACSHA256(secretBytes))
            {
                var data = Encoding.UTF8.GetBytes(body);
                computedSignature = BitConverter.ToString(hasher.ComputeHash(data));
            }

            return computedSignature;
        }

        private void UpdateDb(DbWorker dbWorker, int id, HttpResponseMessage response, HttpRequestMessage request, ProcessStatus status)
        {
            var responseHeaders = JsonSerializer.Serialize(response.Headers.ToDictionary(r => r.Key, v => v.Value));
            var requestHeaders = JsonSerializer.Serialize(request.Headers.ToDictionary(r => r.Key , v => v.Value));
            string responsePayload;

            using (var streamReader = new StreamReader(response.Content.ReadAsStream()))
            {
                var responseContent = streamReader.ReadToEnd();
                responsePayload = JsonSerializer.Serialize(responseContent);
            };

            dbWorker.UpdateWebhookJournal(id, status, responsePayload, responseHeaders, requestHeaders);
        }
    }
}
