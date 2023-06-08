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

namespace ASC.Web.Files.ThirdPartyApp;

public interface IThirdPartyApp
{
    Task<bool> RequestAsync(HttpContext context);
    string GetRefreshUrl();
    Task<(File<string>, bool)> GetFileAsync(string fileId);
    string GetFileStreamUrl(File<string> file);
    Task SaveFileAsync(string fileId, string fileType, string downloadUrl, Stream stream);
}

[Scope(Additional = typeof(ThirdPartySelectorExtension))]
public class ThirdPartySelector
{
    public const string AppAttr = "app";
    public static readonly Regex AppRegex = new Regex("^" + AppAttr + @"-(\S+)\|(\S+)$", RegexOptions.Singleline | RegexOptions.Compiled);
    private readonly ConsumerFactory _consumerFactory;

    public ThirdPartySelector(ConsumerFactory consumerFactory)
    {
        _consumerFactory = consumerFactory;
    }

    public static string BuildAppFileId(string app, object fileId)
    {
        return AppAttr + "-" + app + "|" + fileId;
    }

    public static string GetFileId(string appFileId)
    {
        return AppRegex.Match(appFileId).Groups[2].Value;
    }

    public IThirdPartyApp GetAppByFileId(string fileId)
    {
        if (string.IsNullOrEmpty(fileId))
        {
            return null;
        }

        var match = AppRegex.Match(fileId);

        return match.Success ? GetApp(match.Groups[1].Value) : null;
    }

    public IThirdPartyApp GetApp(string app)
    {
        return app switch
        {
            GoogleDriveApp.AppAttr => _consumerFactory.Get<GoogleDriveApp>(),
            BoxApp.AppAttr => _consumerFactory.Get<BoxApp>(),
            _ => _consumerFactory.Get<GoogleDriveApp>(),
        };
    }
}
public class ThirdPartySelectorExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<GoogleDriveApp>();
        services.TryAdd<BoxApp>();
    }
}
