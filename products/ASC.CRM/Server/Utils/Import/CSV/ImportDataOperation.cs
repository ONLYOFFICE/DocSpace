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
using System.Linq;
using System.Text.Json;

using ASC.Common;
using ASC.Common.Security.Authentication;
using ASC.Common.Threading;
using ASC.Common.Threading.Progress;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.CRM.Core;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Enums;
using ASC.CRM.Resources;
using ASC.Data.Storage;
using ASC.Web.CRM.Services.NotifyService;

using Microsoft.Extensions.Logging;

namespace ASC.Web.CRM.Classes
{
    [Transient]
    public partial class ImportDataOperation : DistributedTaskProgress, IProgressItem
    {
        private readonly ILogger _log;
        private readonly IDataStore _dataStore;
        private readonly IAccount _author;
        private readonly int _tenantID;
        private string _csvFileURI;
        private ImportCSVSettings _importSettings;
        private EntityType _entityType;
        private string[] _columns;
        private bool _IsConfigure;

        private readonly CurrencyProvider _currencyProvider;
        private readonly NotifyClient _notifyClient;
        private readonly SettingsManager _settingsManager;
        private readonly CrmSecurity _crmSecurity;
        private readonly TenantManager _tenantManager;
        private readonly SecurityContext _securityContext;
        private readonly UserManager _userManager;
        private readonly DaoFactory _daoFactory;

        public ImportDataOperation(Global global,
                                   TenantManager tenantManager,
                                   ILogger logger,
                                   UserManager userManager,
                                   CrmSecurity crmSecurity,
                                   NotifyClient notifyClient,
                                   SettingsManager settingsManager,
                                   CurrencyProvider currencyProvider,
                                   DaoFactory daoFactory,
                                   SecurityContext securityContext
                                 )
        {
            _userManager = userManager;

            _securityContext = securityContext;
            _dataStore = global.GetStore();

            _tenantManager = tenantManager;
            _tenantID = tenantManager.GetCurrentTenant().Id;
            _author = _securityContext.CurrentAccount;

            _notifyClient = notifyClient;

            Id = String.Format("{0}_{1}", _tenantID, (int)_entityType);

            _log = logger;

            _crmSecurity = crmSecurity;
            _settingsManager = settingsManager;
            _currencyProvider = currencyProvider;
            _daoFactory = daoFactory;
        }

        public void Configure(EntityType entityType,
                              string CSVFileURI,
                              string importSettingsJSON)
        {

            _entityType = entityType;
            _csvFileURI = CSVFileURI;

            if (!String.IsNullOrEmpty(importSettingsJSON))
                _importSettings = new ImportCSVSettings(importSettingsJSON);

            _IsConfigure = true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ImportDataOperation)) return false;

            var dataOperation = (ImportDataOperation)obj;

            if (_tenantID == dataOperation._tenantID && _entityType == dataOperation._entityType) return true;

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _tenantID.GetHashCode() + _entityType.GetHashCode();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }


        public object Error { get; set; }

        private String GetPropertyValue(String propertyName)
        {
            JsonElement jsonElement;

            if (!_importSettings.ColumnMapping.TryGetProperty(propertyName, out jsonElement)) return String.Empty;

            var values = jsonElement.EnumerateArray().Select(x => x.GetInt32()).ToList().ConvertAll(columnIndex => _columns[columnIndex]);

            values.RemoveAll(item => item.Length == 0);

            return String.Join(",", values.ToArray());
        }

        private void Complete()
        {
            IsCompleted = true;

            Percentage = 100;

            _log.LogDebug("Import is completed");

            _notifyClient.SendAboutImportCompleted(_author.ID, _entityType);
        }

        protected override void DoJob()
        {
            try
            {
                if (!_IsConfigure)
                    throw new Exception("Is not configure. Please, call configure method.");

                _tenantManager.SetCurrentTenant(_tenantID);
                _securityContext.AuthenticateMeWithoutCookie(_author);

                var userCulture = _userManager.GetUsers(_securityContext.CurrentAccount.ID).GetCulture();

                System.Threading.Thread.CurrentThread.CurrentCulture = userCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = userCulture;

                switch (_entityType)
                {
                    case EntityType.Contact:
                        ImportContactsData(_daoFactory);
                        break;
                    case EntityType.Opportunity:
                        ImportOpportunityData(_daoFactory);
                        break;
                    case EntityType.Case:
                        ImportCaseData(_daoFactory);
                        break;
                    case EntityType.Task:
                        ImportTaskData(_daoFactory);
                        break;
                    default:
                        throw new ArgumentException(CRMErrorsResource.EntityTypeUnknown);
                }

            }
            catch (OperationCanceledException)
            {
                _log.LogDebug("Queue canceled");
            }
        }
    }

}