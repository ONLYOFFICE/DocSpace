using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Web.Webhooks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Webhooks
{
    [Singletone]
    public class WebhookSender
    {
        private const int repeatCount = 5;

        private static readonly HttpClient httpClient = new HttpClient();
        private IServiceProvider ServiceProvider { get; }
        private ILog Log { get; }
        public WebhookSender(IOptionsMonitor<ILog> options, IServiceProvider serviceProvider)
        {
            Log = options.Get("ASC.Webhooks");
            ServiceProvider = serviceProvider;
        }
        public async Task Send(WebhookRequest webhookRequest)
        {
            var URI = webhookRequest.URI;
            var secretKey = webhookRequest.SecretKey;

            using var scope = ServiceProvider.CreateScope();
            var dbWorker = scope.ServiceProvider.GetService<DbWorker>();

            for (int i = 0; i < repeatCount; i++)
            {
                try
                {           
                    var request = new HttpRequestMessage(HttpMethod.Post, URI);
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("Secret","SHA256=" + GetSecretHash(secretKey, webhookRequest.Data));

                    request.Content = new StringContent(
                        webhookRequest.Data,
                        Encoding.UTF8,
                        "application/json");

                    var response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        dbWorker.UpdateStatus(webhookRequest.Id, ProcessStatus.Success);
                        Log.Debug("Response: " + response);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if(i == repeatCount)
                    {
                        dbWorker.UpdateStatus(webhookRequest.Id, ProcessStatus.Failed);
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
