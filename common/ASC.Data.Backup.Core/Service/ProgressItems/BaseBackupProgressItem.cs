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

/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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

namespace ASC.Data.Backup.Services;

public abstract class BaseBackupProgressItem : DistributedTaskProgress
{
    private int? _tenantId;
    private BackupProgressItemEnum? _backupProgressItemEnum;
    private string _link;

    public int TenantId
    {
        get => _tenantId ?? this[nameof(_tenantId)];
        set
        {
            _tenantId = value;
            this[nameof(_tenantId)] = value;
        }
    }

    public string Link
    {
        get
        {
            return _link ?? this[nameof(_link)];
        }
        set
        {
            _link = value;
            this[nameof(_link)] = value;
        }
    }

    public BackupProgressItemEnum BackupProgressItemEnum
    {
        get
        {
            return _backupProgressItemEnum ?? (BackupProgressItemEnum)this[nameof(_backupProgressItemEnum)];
        }
        protected set
        {
            _backupProgressItemEnum = value;

            this[nameof(_backupProgressItemEnum)] = (int)value;
        }
    }

    protected ILogger Logger { get; set; }
    protected IServiceScopeFactory _serviceScopeProvider;

    protected BaseBackupProgressItem(ILogger logger, IServiceScopeFactory serviceScopeFactory)
    {
        Logger = logger;
        _serviceScopeProvider = serviceScopeFactory;

        this[nameof(_tenantId)] = 0;
        this[nameof(_link)] = "";
        this[nameof(_backupProgressItemEnum)] = 0;
    }

    public abstract object Clone();
}
