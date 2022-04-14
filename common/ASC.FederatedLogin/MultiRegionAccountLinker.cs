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

namespace ASC.FederatedLogin;

public class MultiRegionAccountLinker
{
    private readonly Dictionary<string, AccountLinker> _accountLinkers = new Dictionary<string, AccountLinker>();
    //private readonly string _baseDatabaseId;

    public MultiRegionAccountLinker(string databaseId, ConfigurationExtension configuration, IOptionsSnapshot<AccountLinker> snapshot)
    {
        foreach (var connection in configuration.GetConnectionStrings())
        {
            if (connection.Name.StartsWith(databaseId))
            {
                _accountLinkers.Add(connection.Name, snapshot.Get(connection.Name));
            }
        }
    }

    public IEnumerable<string> GetLinkedObjects(string id, string provider)
    {
        return _accountLinkers.Values.SelectMany(x => x.GetLinkedObjects(id, provider));
    }

    public IEnumerable<string> GetLinkedObjects(LoginProfile profile)
    {
        return _accountLinkers.Values.SelectMany(x => x.GetLinkedObjects(profile));
    }

    public IEnumerable<string> GetLinkedObjectsByHashId(string hashid)
    {
        return _accountLinkers.Values.SelectMany(x => x.GetLinkedObjectsByHashId(hashid));
    }

    public void AddLink(string hostedRegion, string obj, LoginProfile profile)
    {
        _accountLinkers[GetDatabaseId(hostedRegion)].AddLink(obj, profile);
    }

    public void AddLink(string hostedRegion, string obj, string id, string provider)
    {
        _accountLinkers[GetDatabaseId(hostedRegion)].AddLink(obj, id, provider);
    }

    public void RemoveLink(string hostedRegion, string obj, string id, string provider)
    {
        _accountLinkers[GetDatabaseId(hostedRegion)].RemoveLink(obj, id, provider);
    }

    public void RemoveLink(string hostedRegion, string obj, LoginProfile profile)
    {
        _accountLinkers[GetDatabaseId(hostedRegion)].RemoveLink(obj, profile);
    }

    public void Unlink(string region, string obj)
    {
        _accountLinkers[GetDatabaseId(region)].RemoveProvider(obj);
    }

    public void RemoveProvider(string hostedRegion, string obj, string provider)
    {
        _accountLinkers[GetDatabaseId(hostedRegion)].RemoveProvider(obj, provider);
    }

    public IEnumerable<LoginProfile> GetLinkedProfiles(string obj)
    {
        return _accountLinkers.Values.SelectMany(x => x.GetLinkedProfiles(obj));
    }

    private string GetDatabaseId(string hostedRegion)
    {
        var databaseId = string.Empty;

        if (!string.IsNullOrEmpty(hostedRegion))
        {
            //databaseId = string.Join(".", new[] { _baseDatabaseId, hostedRegion.Trim() });
            databaseId = string.Join(".", new[] { hostedRegion.Trim() });
        }

        if (!_accountLinkers.ContainsKey(databaseId))
        {
            throw new ArgumentException($"Region {databaseId} is not defined", nameof(hostedRegion));
        }

        return databaseId;
    }
}
