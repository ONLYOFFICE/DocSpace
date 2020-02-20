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

        /// <summary>
        ///    Creates account using full information about mail servers.
        /// </summary>
        /// <param name="name">Account name in Teamlab</param>
        /// <param name="email">Account email in string format like: name@domain.</param>
        /// <param name="account">Login for imap or pop server.</param>
        /// <param name="password">Password for imap or pop server</param>
        /// <param name="port">Port for imap or pop server</param>
        /// <param name="server">Imap or pop server address or IP.</param>
        /// <param name="smtp_account">Login for smtp server</param>
        /// <param name="smtp_password">Password for smtp server</param>
        /// <param name="smtp_port">Smtp server port</param>
        /// <param name="smtp_server">Smtp server adress or IP.</param>
        /// <param name="smtp_auth">Flag is smtp server authentication needed. Value: true or false.</param>
        /// <param name="imap">Flag is imap server using for incoming mails. Value: true or false.</param>
        /// <param name="restrict">Flag is all mails needed for download. Value: true or false. If vslue true, it will be downloaded messages from last 30 days only.</param>
        /// <param name="incoming_encryption_type">Specify encription type for imap or pop server. 0- None, 1 - SSL, 2- StartTLS</param>
        /// <param name="outcoming_encryption_type">Specify encription type for smtp server. 0- None, 1 - SSL, 2- StartTLS</param>
        /// <param name="auth_type_in">Specify authentication type for imap or pop server. 1 - Login, 4 - CremdMd5, 5 - OAuth2</param>
        /// <param name="auth_type_smtp">Specify authentication type for imap or pop server. 0- None, 1 - Login, 4 - CremdMd5, 5 - OAuth2</param>
        /// <returns>Created account</returns>
        /// <exception cref="Exception">Exception contains text description of happened error.</exception>
        /// <short>Create account with custom mail servers.</short> 
        /// <category>Accounts</category>
        [Create("accounts")]
        public MailAccountData CreateAccount(string name,
            string email,
            string account,
            string password,
            int port,
            string server,
            string smtp_account,
            string smtp_password,
            int smtp_port,
            string smtp_server,
            bool smtp_auth,
            bool imap,
            bool restrict,
            EncryptionType incoming_encryption_type,
            EncryptionType outcoming_encryption_type,
            SaslMechanism auth_type_in,
            SaslMechanism auth_type_smtp)
        {
            var mbox = new MailBoxData
            {
                Name = name,
                EMail = new MailAddress(email),
                Account = account,
                Password = password,
                Port = port,
                Server = server,
                SmtpAccount = smtp_account,
                SmtpPassword = smtp_password,
                SmtpPort = smtp_port,
                SmtpServer = smtp_server,
                Imap = imap,
                TenantId = TenantId,
                UserId = UserId,
                BeginDate = restrict
                    ? DateTime.Now.Subtract(new TimeSpan(MailBoxData.DefaultMailLimitedTimeDelta))
                    : new DateTime(MailBoxData.DefaultMailBeginTimestamp),
                Encryption = incoming_encryption_type,
                SmtpEncryption = outcoming_encryption_type,
                Authentication = auth_type_in,
                SmtpAuthentication = smtp_auth ? auth_type_smtp : SaslMechanism.None,
                Enabled = true
            };

            var accountInfo = MailEngineFactory.AccountEngine.TryCreateAccount(mbox, out LoginResult loginResult);

            if (accountInfo == null)
                throw new LoginException("Some error has happend", loginResult);

            return accountInfo.ToAccountData().FirstOrDefault();
        }

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