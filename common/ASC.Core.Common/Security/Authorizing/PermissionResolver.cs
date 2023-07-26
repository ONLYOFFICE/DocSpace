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

using Constants = ASC.Core.Configuration.Constants;

namespace ASC.Core.Security.Authorizing;

[Scope]
class PermissionResolver : IPermissionResolver
{
    private readonly AzManager _azManager;

    public PermissionResolver(AzManager azManager)
    {
        _azManager = azManager ?? throw new ArgumentNullException(nameof(azManager));
    }

    public async Task<bool> CheckAsync(ISubject subject, params IAction[] actions)
    {
        return await CheckAsync(subject, null, null, actions);
    }

    public async Task<bool> CheckAsync(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
    {
        var denyActions = await GetDenyActionsAsync(subject, actions, objectId, securityObjProvider);
        return denyActions.Length == 0;
    }

    public async Task DemandAsync(ISubject subject, params IAction[] actions)
    {
        await DemandAsync(subject, null, null, actions);
    }

    public async Task DemandAsync(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
    {
        var denyActions = await GetDenyActionsAsync(subject, actions, objectId, securityObjProvider);
        if (0 < denyActions.Length)
        {
            throw new AuthorizingException(
                subject,
                Array.ConvertAll(denyActions, r => r._targetAction),
                Array.ConvertAll(denyActions, r => r._denySubject),
                Array.ConvertAll(denyActions, r => r._denyAction));
        }
    }


    private async Task<DenyResult[]> GetDenyActionsAsync(ISubject subject, IAction[] actions, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider)
    {
        var denyActions = new List<DenyResult>();
        if (actions == null)
        {
            actions = Array.Empty<IAction>();
        }

        if (subject == null)
        {
            denyActions = actions.Select(a => new DenyResult(a, null, null)).ToList();
        }
        else if (subject is ISystemAccount && subject.ID == Constants.CoreSystem.ID)
        {
            // allow all
        }
        else
        {
            ISubject denySubject = null;
            IAction denyAction = null;
            foreach (var action in actions)
            {
                (var allow, denySubject, denyAction) = await _azManager.CheckPermissionAsync(subject, action, objectId, securityObjProvider);
                if (!allow)
                {
                    denyActions.Add(new DenyResult(action, denySubject, denyAction));
                    break;
                }
            }
        }

        return denyActions.ToArray();
    }

    private class DenyResult
    {
        public readonly IAction _targetAction;
        public readonly ISubject _denySubject;
        public readonly IAction _denyAction;

        public DenyResult(IAction targetAction, ISubject denySubject, IAction denyAction)
        {
            _targetAction = targetAction;
            _denySubject = denySubject;
            _denyAction = denyAction;
        }
    }
}
