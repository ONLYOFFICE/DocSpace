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

namespace ASC.Core.Common.Notify.Push;

public class PushNotification
{
    public int Id { get; set; }
    public string Message { get; set; }
    public string ShortMessage { get; set; }
    public int? Badge { get; set; }
    public PushModule Module { get; set; }
    public PushAction Action { get; set; }
    public PushItem Item { get; set; }
    public PushItem ParentItem { get; set; }
    public DateTime QueuedOn { get; set; }

    public static PushNotification ApiNotification(string message, int? badge)
    {
        return new PushNotification { Message = message, Badge = badge };
    }
}

public class PushItem
{
    public PushItemType Type { get; set; }
    public string ID { get; set; }
    public string Description { get; set; }

    public PushItem() { }

    public PushItem(PushItemType type, string id, string description)
    {
        Type = type;
        ID = id;
        Description = description;
    }
}
