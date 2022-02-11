/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common;
using ASC.Web.Studio.Core;

using Newtonsoft.Json.Linq;

namespace ASC.Web.Files.Utils
{
    public class MailMergeTask : IDisposable
    {
        internal const string MessageBodyFormat = "id={0}&from={1}&subject={2}&to%5B%5D={3}&body={4}&mimeReplyToId=";

        public string From { get; set; }
        public string Subject { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public string AttachTitle { get; set; }
        public Stream Attach { get; set; }
        public int MessageId { get; set; }
        public string StreamId { get; set; }

        public MailMergeTask()
        {
            MessageId = 0;
        }

        public void Dispose()
        {
            if (Attach != null)
                Attach.Dispose();
        }
    }

    [Scope]
    public class MailMergeTaskRunner
    {
        private SetupInfo SetupInfo { get; }
        private SecurityContext SecurityContext { get; }
        private BaseCommonLinkUtility BaseCommonLinkUtility { get; }

        //private ApiServer _apiServer;

        //protected ApiServer Api
        //{
        //    get { return _apiServer ?? (_apiServer = new ApiServer()); }
        //}

        public MailMergeTaskRunner(SetupInfo setupInfo, SecurityContext securityContext, BaseCommonLinkUtility baseCommonLinkUtility)
        {
            SetupInfo = setupInfo;
            SecurityContext = securityContext;
            BaseCommonLinkUtility = baseCommonLinkUtility;
        }

        public async Task<string> RunAsync(MailMergeTask mailMergeTask, IHttpClientFactory clientFactory)
        {
            if (string.IsNullOrEmpty(mailMergeTask.From)) throw new ArgumentException("From is null");
            if (string.IsNullOrEmpty(mailMergeTask.To)) throw new ArgumentException("To is null");

            CreateDraftMail(mailMergeTask);

            var bodySendAttach = await AttachToMailAsync(mailMergeTask, clientFactory);

            return SendMail(mailMergeTask, bodySendAttach);
        }

        private void CreateDraftMail(MailMergeTask mailMergeTask)
        {
            //var apiUrlCreate = $"{SetupInfo.WebApiBaseUrl}mail/drafts/save.json";
            //var bodyCreate =
            //    string.Format(
            //        MailMergeTask.MessageBodyFormat,
            //        mailMergeTask.MessageId,
            //        HttpUtility.UrlEncode(mailMergeTask.From),
            //        HttpUtility.UrlEncode(mailMergeTask.Subject),
            //        HttpUtility.UrlEncode(mailMergeTask.To),
            //        HttpUtility.UrlEncode(mailMergeTask.Message));
            const string responseCreateString = null; //TODO: Encoding.UTF8.GetString(Convert.FromBase64String(Api.GetApiResponse(apiUrlCreate, "PUT", bodyCreate)));
            var responseCreate = JObject.Parse(responseCreateString);

            if (responseCreate["statusCode"].Value<int>() != (int)HttpStatusCode.OK)
            {
                throw new Exception("Create draft failed: " + responseCreate["error"]["message"].Value<string>());
            }

            mailMergeTask.MessageId = responseCreate["response"]["id"].Value<int>();
            mailMergeTask.StreamId = responseCreate["response"]["streamId"].Value<string>();
        }      

        private async Task<string> AttachToMailAsync(MailMergeTask mailMergeTask, IHttpClientFactory clientFactory)
        {
            if (mailMergeTask.Attach == null) return string.Empty;

            if (string.IsNullOrEmpty(mailMergeTask.AttachTitle)) mailMergeTask.AttachTitle = "attach.pdf";

            var apiUrlAttach = string.Format("{0}mail/messages/attachment/add?id_message={1}&name={2}",
                                             SetupInfo.WebApiBaseUrl,
                                             mailMergeTask.MessageId,
                                             mailMergeTask.AttachTitle);

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(BaseCommonLinkUtility.GetFullAbsolutePath(apiUrlAttach));
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", SecurityContext.AuthenticateMe(SecurityContext.CurrentAccount.ID));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(mailMergeTask.AttachTitle);
            request.Content = new StreamContent(mailMergeTask.Attach);

            string responseAttachString;
            var httpClient = clientFactory.CreateClient();
            using var response = await httpClient.SendAsync(request);
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                if (stream == null) throw new HttpRequestException("Could not get an answer");
                using var reader = new StreamReader(stream);
                responseAttachString = await reader.ReadToEndAsync();
            }

            var responseAttach = JObject.Parse(responseAttachString);

            if (responseAttach["statusCode"].Value<int>() != (int)HttpStatusCode.Created)
            {
                throw new Exception("Attach failed: " + responseAttach["error"]["message"].Value<string>());
            }

            var bodySendAttach =
                "&attachments%5B0%5D%5BfileId%5D=" + HttpUtility.UrlEncode(responseAttach["response"]["fileId"].Value<string>())
                + "&attachments%5B0%5D%5BfileName%5D=" + HttpUtility.UrlEncode(responseAttach["response"]["fileName"].Value<string>())
                + "&attachments%5B0%5D%5Bsize%5D=" + HttpUtility.UrlEncode(responseAttach["response"]["size"].Value<string>())
                + "&attachments%5B0%5D%5BcontentType%5D=" + HttpUtility.UrlEncode(responseAttach["response"]["contentType"].Value<string>())
                + "&attachments%5B0%5D%5BfileNumber%5D=" + HttpUtility.UrlEncode(responseAttach["response"]["fileNumber"].Value<string>())
                + "&attachments%5B0%5D%5BstoredName%5D=" + HttpUtility.UrlEncode(responseAttach["response"]["storedName"].Value<string>())
                + "&attachments%5B0%5D%5BstreamId%5D=" + HttpUtility.UrlEncode(responseAttach["response"]["streamId"].Value<string>())
                ;

            return bodySendAttach;
        }

        private string SendMail(MailMergeTask mailMergeTask, string bodySendAttach)
        {
            //var apiUrlSend = $"{SetupInfo.WebApiBaseUrl}mail/messages/send.json";

            //var bodySend =
            //    string.Format(
            //        MailMergeTask.MessageBodyFormat,
            //        mailMergeTask.MessageId,
            //        HttpUtility.UrlEncode(mailMergeTask.From),
            //        HttpUtility.UrlEncode(mailMergeTask.Subject),
            //        HttpUtility.UrlEncode(mailMergeTask.To),
            //        HttpUtility.UrlEncode(mailMergeTask.Message));

            //bodySend += bodySendAttach;
            const string responseSendString = null;//TODO: Encoding.UTF8.GetString(Convert.FromBase64String(Api.GetApiResponse(apiUrlSend, "PUT", bodySend)));
            var responseSend = JObject.Parse(responseSendString);

            if (responseSend["statusCode"].Value<int>() != (int)HttpStatusCode.OK)
            {
                throw new Exception("Create draft failed: " + responseSend["error"]["message"].Value<string>());
            }
            return responseSend["response"].Value<string>();
        }
    }
}