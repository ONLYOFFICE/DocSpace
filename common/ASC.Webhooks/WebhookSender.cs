using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;

using ASC.Common;
using ASC.Core;

namespace ASC.Webhooks
{
    [Scope]
    public class WebhookSender
    {
        private DbWorker DbWorker { get; }
        private TenantManager TenantManager { get; }
        public WebhookSender(DbWorker dbWorker, TenantManager tenantManager)
        {
            DbWorker = dbWorker;
            TenantManager = tenantManager;
        }

        public bool Send(EventName eventName)
        {
            var tenantId = TenantManager.GetCurrentTenant().TenantId;
            var requestURIList = DbWorker.GetWebhookUri(tenantId);
            foreach (var requestUrl in requestURIList)
            {
                try
                {
                    var webRequest = WebRequest.Create(requestUrl);
                    webRequest.Method = "POST";
                    webRequest.ContentType = "application/json";
                    webRequest.Headers.Add("Secret", GetSecret("secretKey"));

                    var data = JsonSerializer.Serialize(eventName);

                    var encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(data);
                    webRequest.ContentLength = bytes.Length;
                    using (var writeStream = webRequest.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }

                    using (var webResponse = webRequest.GetResponse())
                    using (var reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string responseFromServer = reader.ReadToEnd();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
            return false;
        }

        private string GetSecret(string secretKey)
        {
            return "";
        }
    }
}
