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

namespace ASC.Web.Files.Utils;

public class MailMergeTask : IDisposable
{
    internal const string _messageBodyFormat = "id={0}&from={1}&subject={2}&to%5B%5D={3}&body={4}&mimeReplyToId=";

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
        {
            Attach.Dispose();
        }
    }
}

[Scope]
public class MailMergeTaskRunner
{
    private readonly SetupInfo _setupInfo;
    private readonly SecurityContext _securityContext;
    private readonly BaseCommonLinkUtility _baseCommonLinkUtility;

    //private ApiServer _apiServer;

    //protected ApiServer Api
    //{
    //    get { return _apiServer ?? (_apiServer = new ApiServer()); }
    //}

    public MailMergeTaskRunner(SetupInfo setupInfo, SecurityContext securityContext, BaseCommonLinkUtility baseCommonLinkUtility)
    {
        _setupInfo = setupInfo;
        _securityContext = securityContext;
        _baseCommonLinkUtility = baseCommonLinkUtility;
    }

    public async Task<string> RunAsync(MailMergeTask mailMergeTask, IHttpClientFactory clientFactory)
    {
        if (string.IsNullOrEmpty(mailMergeTask.From))
        {
            throw new ArgumentException("From is null");
        }

        if (string.IsNullOrEmpty(mailMergeTask.To))
        {
            throw new ArgumentException("To is null");
        }

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
        if (mailMergeTask.Attach == null)
        {
            return string.Empty;
        }

        if (string.IsNullOrEmpty(mailMergeTask.AttachTitle))
        {
            mailMergeTask.AttachTitle = "attach.pdf";
        }

        var apiUrlAttach = string.Format("{0}mail/messages/attachment/add?id_message={1}&name={2}",
                                         _setupInfo.WebApiBaseUrl,
                                         mailMergeTask.MessageId,
                                         mailMergeTask.AttachTitle);

        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(_baseCommonLinkUtility.GetFullAbsolutePath(apiUrlAttach)),
            Method = HttpMethod.Post
        };
        request.Headers.Add("Authorization", await _securityContext.AuthenticateMeAsync(_securityContext.CurrentAccount.ID));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue(mailMergeTask.AttachTitle);
        request.Content = new StreamContent(mailMergeTask.Attach);

        string responseAttachString;
        var httpClient = clientFactory.CreateClient();
        using var response = await httpClient.SendAsync(request);
        await using (var stream = await response.Content.ReadAsStreamAsync())
        {
            if (stream == null)
            {
                throw new HttpRequestException("Could not get an answer");
            }

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
