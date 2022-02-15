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

namespace ASC.Common.Security.Authorizing;

[Scope]
public class AzManager
{
    private readonly IPermissionProvider _permissionProvider;
    private readonly IRoleProvider _roleProvider;

    internal AzManager() { }

    public AzManager(IRoleProvider roleProvider, IPermissionProvider permissionProvider)
        : this()
    {
        _roleProvider = roleProvider ?? throw new ArgumentNullException(nameof(roleProvider));
        _permissionProvider = permissionProvider ?? throw new ArgumentNullException(nameof(permissionProvider));
    }

    public bool CheckPermission(ISubject subject, IAction action, ISecurityObjectId objectId,
                                ISecurityObjectProvider securityObjProvider, out ISubject denySubject,
                                out IAction denyAction)
    {
        if (subject == null)
        {
            throw new ArgumentNullException(nameof(subject));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        var acl = GetAzManagerAcl(subject, action, objectId, securityObjProvider);
        denySubject = acl.DenySubject;
        denyAction = acl.DenyAction;

        return acl.IsAllow;
    }

    internal AzManagerAcl GetAzManagerAcl(ISubject subject, IAction action, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider)
    {
        if (action.AdministratorAlwaysAllow && (Constants.Admin.ID == subject.ID || _roleProvider.IsSubjectInRole(subject, Constants.Admin)))
        {
            return AzManagerAcl.Allow;
        }

        var acl = AzManagerAcl.Default;
        var exit = false;

        foreach (var s in GetSubjects(subject, objectId, securityObjProvider))
        {
            var aceList = _permissionProvider.GetAcl(s, action, objectId, securityObjProvider);
            foreach (var ace in aceList)
            {
                if (ace.Reaction == AceType.Deny)
                {
                    acl.IsAllow = false;
                    acl.DenySubject = s;
                    acl.DenyAction = action;
                    exit = true;
                }
                if (ace.Reaction == AceType.Allow && !exit)
                {
                    acl.IsAllow = true;
                    if (!action.Conjunction)
                    {
                        // disjunction: first allow and exit
                        exit = true;
                    }
                }
                if (exit)
                {
                    break;
                }
            }
            if (exit)
            {
                break;
            }
        }

        return acl;
    }

    internal IEnumerable<ISubject> GetSubjects(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider)
    {
        var subjects = new List<ISubject>
            {
                subject
            };
        subjects.AddRange(
            _roleProvider.GetRoles(subject)
                .ConvertAll(r => { return (ISubject)r; })
            );
        if (objectId != null)
        {
            var secObjProviderHelper = new AzObjectSecurityProviderHelper(objectId, securityObjProvider);
            do
            {
                if (!secObjProviderHelper.ObjectRolesSupported)
                {
                    continue;
                }

                foreach (var role in secObjProviderHelper.GetObjectRoles(subject))
                {
                    if (!subjects.Contains(role))
                    {
                        subjects.Add(role);
                    }
                }
            } while (secObjProviderHelper.NextInherit());
        }

        return subjects;
    }

    #region Nested type: AzManagerAcl

    internal class AzManagerAcl
    {
        public IAction DenyAction { get; set; }
        public ISubject DenySubject { get; set; }
        public bool IsAllow { get; set; }
        public static AzManagerAcl Allow => new AzManagerAcl { IsAllow = true };
        public static AzManagerAcl Default => new AzManagerAcl { IsAllow = false };
    }

    #endregion
}
