using ASC.Mail.Core.Dao.Expressions.Mailbox;
using ASC.Mail.Exceptions;
using ASC.Mail.Extensions;
using ASC.Mail.Models;
using ASC.Web.Api.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace ASC.Mail.Controllers
{
    public partial class MailController : ControllerBase
    {
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
            var accounts = AccountEngine.GetAccountInfoList();
            return accounts.ToAccountData();
        }

        /// <summary>
        ///    Returns the information about the account.
        /// </summary>
        /// <param name="email">Account email address</param>
        /// <returns>Account with specified email</returns>
        /// <exception cref="ArgumentException">Exception happens when in parameters is invalid. Text description contains parameter name and text description.</exception>
        /// <exception cref="NullReferenceException">Exception happens when mailbox wasn't found by email.</exception>
        /// <short>Get account by email</short> 
        /// <category>Accounts</category>
        [Read(@"accounts/single")]
        public MailBoxData GetAccount(string email)
        {
            var account = AccountEngine.GetAccount(email);

            return account;
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
            var accountInfo = AccountEngine.TryCreateAccount(model, out LoginResult loginResult);

            if (accountInfo == null)
                throw new LoginException(loginResult);

            return accountInfo.ToAccountData().FirstOrDefault();
        }
    }
}
