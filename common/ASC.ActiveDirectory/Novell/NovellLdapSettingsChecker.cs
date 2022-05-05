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

namespace ASC.ActiveDirectory.Novell;

[Scope]
public class NovellLdapSettingsChecker : LdapSettingsChecker
{
    public LdapCertificateConfirmRequest CertificateConfirmRequest { get; set; }

    public LdapHelper LdapHelper
    {
        get { return LdapImporter.LdapHelper; }
    }

    public NovellLdapSettingsChecker(IOptionsMonitor<ILog> option) :
        base(option)
    {
    }

    public void Init(LdapUserImporter importer)
    {
        base.Init(importer);
    }

    public override LdapSettingsStatus CheckSettings()
    {
        if (!Settings.EnableLdapAuthentication)
            return LdapSettingsStatus.Ok;

        if (Settings.Server.Equals("LDAP://", StringComparison.InvariantCultureIgnoreCase))
            return LdapSettingsStatus.WrongServerOrPort;

        if (!LdapHelper.IsConnected)
        {
            try
            {
                LdapHelper.Connect();
            }
            catch (NovellLdapTlsCertificateRequestedException ex)
            {
                log.ErrorFormat("CheckSettings(acceptCertificate={0}): NovellLdapTlsCertificateRequestedException: {1}", Settings.AcceptCertificate, ex);
                CertificateConfirmRequest = ex.CertificateConfirmRequest;
                return LdapSettingsStatus.CertificateRequest;
            }
            catch (NotSupportedException ex)
            {
                log.ErrorFormat("CheckSettings(): NotSupportedException: {0}", ex);
                return LdapSettingsStatus.TlsNotSupported;
            }
            catch (SocketException ex)
            {
                log.ErrorFormat("CheckSettings(): SocketException: {0}", ex);
                return LdapSettingsStatus.ConnectError;
            }
            catch (ArgumentException ex)
            {
                log.ErrorFormat("CheckSettings(): ArgumentException: {0}", ex);
                return LdapSettingsStatus.WrongServerOrPort;
            }
            catch (SecurityException ex)
            {
                log.ErrorFormat("CheckSettings(): SecurityException: {0}", ex);
                return LdapSettingsStatus.StrongAuthRequired;
            }
            catch (SystemException ex)
            {
                log.ErrorFormat("CheckSettings(): SystemException: {0}", ex);
                return LdapSettingsStatus.WrongServerOrPort;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("CheckSettings(): Exception: {0}", ex);
                return LdapSettingsStatus.CredentialsNotValid;
            }
        }

        if (!CheckUserDn(Settings.UserDN))
        {
            return LdapSettingsStatus.WrongUserDn;
        }

        if (Settings.GroupMembership)
        {
            if (!CheckGroupDn(Settings.GroupDN))
            {
                return LdapSettingsStatus.WrongGroupDn;
            }

            try
            {
                new RfcFilter(Settings.GroupFilter);
            }
            catch
            {
                return LdapSettingsStatus.IncorrectGroupLDAPFilter;
            }

            if (!LdapImporter.TryLoadLDAPGroups())
            {
                if (!LdapImporter.AllSkipedDomainGroups.Any())
                    return LdapSettingsStatus.IncorrectGroupLDAPFilter;

                if (LdapImporter.AllSkipedDomainGroups.All(kv => kv.Value == LdapSettingsStatus.WrongSidAttribute))
                    return LdapSettingsStatus.WrongSidAttribute;

                if (LdapImporter.AllSkipedDomainGroups.All(kv => kv.Value == LdapSettingsStatus.WrongGroupAttribute))
                    return LdapSettingsStatus.WrongGroupAttribute;

                if (LdapImporter.AllSkipedDomainGroups.All(kv => kv.Value == LdapSettingsStatus.WrongGroupNameAttribute))
                    return LdapSettingsStatus.WrongGroupNameAttribute;
            }

            if (!LdapImporter.AllDomainGroups.Any())
                return LdapSettingsStatus.GroupsNotFound;
        }

        try
        {
            new RfcFilter(Settings.UserFilter);
        }
        catch
        {
            return LdapSettingsStatus.IncorrectLDAPFilter;
        }

        if (!LdapImporter.TryLoadLDAPUsers())
        {
            if (!LdapImporter.AllSkipedDomainUsers.Any())
                return LdapSettingsStatus.IncorrectLDAPFilter;

            if (LdapImporter.AllSkipedDomainUsers.All(kv => kv.Value == LdapSettingsStatus.WrongSidAttribute))
                return LdapSettingsStatus.WrongSidAttribute;

            if (LdapImporter.AllSkipedDomainUsers.All(kv => kv.Value == LdapSettingsStatus.WrongLoginAttribute))
                return LdapSettingsStatus.WrongLoginAttribute;

            if (LdapImporter.AllSkipedDomainUsers.All(kv => kv.Value == LdapSettingsStatus.WrongUserAttribute))
                return LdapSettingsStatus.WrongUserAttribute;
        }

        if (!LdapImporter.AllDomainUsers.Any())
            return LdapSettingsStatus.UsersNotFound;

        return string.IsNullOrEmpty(LdapImporter.LDAPDomain)
            ? LdapSettingsStatus.DomainNotFound
            : LdapSettingsStatus.Ok;
    }

    private bool CheckUserDn(string userDn)
    {
        try
        {
            return LdapHelper.CheckUserDn(userDn);
        }
        catch (Exception e)
        {
            log.ErrorFormat("Wrong User DN parameter: {0}. {1}", userDn, e);
            return false;
        }
    }

    private bool CheckGroupDn(string groupDn)
    {
        try
        {
            return LdapHelper.CheckGroupDn(groupDn);
        }
        catch (Exception e)
        {
            log.ErrorFormat("Wrong Group DN parameter: {0}. {1}", groupDn, e);
            return false;
        }
    }
}
