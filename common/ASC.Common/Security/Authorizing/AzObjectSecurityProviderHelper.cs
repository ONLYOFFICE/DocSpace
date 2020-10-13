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


#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Common.Security.Authorizing
{
    public class AzObjectSecurityProviderHelper
    {
        private readonly SecurityCallContext callContext;
        private readonly bool currObjIdAsProvider;
        private ISecurityObjectProvider currSecObjProvider;

        public AzObjectSecurityProviderHelper(ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider)
        {
            currObjIdAsProvider = false;
            CurrentObjectId = objectId ?? throw new ArgumentNullException(nameof(objectId));
            currSecObjProvider = secObjProvider;
            if (currSecObjProvider == null && CurrentObjectId is ISecurityObjectProvider securityObjectProvider)
            {
                currObjIdAsProvider = true;
                currSecObjProvider = securityObjectProvider;
            }
            callContext = new SecurityCallContext();
        }

        public ISecurityObjectId CurrentObjectId { get; private set; }

        public bool ObjectRolesSupported
        {
            get { return currSecObjProvider != null && currSecObjProvider.ObjectRolesSupported; }
        }

        public IEnumerable<IRole> GetObjectRoles(ISubject account)
        {
            var roles = currSecObjProvider.GetObjectRoles(account, CurrentObjectId, callContext);
            foreach (var role in roles)
            {
                if (!callContext.RolesList.Contains(role)) callContext.RolesList.Add(role);
            }
            return roles;
        }

        public bool NextInherit()
        {
            if (currSecObjProvider == null || !currSecObjProvider.InheritSupported) return false;
            CurrentObjectId = currSecObjProvider.InheritFrom(CurrentObjectId);
            if (CurrentObjectId == null) return false;
            if (currObjIdAsProvider)
            {
                currSecObjProvider = CurrentObjectId as ISecurityObjectProvider;
            }
            callContext.ObjectsStack.Insert(0, CurrentObjectId);
            return currSecObjProvider != null;
        }
    }
}