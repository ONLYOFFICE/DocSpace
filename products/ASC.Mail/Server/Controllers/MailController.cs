using ASC.Api.Core;
using ASC.Common.Logging;
using ASC.Core.Tenants;
using ASC.Mail.Models;
using ASC.Mail.Core;
using ASC.Web.Api.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using ASC.Mail.Extensions;
using ASC.Core;
using ASC.Mail.Enums;
using System.Net.Mail;
using System;
using System.Linq;
using ASC.Mail.Exceptions;

namespace ASC.Mail.Controllers
{
    [DefaultRoute]
    [ApiController]
    public class MailController : ControllerBase
    {
        public int TenantId { 
            get { 
                return ApiContext.Tenant.TenantId; 
            }
        }

        public string UserId { 
            get { 
                return SecurityContext.CurrentAccount.ID.ToString(); 
            } 
        }

        public SecurityContext SecurityContext { get; }

        public ApiContext ApiContext { get; }

        public EngineFactory MailEngineFactory { get; }

        public ILog Log { get; }

        public MailController(
            ApiContext apiContext,
            SecurityContext securityContext,
            IOptionsMonitor<ILog> option,
            EngineFactory engine)
        {
            ApiContext = apiContext;
            SecurityContext = securityContext;
            Log = option.Get("ASC.Api");
            MailEngineFactory = engine; //new EngineFactory(TenantId, UserId, Log);
        }

        [Read("info")]
        public Module GetModule()
        {
            var product = new MailProduct();
            product.Init();
            return new Module(product, false);
        }

        #region Api.Accounts
        /// <summary>
        ///    Returns lists of all mailboxes, aliases and groups for user.
        /// </summary>
        /// <param name="username" visible="false">User id</param>
        /// <returns>Mailboxes, aliases and groups list</returns>
        /// <short>Get mailboxes, aliases and groups list</short> 
        /// <category>Accounts</category>
        [Read("accounts")]
        public IEnumerable<MailAccountData> GetAccounts()
        {
            var accounts = MailEngineFactory.AccountEngine.GetAccountInfoList();
            return accounts.ToAccountData();
        }

        /// <summary>
        ///    Creates account using full information about mail servers.
        /// </summary>
        /// <param name="model">instance of AccountModel</param>
        /// <returns>Created account</returns>
        /// <exception cref="Exception">Exception contains text description of happened error.</exception>
        /// <short>Create account with custom mail servers.</short> 
        /// <category>Accounts</category>
        [Create("accounts")]
        public MailAccountData CreateAccount(AccountModel model)
        {
            var accountInfo = MailEngineFactory.AccountEngine.TryCreateAccount(model, out LoginResult loginResult);

            if (accountInfo == null)
                throw new LoginException(loginResult);

            return accountInfo.ToAccountData().FirstOrDefault();
        }
        #endregion
        #region Api.Alerts
        /// <summary>
        ///    Returns the list of alerts for the authenticated user
        /// </summary>
        /// <returns>Alerts list</returns>
        /// <short>Get alerts list</short> 
        /// <category>Alerts</category>
        [Read("alert")]
        public IList<MailAlertData> GetAlerts()
        {
            var alerts = MailEngineFactory.AlertEngine.GetAlerts();
            return alerts;
        }

        /// <summary>
        ///    Deletes the alert with the ID specified in the request
        /// </summary>
        /// <param name="id">Alert ID</param>
        /// <returns>Deleted alert id. Same as request parameter.</returns>
        /// <short>Delete alert by ID</short> 
        /// <category>Alerts</category>
        [Delete("alert/{id}")]
        public long DeleteAlert(long id)
        {
            if (id < 0)
                throw new ArgumentException(@"Invalid alert id. Id must be positive integer.", "id");

            var success = MailEngineFactory.AlertEngine.DeleteAlert(id);

            if (!success)
                throw new Exception("Delete failed");

            return id;
        }
        #endregion
        #region Api.Images
        /// <summary>
        ///    Returns list of all trusted addresses for image displaying.
        /// </summary>
        /// <returns>Addresses list. Email adresses represented as string name@domain.</returns>
        /// <short>Get trusted addresses</short> 
        /// <category>Images</category>
        [Read("display_images/addresses")]
        public IEnumerable<string> GetDisplayImagesAddresses()
        {
            return MailEngineFactory.DisplayImagesAddressEngine.Get();
        }

