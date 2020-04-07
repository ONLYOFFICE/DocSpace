using ASC.Api.Core;
using ASC.Common.Logging;
using ASC.Mail.Core;
using ASC.Web.Api.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ASC.Core;
using ASC.Common;
using ASC.Mail.Core.Engine;
using ASC.Mail.Core.Engine.Operations.Base;
using ASC.Web.Mail.Resources;
using ASC.Common.Threading;

namespace ASC.Mail.Controllers
{
    [DefaultRoute]
    [ApiController]
    public partial class MailController : ControllerBase
    {
        public int TenantId { 
            get { 
                return TenantManager.GetCurrentTenant().TenantId; 
            }
        }

        public string UserId { 
            get { 
                return SecurityContext.CurrentAccount.ID.ToString(); 
            } 
        }

        public TenantManager TenantManager { get; }
        public SecurityContext SecurityContext { get; }
        public AccountEngine AccountEngine { get; }
        public AlertEngine AlertEngine { get; }
        public DisplayImagesAddressEngine DisplayImagesAddressEngine { get; }
        public SignatureEngine SignatureEngine { get; }
        public TagEngine TagEngine { get; }
        public MailboxEngine MailboxEngine { get; }
        //public OperationEngine OperationEngine { get; }
        public ILog Log { get; }

        public MailController(
            TenantManager tenantManager,
            SecurityContext securityContext,
            AccountEngine accountEngine,
            AlertEngine alertEngine,
            DisplayImagesAddressEngine displayImagesAddressEngine,
            SignatureEngine signatureEngine,
            TagEngine tagEngine,
            MailboxEngine mailboxEngine,
            //OperationEngine operationEngine,
            IOptionsMonitor<ILog> option)
        {
            TenantManager = tenantManager;
            SecurityContext = securityContext;
            AccountEngine = accountEngine;
            AlertEngine = alertEngine;
            DisplayImagesAddressEngine = displayImagesAddressEngine;
            SignatureEngine = signatureEngine;
            TagEngine = tagEngine;
            MailboxEngine = mailboxEngine;
            //OperationEngine = operationEngine;

            Log = option.Get("ASC.Api.Mail");
        }

        [Read("info")]
        public Module GetModule()
        {
            var product = new MailProduct();
            product.Init();
            return new Module(product, false);
        }


        /// <summary>
        /// Method for translation mail operation statuses
        /// </summary>
        /// <param name="op">instance of DistributedTask</param>
        /// <returns>translated status text</returns>
        private static string TranslateMailOperationStatus(DistributedTask op)
        {
            var type = op.GetProperty<MailOperationType>(MailOperation.OPERATION_TYPE);
            var status = op.GetProperty<string>(MailOperation.STATUS);
            //TODO: Move strings to Resource file
            switch (type)
            {
                case MailOperationType.DownloadAllAttachments:
                    {
                        var progress = op.GetProperty<MailOperationDownloadAllAttachmentsProgress>(MailOperation.PROGRESS);
                        switch (progress)
                        {
                            case MailOperationDownloadAllAttachmentsProgress.Init:
                                return MailApiResource.SetupTenantAndUserHeader;
                            case MailOperationDownloadAllAttachmentsProgress.GetAttachments:
                                return MailApiResource.GetAttachmentsHeader;
                            case MailOperationDownloadAllAttachmentsProgress.Zipping:
                                return MailApiResource.ZippingAttachmentsHeader;
                            case MailOperationDownloadAllAttachmentsProgress.ArchivePreparation:
                                return MailApiResource.PreparationArchiveHeader;
                            case MailOperationDownloadAllAttachmentsProgress.CreateLink:
                                return MailApiResource.CreatingLinkHeader;
                            case MailOperationDownloadAllAttachmentsProgress.Finished:
                                return MailApiResource.FinishedHeader;
                            default:
                                return status;
                        }
                    }
                case MailOperationType.RemoveMailbox:
                    {
                        var progress = op.GetProperty<MailOperationRemoveMailboxProgress>(MailOperation.PROGRESS);
                        switch (progress)
                        {
                            case MailOperationRemoveMailboxProgress.Init:
                                return "Setup tenant and user";
                            case MailOperationRemoveMailboxProgress.RemoveFromDb:
                                return "Remove mailbox from Db";
                            case MailOperationRemoveMailboxProgress.FreeQuota:
                                return "Decrease newly freed quota space";
                            case MailOperationRemoveMailboxProgress.RecalculateFolder:
                                return "Recalculate folders counters";
                            case MailOperationRemoveMailboxProgress.ClearCache:
                                return "Clear accounts cache";
                            case MailOperationRemoveMailboxProgress.Finished:
                                return "Finished";
                            default:
                                return status;
                        }
                    }
                case MailOperationType.RecalculateFolders:
                    {
                        var progress = op.GetProperty<MailOperationRecalculateMailboxProgress>(MailOperation.PROGRESS);
                        switch (progress)
                        {
                            case MailOperationRecalculateMailboxProgress.Init:
                                return "Setup tenant and user";
                            case MailOperationRecalculateMailboxProgress.CountUnreadMessages:
                                return "Calculate unread messages";
                            case MailOperationRecalculateMailboxProgress.CountTotalMessages:
                                return "Calculate total messages";
                            case MailOperationRecalculateMailboxProgress.CountUreadConversation:
                                return "Calculate unread conversations";
                            case MailOperationRecalculateMailboxProgress.CountTotalConversation:
                                return "Calculate total conversations";
                            case MailOperationRecalculateMailboxProgress.UpdateFoldersCounters:
                                return "Update folders counters";
                            case MailOperationRecalculateMailboxProgress.CountUnreadUserFolderMessages:
                                return "Calculate unread messages in user folders";
                            case MailOperationRecalculateMailboxProgress.CountTotalUserFolderMessages:
                                return "Calculate total messages in user folders";
                            case MailOperationRecalculateMailboxProgress.CountUreadUserFolderConversation:
                                return "Calculate unread conversations in user folders";
                            case MailOperationRecalculateMailboxProgress.CountTotalUserFolderConversation:
                                return "Calculate total conversations in user folders";
                            case MailOperationRecalculateMailboxProgress.UpdateUserFoldersCounters:
                                return "Update user folders counters";
                            case MailOperationRecalculateMailboxProgress.Finished:
                                return "Finished";
                            default:
                                return status;
                        }
                    }
                case MailOperationType.RemoveUserFolder:
                    {
                        var progress = op.GetProperty<MailOperationRemoveUserFolderProgress>(MailOperation.PROGRESS);
                        switch (progress)
                        {
                            case MailOperationRemoveUserFolderProgress.Init:
                                return "Setup tenant and user";
                            case MailOperationRemoveUserFolderProgress.MoveMailsToTrash:
                                return "Move mails into Trash folder";
                            case MailOperationRemoveUserFolderProgress.DeleteFolders:
                                return "Delete folder";
                            case MailOperationRemoveUserFolderProgress.Finished:
                                return "Finished";
                            default:
                                return status;
                        }
                    }
                default:
                    return status;
            }
        }
    }

    public static class MailControllerExtention
    {
        public static DIHelper AddMailController(this DIHelper services)
        {
            return services
                .AddTenantManagerService()
                .AddSecurityContextService()
                .AddAccountEngineService()
                .AddAlertEngineService()
                .AddDisplayImagesAddressEngineService()
                .AddSignatureEngineService()
                .AddTagEngineService()
                .AddMailboxEngineService();
        }
    }
}