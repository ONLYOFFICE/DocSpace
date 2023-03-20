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

namespace ASC.Migration.GoogleWorkspace.Models;

[Scope]
public class GwsMigratingContacts : MigratingContacts
{
    public override int ContactsCount => _contacts.Count;
    public override string ModuleName => MigrationResource.GoogleModuleNameContacts;

    private readonly string _rootFolder;

    private readonly List<GwsContact> _contacts = new List<GwsContact>();
    private readonly GwsMigratingUser _user;
    private readonly TenantManager _tenantManager;

    public GwsMigratingContacts(TenantManager tenantManager)
    {
        _tenantManager = tenantManager;
    }

    public override void Parse()
    {
        /*
        var vcfPath = Path.Combine(_rootFolder, "Contacts", "All Contacts", "All Contacts.vcf");
        if (!File.Exists(vcfPath))
        {
            return;
        }

        var vcards = VCard.LoadVcf(vcfPath);
        foreach (var vcard in vcards)
        {
            if (vcard.EmailAddresses == null || !vcard.EmailAddresses.Any())
            {
                continue; // We can't import contacts without email
            }

            var contact = new GwsContact()
            {
                Emails = vcard.EmailAddresses.Select(v => v.Value).Distinct().ToList()
            };

            if (vcard.Addresses != null && vcard.Addresses.Any())
            {
                contact.Address = vcard.Addresses.First().Value.ToString();
            }

            if (vcard.DisplayNames != null && vcard.DisplayNames.Any())
            {
                contact.ContactName = vcard.DisplayNames.First().Value.ToString();
            }

            if (vcard.Notes != null && vcard.Notes.Any())
            {
                contact.Description = string.Join("\n", vcard.Notes.Select(v => v.Value));
            }

            if (vcard.PhoneNumbers != null && vcard.PhoneNumbers.Any())
            {
                contact.Phones = vcard.PhoneNumbers.Select(v => v.Value).Distinct().ToList();
            }

            _contacts.Add(contact);
        }*/
    }

    public override Task MigrateAsync()
    {/*
        if (!ShouldImport)
        {
            return;
        }

        var engine = new ContactEngine(_tenantManager.GetCurrentTenant().Id, _user.Guid.ToString());
        foreach (var card in GetContactCards())
        {
            try
            {
                engine.SaveContactCard(card);
            }
            catch (Exception ex)
            {
                Log($"Couldn't save contactCard {card.ContactInfo.ContactName}", ex);
            }
        }*/
        return Task.CompletedTask;
    }

    public GwsMigratingContacts(string rootFolder, GwsMigratingUser user, Action<string, Exception> log) : base(log)
    {
        _rootFolder = rootFolder;
        _user = user;
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
    }*/
}