        /// <summary>
        ///    Add the address to trusted addresses.
        /// </summary>
        /// <param name="address">Address for adding. </param>
        /// <returns>Added address</returns>
        /// <short>Add trusted address</short> 
        /// <exception cref="ArgumentException">Exception happens when in parameters is invalid. Text description contains parameter name and text description.</exception>
        /// <category>Images</category>
        [Create("display_images/address")]
        public string AddDisplayImagesAddress(string address)
        {
            MailEngineFactory.DisplayImagesAddressEngine.Add(address);

            return address;
        }

        /// <summary>
        ///    Remove the address from trusted addresses.
        /// </summary>
        /// <param name="address">Address for removing</param>
        /// <returns>Removed address</returns>
        /// <short>Remove from trusted addresses</short> 
        /// <exception cref="ArgumentException">Exception happens when in parameters is invalid. Text description contains parameter name and text description.</exception>
        /// <category>Images</category>
        [Delete("display_images/address")]
        public string RemovevDisplayImagesAddress(string address)
        {
            MailEngineFactory.DisplayImagesAddressEngine.Remove(address);

            return address;
        }
        #endregion
        #region Api.Signature
        /// <summary>
        /// This method needed for getting mailbox signature.
        /// </summary>
        /// <param name="mailbox_id"></param>
        /// <returns>Signature object</returns>
        [Read("signature/{mailbox_id}")]
        public MailSignatureData GetSignature(int mailbox_id)
        {
            var accounts = GetAccounts();

            var account = accounts.FirstOrDefault(a => a.MailboxId == mailbox_id);

            if (account == null)
                throw new ArgumentException("Mailbox not found");

            return account.Signature;
        }

        /// <summary>
        /// This method needed for update or create signature.
        /// </summary>
        /// <param name="mailbox_id">Id of updated mailbox.</param>
        /// <param name="html">New signature value.</param>
        /// <param name="is_active">New signature status.</param>
        [Create("signature/update/{mailbox_id}")]
        public MailSignatureData UpdateSignature(int mailbox_id, string html, bool is_active)
        {
            var accounts = GetAccounts();

            var account = accounts.FirstOrDefault(a => a.MailboxId == mailbox_id);

            if (account == null)
                throw new ArgumentException("Mailbox not found");

            return MailEngineFactory.SignatureEngine.SaveSignature(mailbox_id, html, is_active);
        }

        #endregion
        #region Api.Tags
        /// <summary>
        ///    Returns list of the tags used in Mail
        /// </summary>
        /// <returns>Tags list. Tags represented as JSON.</returns>
        /// <short>Get tags list</short> 
        /// <category>Tags</category>
        [Read(@"tags")]
        public IEnumerable<MailTagData> GetTags()
        {
            return MailEngineFactory.TagEngine.GetTags().ToTagData();
        }

        /// <summary>
        ///    Creates a new tag
        /// </summary>
        /// <param name="name">Tag name represented as string</param>
        /// <param name="style">Style identificator. With postfix will be added to tag css style whe it will represent. Specifies color of tag.</param>
        /// <param name="addresses">Specifies list of addresses tag associated with.</param>
        /// <returns>MailTag</returns>
        /// <short>Create tag</short> 
        /// <category>Tags</category>
        /// <exception cref="ArgumentException">Exception happens when in parameters is invalid. Text description contains parameter name and text description.</exception>
        /*[Create(@"tags")]
        public MailTagData CreateTag(string name, string style, IEnumerable<string> addresses)
        {
            //TODO: Is it necessary?
            Thread.CurrentThread.CurrentCulture = CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CurrentCulture;

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(MailApiResource.ErrorTagNameCantBeEmpty);

            if (MailEngineFactory.TagEngine.IsTagExists(name))
                throw new ArgumentException(MailApiResource.ErrorTagNameAlreadyExists.Replace("%1", "\"" + name + "\""));

            return MailEngineFactory.TagEngine.CreateTag(name, style, addresses).ToTagData();

        }*/

