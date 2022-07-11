using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Web.Api.Routing;

using Microsoft.AspNetCore.Mvc;

namespace ASC.Webhooks.Tests
{
    [Scope]
    [DefaultRoute]
    [ApiController]
    public class TestController : ControllerBase
    {
        private RequestHistory requestHistory;
        private string secretKey = "testSecretKey";
        public TestController(RequestHistory requestHistory)
        {
            this.requestHistory = requestHistory;
        }

        [Read("testMethod")]
        public string GetMethod()
        {
            return "testContent";
        }

        [Create("testMethod")]
        public string PostMethod()
        {
            return "testContent";
        }

        [Create("SuccessRequest")]
        public async Task<IActionResult> SuccessRequest()
        {
            requestHistory.SuccessCounter++;
            await CheckSignature(secretKey);
            return Ok();
        }

        [Create("FailedRequest")]
        public async Task<IActionResult> FailedRequest()
        {
            requestHistory.FailedCounter++;
            await CheckSignature(secretKey);
            return BadRequest();
        }

        private async Task CheckSignature(string secretKey)
        {
            if (!HttpContext.Request.Headers.ContainsKey("Secret"))
            {
                return;
            }

            string body;
            var bodyStream = HttpContext.Request.Body;
            using (var responseReader = new StreamReader(bodyStream))
            {
                body = await responseReader.ReadToEndAsync();
            }

            var receivedSignature = HttpContext.Request.Headers["Secret"].ToString().Split("=");

            string computedSignature;
            switch (receivedSignature[0])
            {
                case "sha256":
                case "SHA256":
                    var secretBytes = Encoding.UTF8.GetBytes(secretKey);
                    using (var hasher = new HMACSHA256(secretBytes))
                    {
                        var data = Encoding.UTF8.GetBytes(body);
                        computedSignature = BitConverter.ToString(hasher.ComputeHash(data));
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            requestHistory.СorrectSignature = computedSignature == receivedSignature[1];
        }
    }
}
