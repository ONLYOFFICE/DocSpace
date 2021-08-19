using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
        public int RepeatCount { get; } = 5;

        private static readonly HttpClient httpClient = new HttpClient();
        private IServiceProvider ServiceProvider { get; }
        private ILog Log { get; }

        public WebhookSender(IOptionsMonitor<ILog> options, IServiceProvider serviceProvider)
        {
            Log = options.Get("ASC.Webhooks");
            ServiceProvider = serviceProvider;
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
                    var testRequest = new HttpRequestMessage(HttpMethod.Post, "http://localhost:8092/api/2.0/authentication.json");
                    string httpContent = @"{ ""userName"":""www.vna-97@mail.ru"", ""password"":""265676-333"" }";
                    testRequest.Content = new StringContent(
                        httpContent,
                        Encoding.UTF8,
                        "application/json");

                    var testResponse = httpClient.Send(testRequest, cancellationToken);

                    //HttpResponseMessage testResponseCalendar =httpClient.GetAsync("http://localhost:8092/api/2.0/calendar/info");
                    //var token = "1FR2TsR3kXu2zor7fModuf/3nBJRPI4I7LG5x3ODzTVVgFmUd3NguHEmVqDNMJkNc7MRJjeacv+UaAOlRmLcUyCBtEt54Hzd6TCADQtzUEVvl2M20tX0uYd8sftTdIn/faWV415KXFsY3E16StTZ5A==";

                    var request = new HttpRequestMessage(HttpMethod.Post, requestURI);
                    request.Headers.Add("Accept", "*/*");
                    request.Headers.Add("Secret","SHA256=" + GetSecretHash(secretKey, data));
                    //request.Headers.Add("Authorization", token);

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
                    if(i == RepeatCount)
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
