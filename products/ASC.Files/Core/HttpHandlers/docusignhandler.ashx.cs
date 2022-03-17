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

namespace ASC.Web.Files.HttpHandlers
{
    public class DocuSignHandler
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DocuSignHandler(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext context)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var docuSignHandlerService = scope.ServiceProvider.GetService<DocuSignHandlerService>();
            await docuSignHandlerService.Invoke(context).ConfigureAwait(false);
        }
    }

    [Scope]
    public class DocuSignHandlerService
    {
        public static string Path(FilesLinkUtility filesLinkUtility)
        {
            return filesLinkUtility.FilesBaseAbsolutePath + "httphandlers/docusignhandler.ashx";
        }

        private ILog Log { get; set; }
        private TenantExtra TenantExtra { get; }
        private DocuSignHelper DocuSignHelper { get; }
        private SecurityContext SecurityContext { get; }
        private NotifyClient NotifyClient { get; }

        public DocuSignHandlerService(
            IOptionsMonitor<ILog> optionsMonitor,
            TenantExtra tenantExtra,
            DocuSignHelper docuSignHelper,
            SecurityContext securityContext,
            NotifyClient notifyClient)
        {
            TenantExtra = tenantExtra;
            DocuSignHelper = docuSignHelper;
            SecurityContext = securityContext;
            NotifyClient = notifyClient;
            Log = optionsMonitor.CurrentValue;
        }

        public async Task Invoke(HttpContext context)
        {
            if (TenantExtra.IsNotPaid())
            {
                context.Response.StatusCode = (int)HttpStatusCode.PaymentRequired;
                await context.Response.WriteAsync("Payment Required.");
                return;
            }

            try
            {
                switch ((context.Request.Query[FilesLinkUtility.Action].FirstOrDefault() ?? "").ToLower())
                {
                    case "redirect":
                        Redirect(context);
                        break;
                    case "webhook":
                        await WebhookAsync(context);
                        break;
                    default:
                        throw new HttpException((int)HttpStatusCode.BadRequest, FilesCommonResource.ErrorMassage_BadRequest);
                }
            }
            catch (InvalidOperationException e)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, FilesCommonResource.ErrorMassage_BadRequest, e);
            }
        }

        private void Redirect(HttpContext context)
        {
            Log.Info("DocuSign redirect query: " + context.Request.QueryString);

            var eventRedirect = context.Request.Query["event"].FirstOrDefault();
            switch (eventRedirect.ToLower())
            {
                case "send":
                    context.Response.Redirect(PathProvider.StartURL + "#message/" + HttpUtility.UrlEncode(FilesCommonResource.DocuSignStatusSended), true);
                    break;
                case "save":
                case "cancel":
                    context.Response.Redirect(PathProvider.StartURL + "#error/" + HttpUtility.UrlEncode(FilesCommonResource.DocuSignStatusNotSended), true);
                    break;
                case "error":
                case "sessionend":
                    context.Response.Redirect(PathProvider.StartURL + "#error/" + HttpUtility.UrlEncode(FilesCommonResource.DocuSignStatusError), true);
                    break;
            }
            context.Response.Redirect(PathProvider.StartURL, true);
        }

        private const string XmlPrefix = "docusign";

        private async Task WebhookAsync(HttpContext context)
        {
            Log.Info("DocuSign webhook: " + context.Request.QueryString);
            try
            {
                var xmldoc = new XmlDocument();
                xmldoc.Load(context.Request.Body);
                Log.Info("DocuSign webhook outerXml: " + xmldoc.OuterXml);

                var mgr = new XmlNamespaceManager(xmldoc.NameTable);
                mgr.AddNamespace(XmlPrefix, "http://www.docusign.net/API/3.0");

                var envelopeStatusNode = GetSingleNode(xmldoc, "DocuSignEnvelopeInformation/" + XmlPrefix + ":EnvelopeStatus", mgr);
                var envelopeId = GetSingleNode(envelopeStatusNode, "EnvelopeID", mgr).InnerText;
                var subject = GetSingleNode(envelopeStatusNode, "Subject", mgr).InnerText;

                var statusString = GetSingleNode(envelopeStatusNode, "Status", mgr).InnerText;
                if (!Enum.TryParse(statusString, true, out DocuSignStatus status))
                {
                    throw new Exception("DocuSign webhook unknown status: " + statusString);
                }

                Log.Info("DocuSign webhook: " + envelopeId + " " + subject + " " + status);

                var customFieldUserIdNode = GetSingleNode(envelopeStatusNode, "CustomFields/" + XmlPrefix + ":CustomField[" + XmlPrefix + ":Name='" + DocuSignHelper.UserField + "']", mgr);
                var userIdString = GetSingleNode(customFieldUserIdNode, "Value", mgr).InnerText;
                Auth(userIdString);

                switch (status)
                {
                    case DocuSignStatus.Completed:

                        var documentStatuses = GetSingleNode(envelopeStatusNode, "DocumentStatuses", mgr);
                        foreach (XmlNode documentStatus in documentStatuses.ChildNodes)
                        {
                            try
                            {
                                var documentId = GetSingleNode(documentStatus, "ID", mgr).InnerText;
                                var documentName = GetSingleNode(documentStatus, "Name", mgr).InnerText;

                                string folderId = null;
                                string sourceTitle = null;

                                var documentFiels = GetSingleNode(documentStatus, "DocumentFields", mgr, true);
                                if (documentFiels != null)
                                {
                                    var documentFieldFolderNode = GetSingleNode(documentFiels, "DocumentField[" + XmlPrefix + ":Name='" + FilesLinkUtility.FolderId + "']", mgr, true);
                                    if (documentFieldFolderNode != null)
                                    {
                                        folderId = GetSingleNode(documentFieldFolderNode, "Value", mgr).InnerText;
                                    }
                                    var documentFieldTitleNode = GetSingleNode(documentFiels, "DocumentField[" + XmlPrefix + ":Name='" + FilesLinkUtility.FileTitle + "']", mgr, true);
                                    if (documentFieldTitleNode != null)
                                    {
                                        sourceTitle = GetSingleNode(documentFieldTitleNode, "Value", mgr).InnerText;
                                    }
                                }

                                var file = await DocuSignHelper.SaveDocumentAsync(envelopeId, documentId, documentName, folderId);

                                NotifyClient.SendDocuSignComplete(file, sourceTitle ?? documentName);
                            }
                            catch (Exception ex)
                            {
                                Log.Error("DocuSign webhook save document: " + documentStatus.InnerText, ex);
                            }
                        }
                        break;
                    case DocuSignStatus.Declined:
                    case DocuSignStatus.Voided:
                        var statusFromResource = status == DocuSignStatus.Declined
                                                     ? FilesCommonResource.DocuSignStatusDeclined
                                                     : FilesCommonResource.DocuSignStatusVoided;
                        NotifyClient.SendDocuSignStatus(subject, statusFromResource);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error("DocuSign webhook", e);

                throw new HttpException((int)HttpStatusCode.BadRequest, e.Message);
            }
        }

        private void Auth(string userIdString)
        {
            if (!Guid.TryParse(userIdString ?? "", out var userId))
            {
                throw new Exception("DocuSign incorrect User ID: " + userIdString);
            }

            SecurityContext.AuthenticateMeWithoutCookie(userId);
        }

        private static XmlNode GetSingleNode(XmlNode node, string xpath, XmlNamespaceManager mgr, bool canMiss = false)
        {
            var result = node.SelectSingleNode(XmlPrefix + ":" + xpath, mgr);
            if (!canMiss && result == null)
            {
                throw new Exception(xpath + " is null");
            }

            return result;
        }
    }

    public static class DocuSignHandlerExtension
    {
        public static IApplicationBuilder UseDocuSignHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DocuSignHandler>();
        }
    }
}