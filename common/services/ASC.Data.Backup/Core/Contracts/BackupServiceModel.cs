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
using System.Collections.Generic;

namespace ASC.Data.Backup.Contracts
{
    public enum BackupStorageType
    {
        Documents = 0,

        ThridpartyDocuments = 1,

        CustomCloud = 2,

        Local = 3,

        DataStore = 4,

        ThirdPartyConsumer = 5
    }

    public class StartBackupRequest
    {
        public int TenantId { get; set; }


        public Guid UserId { get; set; }


        public bool BackupMail { get; set; }


        public BackupStorageType StorageType { get; set; }


        public string StorageBasePath { get; set; }


        public Dictionary<string, string> StorageParams { get; set; }
    }


    public class BackupHistoryRecord
    {

        public Guid Id { get; set; }


        public string FileName { get; set; }


        public BackupStorageType StorageType { get; set; }


        public DateTime CreatedOn { get; set; }


        public DateTime ExpiresOn { get; set; }
    }


    public class StartTransferRequest
    {

        public int TenantId { get; set; }


        public string TargetRegion { get; set; }


        public bool NotifyUsers { get; set; }


        public bool BackupMail { get; set; }
    }


    public class TransferRegion
    {

        public string Name { get; set; }


        public string BaseDomain { get; set; }


        public bool IsCurrentRegion { get; set; }
    }


    public class StartRestoreRequest
    {

        public int TenantId { get; set; }


        public Guid BackupId { get; set; }


        public BackupStorageType StorageType { get; set; }


        public string FilePathOrId { get; set; }


        public bool NotifyAfterCompletion { get; set; }


        public Dictionary<string, string> StorageParams { get; set; }
    }


    public class CreateScheduleRequest : StartBackupRequest
    {

        public string Cron { get; set; }


        public int NumberOfBackupsStored { get; set; }
    }


    public class ScheduleResponse
    {

        public BackupStorageType StorageType { get; set; }


        public string StorageBasePath { get; set; }


        public bool BackupMail { get; set; }


        public int NumberOfBackupsStored { get; set; }


        public string Cron { get; set; }


        public DateTime LastBackupTime { get; set; }


        public Dictionary<string, string> StorageParams { get; set; }
    }
}