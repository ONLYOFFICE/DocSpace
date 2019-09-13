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
using System.Runtime.Serialization;
using ASC.Core.Tenants;

namespace ASC.Core.Common.Settings
{
    public interface ISettings
    {
        Guid ID { get; }
        ISettings GetDefault();
    }

    [Serializable]
    [DataContract]
    public abstract class BaseSettings<T> : ISettings where T : class, ISettings
    {
        public BaseSettings()
        {

        }

        public BaseSettings(AuthContext authContext, SettingsManager settingsManager, TenantManager tenantManager)
        {
            AuthContext = authContext;
            SettingsManager = settingsManager;
            TenantManager = tenantManager;
        }

        private int TenantID
        {
            get { return TenantManager.GetCurrentTenant().TenantId; }
        }
        //
        private Guid CurrentUserID
        {
            get { return AuthContext.CurrentAccount.ID; }
        }

        public T Load()
        {
            return SettingsManager.LoadSettings<T>(TenantID);
        }

        public T LoadForCurrentUser()
        {
            return LoadForUser(CurrentUserID);
        }

        public T LoadForUser(Guid userId)
        {
            return SettingsManager.LoadSettingsFor<T>(TenantID, userId);
        }

        public T LoadForDefaultTenant()
        {
            return LoadForTenant(Tenant.DEFAULT_TENANT);
        }

        public T LoadForTenant(int tenantId)
        {
            return SettingsManager.LoadSettings<T>(tenantId);
        }

        public virtual bool Save()
        {
            return SettingsManager.SaveSettings(this, TenantID);
        }

        public bool SaveForCurrentUser()
        {
            return SaveForUser(CurrentUserID);
        }

        public bool SaveForUser(Guid userId)
        {
            return SettingsManager.SaveSettingsFor(this, userId);
        }

        public bool SaveForDefaultTenant()
        {
            return SaveForTenant(Tenant.DEFAULT_TENANT);
        }

        public bool SaveForTenant(int tenantId)
        {
            return SettingsManager.SaveSettings(this, tenantId);
        }

        public void ClearCache()
        {
            SettingsManager.ClearCache<T>();
        }

        public abstract Guid ID { get; }
        public AuthContext AuthContext { get; }
        public SettingsManager SettingsManager { get; }
        public TenantManager TenantManager { get; }

        public abstract ISettings GetDefault();
    }
}
