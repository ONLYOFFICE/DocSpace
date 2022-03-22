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

namespace ASC.Core.Common.Notify;

/// <summary>
/// Class that generates 'mail to' addresses to create new TeamLab entities from post client 
/// </summary>
public class ReplyToTagProvider
{
    private static readonly Regex _entityType = new Regex(@"blog|forum.topic|event|photo|file|wiki|bookmark|project\.milestone|project\.task|project\.message");

    private const string _tagName = "replyto";

    public ReplyToTagProvider(TenantManager tenantManager, CoreBaseSettings coreBaseSettings, CoreSettings coreSettings)
    {
        _tenantManager = tenantManager;
        _coreBaseSettings = coreBaseSettings;
        _coreSettings = coreSettings;
    }

    /// <summary>
    /// Creates 'replyto' tag that can be used to comment some TeamLab entity
    /// </summary>
    /// <param name="entity">Name of entity e.g. 'blog', 'project.task', etc.</param>
    /// <param name="entityId">Uniq id of the entity</param>
    /// <returns>New TeamLab tag</returns>
    public TagValue Comment(string entity, string entityId)
    {
        return Comment(entity, entityId, null);
    }

    /// <summary>
    /// Creates 'replyto' tag that can be used to comment some TeamLab entity
    /// </summary>
    /// <param name="entity">Name of entity e.g. 'blog', 'project.task', etc.</param>
    /// <param name="entityId">Uniq id of the entity</param>
    /// <param name="parentId">Comment's parent comment id</param>
    /// <returns>New TeamLab tag</returns>
    public TagValue Comment(string entity, string entityId, string parentId)
    {
        if (string.IsNullOrEmpty(entity) || !_entityType.Match(entity).Success)
        {
            throw new ArgumentException(@"Not supported entity type", entity);
        }

        if (string.IsNullOrEmpty(entityId))
        {
            throw new ArgumentException(@"Entity Id is null or empty", entityId);
        }

        var pId = parentId != Guid.Empty.ToString() && parentId != null ? parentId : string.Empty;

        return new TagValue(_tagName, $"reply_{entity}_{entityId}_{pId}@{AutoreplyDomain}");
    }

    /// <summary>
    /// Creates 'replyto' tag that can be used to create TeamLab project message
    /// </summary>
    /// <param name="projectId">Id of the project to create message</param>
    /// <returns>New TeamLab tag</returns>
    public TagValue Message(int projectId)
    {
        return new TagValue(_tagName, string.Format("message_{0}@{1}", projectId, AutoreplyDomain));
    }

    private string AutoreplyDomain
    {
        get
        {
            // we use mapped domains for standalone portals because it is the only way to reach autoreply service
            // mapped domains are no allowed for SAAS because of http(s) problem
            var tenant = _tenantManager.GetCurrentTenant();

            return tenant.GetTenantDomain(_coreSettings, _coreBaseSettings.Standalone);
        }
    }

    private readonly TenantManager _tenantManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly CoreSettings _coreSettings;
}
