/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;

using ASC.Common.Utils;
using ASC.FederatedLogin.Profile;

using Microsoft.Extensions.Options;

namespace ASC.FederatedLogin
{
    public class MultiRegionAccountLinker
    {
        private readonly Dictionary<string, AccountLinker> _accountLinkers = new Dictionary<string, AccountLinker>();
        private readonly string _baseDatabaseId = null;

        private string GetDatabaseId(string hostedRegion)
        {
            var databaseId = _baseDatabaseId;

            if (!string.IsNullOrEmpty(hostedRegion))
                databaseId = string.Join(".", new[] { _baseDatabaseId, hostedRegion.Trim() });

            if (!_accountLinkers.ContainsKey(databaseId))
                throw new ArgumentException($"Region {databaseId} is not defined", nameof(hostedRegion));

            return databaseId;
        }


        public MultiRegionAccountLinker(string databaseId, ConfigurationExtension configuration, IOptionsSnapshot<AccountLinker> snapshot)
        {
            foreach (var connection in configuration.GetConnectionStrings())
            {
                if (connection.Name.StartsWith(databaseId))
                    _accountLinkers.Add(connection.Name, snapshot.Get(connection.Name));
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
    }
}