        /// <summary>
        ///    Updates the selected tag
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name">Tag name represented as string</param>
        /// <param name="style">Style identificator. With postfix will be added to tag css style whe it will represent. Specifies color of tag.</param>
        /// <param name="addresses">Specifies list of addresses tag associated with.</param>
        /// <returns>Updated MailTag</returns>
        /// <short>Update tag</short> 
        /// <category>Tags</category>
        /// <exception cref="ArgumentException">Exception happens when in parameters is invalid. Text description contains parameter name and text description.</exception>
        /*[Update(@"tags/{id}")]
        public MailTagData UpdateTag(int id, string name, string style, IEnumerable<string> addresses)
        {
            if (id < 0)
                throw new ArgumentException(@"Invalid tag id", "id");

            //TODO: Is it necessary?
            Thread.CurrentThread.CurrentCulture = CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CurrentCulture;

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(MailApiResource.ErrorTagNameCantBeEmpty);

            try
            {
                var tag = MailEngineFactory.TagEngine.UpdateTag(id, name, style, addresses);

                return tag.ToTagData();
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Equals("Tag name already exists"))
                    throw new ArgumentException(MailApiResource.ErrorTagNameAlreadyExists.Replace("%1",
                        "\"" + name + "\""));

                throw;
            }
        }*/

        /// <summary>
        ///    Deletes the selected tag from TLMail
        /// </summary>
        /// <param name="id">Tag for deleting id</param>
        /// <returns>Deleted MailTag</returns>
        /// <short>Delete tag</short> 
        /// <category>Tags</category>
        /// <exception cref="ArgumentException">Exception happens when in parameters is invalid. Text description contains parameter name and text description.</exception>
        /*[Delete(@"tags/{id}")]
        public int DeleteTag(int id)
        {
            if (id < 0)
                throw new ArgumentException(@"Invalid tag id", "id");

            if (!MailEngineFactory.TagEngine.DeleteTag(id))
                throw new Exception("DeleteTag failed");

            return id;
        }*/

        /// <summary>
        ///    Adds the selected tag to the messages
        /// </summary>
        /// <param name="id">Tag for setting id</param>
        /// <param name="messages">Messages id for setting.</param>
        /// <returns>Setted MailTag</returns>
        /// <short>Set tag to messages</short> 
        /// <category>Tags</category>
        /// <exception cref="ArgumentException">Exception happens when in parameters is invalid. Text description contains parameter name and text description.</exception>
        /*[Update(@"tags/{id}/set")]
        public int SetTag(int id, List<int> messages)
        {
            if (!messages.Any())
                throw new ArgumentException(@"Messages are empty", "messages");

            MailEngineFactory.TagEngine.SetMessagesTag(messages, id);

            return id;
        }*/

        /// <summary>
        ///    Removes the specified tag from messages
        /// </summary>
        /// <param name="id">Tag for removing id</param>
        /// <param name="messages">Messages id for removing.</param>
        /// <returns>Removed mail tag</returns>
        /// <short>Remove tag from messages</short> 
        /// <category>Tags</category>
        /// <exception cref="ArgumentException">Exception happens when parameters are invalid. Text description contains parameter name and text description.</exception>
        /*[Update(@"tags/{id}/unset")]
        public int UnsetTag(int id, List<int> messages)
        {
            if (!messages.Any())
                throw new ArgumentException(@"Messages are empty", "messages");

            MailEngineFactory.TagEngine.UnsetMessagesTag(messages, id);

            return id;
        }*/
        #endregion
    }

    public static class MailControllerExtention
    {
        public static IServiceCollection AddMailController(this IServiceCollection services)
        {
            return services
                .AddApiContextService()
                .AddSecurityContextService()
                .AddEngineFactoryService();
        }
    }
}