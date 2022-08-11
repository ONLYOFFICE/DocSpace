// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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
        private readonly RequestHistory requestHistory;
        private readonly string secretKey = "testSecretKey";
        public TestController(RequestHistory requestHistory)
        {
            this.requestHistory = requestHistory;
        }

        [HttpGet("testMethod")]
        public string GetMethod()
        {
            return "testContent";
        }

        [HttpPost("testMethod")]
        public string PostMethod()
        {
            return "testContent";
        }

        [HttpPost("SuccessRequest")]
        public async Task<IActionResult> SuccessRequest()
        {
            requestHistory.SuccessCounter++;
            await CheckSignature(secretKey);
            return Ok();
        }

        [HttpPost("FailedRequest")]
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
