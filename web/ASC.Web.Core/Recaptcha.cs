using System;
using System.IO;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Web.Studio.Core;

using Newtonsoft.Json.Linq;

namespace ASC.Web.Core
{
    public class RecaptchaException : InvalidCredentialException
    {
        public RecaptchaException()
        {
        }

        public RecaptchaException(string message)
            : base(message)
        {
        }
    }

    [Scope]
    public class Recaptcha
    {
        private SetupInfo SetupInfo { get; }
        private IHttpClientFactory ClientFactory { get; }

        public Recaptcha(SetupInfo setupInfo, IHttpClientFactory clientFactory)
        {
            SetupInfo = setupInfo;
            ClientFactory = clientFactory;
        }

        public async Task<bool> ValidateRecaptchaAsync(string response, string ip)
        {
            try
            {
                var data = $"secret={SetupInfo.RecaptchaPrivateKey}&remoteip={ip}&response={response}";

                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(SetupInfo.RecaptchaVerifyUrl);
                request.Method = HttpMethod.Post;
                request.Content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                var httpClient = ClientFactory.CreateClient();
                using var httpClientResponse = await httpClient.SendAsync(request);
                using (var reader = new StreamReader(await httpClientResponse.Content.ReadAsStreamAsync()))
                {
                    var resp = await reader.ReadToEndAsync();
                    var resObj = JObject.Parse(resp);

                    if (resObj["success"] != null && resObj.Value<bool>("success"))
                    {
                        return true;
                    }
                    if (resObj["error-codes"] != null && resObj["error-codes"].HasValues)
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
            }

            return false;
        }
    }
}
