using ASC.Mail.Core.Engine;
using ASC.Web.Api.Routing;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ASC.Mail.Controllers
{
    public partial class MailController : ControllerBase
    {
        /// <summary>
        /// Export all message's attachments to MyDocuments
        /// </summary>
        /// <param name="id_message">Id of any message</param>
        /// <param name="id_folder" optional="true">Id of Documents folder (if empty then @My)</param>
        /// <returns>Count of exported attachments</returns>
        /// <category>Messages</category>
        [Update(@"messages/attachments/export")]
        public int ExportAttachmentsToDocuments(int id_message, string id_folder = null)
        {
            if (id_message < 1)
                throw new ArgumentException(@"Invalid message id", "id_message");

            if (string.IsNullOrEmpty(id_folder))
                id_folder = DocumentsEngine.MY_DOCS_FOLDER_ID;

            var savedAttachmentsList = DocumentsEngine.StoreAttachmentsToDocuments(id_message, id_folder);

            return savedAttachmentsList.Count;
        }
    }
}
