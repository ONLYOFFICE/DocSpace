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
using System.Diagnostics;

using ASC.Common.Caching;
using ASC.Core.Caching;

namespace ASC.Core
{
    [DebuggerDisplay("{UserId} - {GroupId}")]
    public class UserGroupRef
    {
        public Guid UserId { get; set; }

        public Guid GroupId { get; set; }

        public bool Removed { get; set; }

        public DateTime LastModified { get; set; }

        public UserGroupRefType RefType { get; set; }

        public int Tenant { get; set; }


        public UserGroupRef()
        {
        }

        public UserGroupRef(Guid userId, Guid groupId, UserGroupRefType refType)
        {
            UserId = userId;
            GroupId = groupId;
            RefType = refType;
        }

        public static string CreateKey(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
        {
            return tenant.ToString() + userId.ToString("N") + groupId.ToString("N") + ((int)refType).ToString();
        }

        public string CreateKey()
        {
            return CreateKey(Tenant, UserId, GroupId, RefType);
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode() ^ GroupId.GetHashCode() ^ Tenant.GetHashCode() ^ RefType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is UserGroupRef r && r.Tenant == Tenant && r.UserId == UserId && r.GroupId == GroupId && r.RefType == RefType;
        }

        public static implicit operator UserGroupRef(UserGroupRefCacheItem cache)
        {
            var result = new UserGroupRef
            {
                UserId = new Guid(cache.UserId),
                GroupId = new Guid(cache.GroupId)
            };

            if (Enum.TryParse<UserGroupRefType>(cache.RefType, out var refType))
            {
                result.RefType = refType;
            }

            result.Tenant = cache.Tenant;
            result.LastModified = new DateTime(cache.LastModified);
            result.Removed = cache.Removed;

            return result;
        }

        public static implicit operator UserGroupRefCacheItem(UserGroupRef cache)
        {
            return new UserGroupRefCacheItem
            {
                GroupId = cache.GroupId.ToString(),
                UserId = cache.UserId.ToString(),
                RefType = cache.RefType.ToString(),
                LastModified = cache.LastModified.Ticks,
                Removed = cache.Removed,
                Tenant = cache.Tenant
            };
        }
    }
}
