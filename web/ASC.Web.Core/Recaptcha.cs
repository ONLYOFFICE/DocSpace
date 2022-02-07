using System;
using System.IO;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;

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

        public Recaptcha(SetupInfo setupInfo)
        {
            SetupInfo = setupInfo;
        }


        public bool ValidateRecaptcha(string response, string ip)
        {
            try
            {
                var data = string.Format("secret={0}&remoteip={1}&response={2}", SetupInfo.RecaptchaPrivateKey, ip, response);

                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(SetupInfo.RecaptchaVerifyUrl);
                request.Method = HttpMethod.Post;
                request.Content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                using var httpClient = new HttpClient();
                using var httpClientResponse = httpClient.Send(request);
                using (var reader = new StreamReader(httpClientResponse.Content.ReadAsStream()))
                {
                    var resp = reader.ReadToEnd();
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
