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

namespace ASC.Migration.NextcloudWorkspace.Models.Parse;

public class NCMigratingContacts : MigratingContacts
{
    public override int ContactsCount => _contacts.Count;
    public override string ModuleName => MigrationResource.NextcloudModuleNameContacts;

    private readonly List<NCContact> _contacts = new List<NCContact>();
    private readonly TenantManager _tenantManager;
    private readonly NCMigratingUser _user;
    private readonly NCAddressbooks _addressbooks;

    public NCMigratingContacts(TenantManager tenantManager, NCMigratingUser user, NCAddressbooks addressbooks, Action<string, Exception> log) : base(log)
    {
        _tenantManager = tenantManager;
        _user = user;
        _addressbooks = addressbooks;
    }

    public override void Parse()
    {/*
        if (_addressbooks != null)
        {
            if (_addressbooks.Cards != null)
            {
                foreach (var vcardByte in _addressbooks.Cards)
                {
                    var vcard = VCard.ParseVcf(Encoding.Default.GetString(vcardByte.CardData));
                    foreach (var item in vcard)
                    {
                        if (item.EmailAddresses == null || !item.EmailAddresses.Any())
                        {
                            continue;
                        }

                        var contact = new NCContact()
                        {
                            Emails = item.EmailAddresses.Select(v => v.Value).Distinct().ToList()
                        };

                        if (item.Addresses != null && item.Addresses.Any())
                        {
                            contact.Address = item.Addresses.First().Value.ToString();
                        }

                        if (item.DisplayNames != null && item.DisplayNames.Any())
                        {
                            contact.ContactName = item.DisplayNames.First().Value.ToString();
                        }

                        if (item.Notes != null && item.Notes.Any())
                        {
                            contact.Description = string.Join("\n", item.Notes.Select(v => v.Value));
                        }

                        if (item.PhoneNumbers != null && item.PhoneNumbers.Any())
                        {
                            contact.Phones = item.PhoneNumbers.Select(v => v.Value).Distinct().ToList();
                        }

                        _contacts.Add(contact);
                    }
                }
            }
        }
        */
    }

    public override Task MigrateAsync()
    {
        return Task.CompletedTask;
        /*
        if (!ShouldImport)
        {
            return;
        }

        var engine = new ContactEngine(_tenantManager.GetCurrentTenant().Id, _user.Guid.ToString());
        var cards = GetContactCards();
        foreach (var card in cards)
        {
            try
            {
                engine.SaveContactCard(card);
            }
            catch (Exception ex)
            {
                Log($"Couldn't save contactCard {card.ContactInfo.ContactName}", ex);
            }
        }
        */
    }

    /*
    private List<ContactCard> GetContactCards()
    {
        var tenantId = _tenantManager.GetCurrentTenant().Id;
        var userId = _user.Guid.ToString();

        var portalContacts = new List<ContactCard>();
        foreach (var gwsContact in _contacts)
        {
            var portalContact = new Contact()
            {
                Type = ContactType.Personal,
                ContactName = gwsContact.ContactName,
                Address = gwsContact.Address,
                Tenant = tenantId,
                User = userId
            };

            var infos = new List<ContactInfo>();
            if (gwsContact.Emails != null)
            {
                infos.AddRange(gwsContact
                    .Emails.Select(e => new ContactInfo() { Data = e, Type = (int)ContactInfoType.Email, Tenant = tenantId, User = userId }));
            }
            if (gwsContact.Phones != null)
            {
                infos.AddRange(gwsContact
                    .Phones.Select(p => new ContactInfo() { Data = p, Type = (int)ContactInfoType.Phone, Tenant = tenantId, User = userId }));
            }
            try
            {
                portalContacts.Add(new ContactCard(portalContact, infos));
            }
            catch (Exception ex)
            {
                Log($"Couldn't create contactCard {gwsContact.ContactName}", ex);
            }
        }

        return portalContacts;
    }
    */
}
