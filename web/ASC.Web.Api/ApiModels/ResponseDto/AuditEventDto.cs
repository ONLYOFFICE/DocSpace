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

namespace ASC.Web.Api.ApiModel.ResponseDto;

/// <summary>
/// </summary>
public class AuditEventDto
{
    /// <summary>ID</summary>
    /// <type>System.Int32, System</type>
    public int Id { get; set; }

    /// <summary>Date</summary>
    /// <type>ASC.Api.Core.ApiDateTime, ASC.Api.Core</type>
    public ApiDateTime Date { get; set; }

    /// <summary>User</summary>
    /// <type>System.String, System</type>
    public string User { get; set; }

    /// <summary>User ID</summary>
    /// <type>System.Guid, System</type>
    public Guid UserId { get; set; }

    /// <summary>Action</summary>
    /// <type>System.String, System</type>
    public string Action { get; set; }

    /// <summary>Action ID</summary>
    /// <type>ASC.MessagingSystem.Core.MessageAction, ASC.Core.Common</type>
    public MessageAction ActionId { get; set; }

    /// <summary>IP</summary>
    /// <type>System.String, System</type>
    public string IP { get; set; }

    /// <summary>Browser</summary>
    /// <type>System.String, System</type>
    public string Browser { get; set; }

    /// <summary>Platform</summary>
    /// <type>System.String, System</type>
    public string Platform { get; set; }

    /// <summary>Page</summary>
    /// <type>System.String, System</type>
    public string Page { get; set; }

    /// <summary>Action type</summary>
    /// <type>ASC.AuditTrail.Types.ActionType, ASC.AuditTrail</type>
    public ActionType ActionType { get; set; }

    /// <summary>Product type</summary>
    /// <type>ASC.AuditTrail.Types.ProductType, ASC.AuditTrail</type>
    public ProductType Product { get; set; }

    /// <summary>Module type</summary>
    /// <type>ASC.AuditTrail.Types.ModuleType, ASC.AuditTrail</type>
    public ModuleType Module { get; set; }

    /// <summary>List of targets</summary>
    /// <type>System.Collections.Generic.IEnumerable{System.String}, System.Collections.Generic</type>
    public IEnumerable<string> Target { get; set; }

    /// <summary>List of entry types</summary>
    /// <type>System.Collections.Generic.IEnumerable{ASC.AuditTrail.Types.EntryType}, System.Collections.Generic</type>
    public IEnumerable<EntryType> Entries { get; set; }

    /// <summary>Context</summary>
    /// <type>System.String, System</type>
    public string Context { get; set; }

    public AuditEventDto(AuditTrail.Models.AuditEventDto auditEvent, AuditActionMapper auditActionMapper)
    {
        Id = auditEvent.Id;
        Date = new ApiDateTime(auditEvent.Date, TimeSpan.Zero);
        User = auditEvent.UserName;
        UserId = auditEvent.UserId;
        Action = auditEvent.ActionText;
        ActionId = (MessageAction)auditEvent.Action;
        IP = auditEvent.IP;
        Browser = auditEvent.Browser;
        Platform = auditEvent.Platform;
        Page = auditEvent.Page;

        var maps = auditActionMapper.GetMessageMaps(auditEvent.Action);

        ActionType = maps.ActionType;
        Product = maps.ProductType;
        Module = maps.ModuleType;

        var list = new List<EntryType>(2);

        if (maps.EntryType1 != EntryType.None)
        {
            list.Add(maps.EntryType1);
        }

        if (maps.EntryType2 != EntryType.None)
        {
            list.Add(maps.EntryType2);
        }

        Entries = list;

        if (auditEvent.Target != null)
        {
            Target = auditEvent.Target.GetItems();
        }

        Context = auditEvent.Context;
    }
}