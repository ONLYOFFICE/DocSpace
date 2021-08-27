using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Web.Webhooks;
using ASC.Webhooks.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Webhooks.Service
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
            var data = entry.Data;


            for (int i = 0; i < RepeatCount; i++)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, requestURI);
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("Secret", "SHA256=" + GetSecretHash(secretKey, data));

                    request.Content = new StringContent(
                        data,
                        Encoding.UTF8,
                        "application/json");

                    var response = await httpClient.SendAsync(request, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        dbWorker.UpdateStatus(id, ProcessStatus.Success);
                        Log.Debug("Response: " + response);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (i == RepeatCount)
                    {
                        dbWorker.UpdateStatus(id, ProcessStatus.Failed);
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
    }
}
