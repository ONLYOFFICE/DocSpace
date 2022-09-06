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

namespace ASC.Data.Backup.Core.IntegrationEvents.Events;

[ProtoContract]
public record BackupRequestIntegrationEvent : IntegrationEvent
{
    private BackupRequestIntegrationEvent() : base()
    {
        StorageParams = new Dictionary<string, string>();
    }

    public BackupRequestIntegrationEvent(BackupStorageType storageType,
                                  int tenantId,
                                  Guid createBy,
                                  Dictionary<string, string> storageParams,
                                  bool backupMail,
                                  bool isScheduled = false,
                                  int backupsStored = 0,
                                  string storageBasePath = "") : base(createBy, tenantId)
    {
        StorageType = storageType;
        StorageParams = storageParams;
        BackupMail = backupMail;
        IsScheduled = isScheduled;
        BackupsStored = backupsStored;
        StorageBasePath = storageBasePath;
    }

    [ProtoMember(1)]
    public BackupStorageType StorageType { get; private init; }

    [ProtoMember(2)]
    public Dictionary<string, string> StorageParams { get; private init; }

    [ProtoMember(3)]
    public bool BackupMail { get; private init; }

    [ProtoMember(4)]
    public bool IsScheduled { get; private init; }

    [ProtoMember(5)]
    public int BackupsStored { get; private init; }

    [ProtoMember(6)]
    public string StorageBasePath { get; private init; }
}

