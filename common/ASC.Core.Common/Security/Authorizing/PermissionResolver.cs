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

    public bool Check(ISubject subject, params IAction[] actions)
    {
        return Check(subject, null, null, actions);
    }

    public bool Check(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
    {
        var denyActions = GetDenyActions(subject, actions, objectId, securityObjProvider);
        return denyActions.Length == 0;
    }

    public void Demand(ISubject subject, params IAction[] actions)
    {
        Demand(subject, null, null, actions);
    }

    public void Demand(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
    {
        var denyActions = GetDenyActions(subject, actions, objectId, securityObjProvider);
        if (0 < denyActions.Length)
        {
            throw new AuthorizingException(
                subject,
                Array.ConvertAll(denyActions, r => r._targetAction),
                Array.ConvertAll(denyActions, r => r._denySubject),
                Array.ConvertAll(denyActions, r => r._denyAction));
        }
    }


    private DenyResult[] GetDenyActions(ISubject subject, IAction[] actions, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider)
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
                var allow = _azManager.CheckPermission(subject, action, objectId, securityObjProvider, out denySubject, out denyAction);
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
