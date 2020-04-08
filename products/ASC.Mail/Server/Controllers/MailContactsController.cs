using ASC.Mail.Core.Dao.Expressions.Contact;
using ASC.Mail.Enums;
using ASC.Mail.Extensions;
using ASC.Mail.Models;
using ASC.Web.Api.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ASC.Mail.Controllers
{
    public partial class MailController : ControllerBase
    {
        /// <summary>
        ///    Returns the list of the contacts for auto complete feature.
        /// </summary>
        /// <param name="term">string part of contact name, lastname or email.</param>
        /// <returns>Strings list.  Strings format: "Name Lastname" email</returns>
        /// <short>Get contact list for auto complete</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException">Exception happens when in parameters is invalid. Text description contains parameter name and text description.</exception>
        [Read(@"emails/search")]
        public IEnumerable<string> SearchEmails(string term)
        {
            if (string.IsNullOrEmpty(term))
                throw new ArgumentException(@"term parameter empty.", "term");

            return ContactEngine.SearchEmails(TenantId, UserId, term, 
                MailAutocompleteMaxCountPerSystem, MailAutocompleteTimeout);
        }

        /// <summary>
        ///    Returns lists of mail contacts.
        /// </summary>
        /// <param optional="true" name="search">Text to search in contacts name or emails.</param>
        /// <param optional="true" name="contactType">Type of contacts</param>
        /// <param optional="true" name="pageSize">Count of contacts on page</param>
        /// <param optional="true" name="fromIndex">Page number</param> 
        /// <param name="sortorder">Sort order by name. String parameter: "ascending" - ascended, "descending" - descended.</param> 
        /// <returns>List of filtered contacts</returns>
        /// <short>Gets filtered contacts</short> 
        /// <category>Contacts</category>
        [Read(@"contacts")]
        public IEnumerable<MailContactData> GetContacts(string search, int? contactType, int? pageSize, int fromIndex,
            string sortorder)
        {
            var contacts = ContactEngine
                .GetContacts(search, contactType, pageSize, fromIndex, sortorder, out int totalCount);

            ApiContext.SetTotalCount(totalCount);

            return contacts;
        }

        /// <summary>
        ///   Returns lists of mail contacts with contact information
        /// </summary>
        /// <param optional="false" name="infoType">infoType</param>
        /// <param optional="false" name="data">data</param>
        /// <param optional="true" name="isPrimary">isPrimary</param>
        /// <returns>List of filtered contacts</returns>
        /// <short>Gets filtered contacts</short> 
        /// <category>Contacts</category>
        [Read(@"contacts/bycontactinfo")]
        public IEnumerable<MailContactData> GetContactsByContactInfo(ContactInfoType infoType, string data, bool? isPrimary)
        {
            var contacts = ContactEngine.GetContactsByContactInfo(infoType, data, isPrimary);

            return contacts;
        }
    }
}
