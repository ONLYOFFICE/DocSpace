using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Web.Webhooks;
using ASC.Webhooks.Dao.Models;

using Microsoft.Extensions.Options;

namespace ASC.Webhooks
{
    [Scope]
    public class WebhookSender
    {
        private const int repeatCount = 5;

        private static readonly HttpClient httpClient = new HttpClient();
        private ILog Log { get; }
        private DbWorker DbWorker { get; }
        public WebhookSender(IOptionsMonitor<ILog> options, DbWorker dbWorker)
        {
            DbWorker = dbWorker;
            Log = options.Get("ASC.Webhooks");
        }
        public async Task Send(WebhookRequest webhookRequest)
        {
            var URI = webhookRequest.URI;
            var secretKey = webhookRequest.SecretKey;

            for (int i = 0; i < repeatCount; i++)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, URI);
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("Secret","SHA256=" + GetSecretHash(secretKey, webhookRequest.Data));//*retry

                    request.Content = new StringContent(
                        webhookRequest.Data,
                        Encoding.UTF8,
                        "application/json");

                    var response = await httpClient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        DbWorker.UpdateStatus(webhookRequest.Id, ProcessStatus.Success);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if(i == repeatCount)
                    {
                        DbWorker.UpdateStatus(webhookRequest.Id, ProcessStatus.Failed);
                    }

                    Log.Error("ERROR: " + ex.Message);
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
