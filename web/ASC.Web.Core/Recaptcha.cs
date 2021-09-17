using System;
using System.IO;
using System.Net;
using System.Security.Authentication;

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

                var webRequest = (HttpWebRequest)WebRequest.Create(SetupInfo.RecaptchaVerifyUrl);
                webRequest.Method = WebRequestMethods.Http.Post;
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = data.Length;
                using (var writer = new StreamWriter(webRequest.GetRequestStream()))
                {
                    writer.Write(data);
                }

                using (var webResponse = webRequest.GetResponse())
                using (var reader = new StreamReader(webResponse.GetResponseStream()))
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
