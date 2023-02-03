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

namespace ASC.Core;

[DebuggerDisplay("{UserId} - {GroupId}")]
public class UserGroupRef : IMapFrom<UserGroup>
{
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public bool Removed { get; set; }
    public DateTime LastModified { get; set; }
    public UserGroupRefType RefType { get; set; }
    public int TenantId { get; set; }

    public UserGroupRef() { }

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
        return CreateKey(TenantId, UserId, GroupId, RefType);
    }

    public override int GetHashCode()
    {
        return UserId.GetHashCode() ^ GroupId.GetHashCode() ^ TenantId.GetHashCode() ^ RefType.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is UserGroupRef r && r.TenantId == TenantId && r.UserId == UserId && r.GroupId == GroupId && r.RefType == RefType;
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<UserGroup, UserGroupRef>()
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.UserGroupId));
    }

    public static implicit operator UserGroupRef(UserGroupRefCacheItem cache)
    {
        var result = new UserGroupRef
        {
            UserId = new Guid(cache.UserId),
            GroupId = new Guid(cache.GroupId)
        };

        if (UserGroupRefTypeExtensions.TryParse(cache.RefType, out var refType))
        {
            result.RefType = refType;
        }

        result.TenantId = cache.Tenant;
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
            Tenant = cache.TenantId
        };
    }
}
