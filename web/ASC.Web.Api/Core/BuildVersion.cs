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

namespace ASC.Api.Settings;

[Scope]
public class BuildVersion
{
    public string DocSpace { get; set; }

    public string CommunityServer { get; set; } //old

    public string DocumentServer { get; set; }

    public string MailServer { get; set; }

    public string XmppServer { get; set; }

    [JsonIgnore]
    private readonly IConfiguration _configuration;

    [JsonIgnore]
    private readonly FilesLinkUtility _filesLinkUtility;

    [JsonIgnore]
    private readonly DocumentServiceConnector _documentServiceConnector;

    public BuildVersion(IConfiguration configuration, FilesLinkUtility filesLinkUtility, DocumentServiceConnector documentServiceConnector)
    {
        _configuration = configuration;
        _filesLinkUtility = filesLinkUtility;
        _documentServiceConnector = documentServiceConnector;
    }

    public async Task<BuildVersion> GetCurrentBuildVersionAsync()
    {
        CommunityServer = "12.0.0";

        DocSpace = GetDocSpaceVersion();
        DocumentServer = await GetDocumentVersionAsync();
        MailServer = GetMailServerVersion();
        XmppServer = GetXmppServerVersion();

        return this;
    }

    private string GetDocSpaceVersion()
    {
        return _configuration["version:number"] ?? "1.0.0";
    }

    private async Task<string> GetDocumentVersionAsync()
    {
        if (string.IsNullOrEmpty(_filesLinkUtility.DocServiceApiUrl))
        {
            return null;
        }

        return await _documentServiceConnector.GetVersionAsync();
    }

    private static string GetMailServerVersion()
    {
        //TODO
        return "";
        /*
        try
        {
               
            var engineFactory = new EngineFactory(
                CoreContext.TenantManager.GetCurrentTenant().Id,
                SecurityContext.CurrentAccount.ID.ToString());

            var version = engineFactory.ServerEngine.GetServerVersion();
            return version;
            }
        catch (Exception e)
        {
            LogManager.GetLogger("ASC").Warn(e.Message, e);
        }

        return null;*/
    }

    private static string GetXmppServerVersion()
    {
        //try
        //{
        //    if (ConfigurationManagerExtension.AppSettings["web.talk"] != "true")
        //        return null;

        //    return new JabberServiceClient().GetVersion();
        //}
        //catch (Exception e)
        //{
        //    LogManager.GetLogger("ASC").Warn(e.Message, e);
        //}

        return null;
    }
}
