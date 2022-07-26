﻿// (c) Copyright Ascensio System SIA 2010-2022
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


namespace ASC.MessagingSystem.Mapping;

[Scope]
public class EventTypeConverter : ITypeConverter<EventMessage, LoginEvent>, ITypeConverter<EventMessage, AuditEvent>
{
    public LoginEvent Convert(EventMessage source, LoginEvent destination, ResolutionContext context)
    {
        var messageEvent = context.Mapper.Map<EventMessage, MessageEvent>(source);
        var loginEvent = context.Mapper.Map<MessageEvent, LoginEvent>(messageEvent);

        loginEvent.Login = source.Initiator;
        loginEvent.Active = source.Active;

        if (source.Description != null && source.Description.Count > 0)
        {
            loginEvent.DescriptionRaw =
                JsonConvert.SerializeObject(source.Description, new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
        }

        return loginEvent;
    }

    public AuditEvent Convert(EventMessage source, AuditEvent destination, ResolutionContext context)
    {
        var messageEvent = context.Mapper.Map<EventMessage, MessageEvent>(source);
        var auditEvent = context.Mapper.Map<MessageEvent, AuditEvent>(messageEvent);

        auditEvent.Initiator = source.Initiator;
        auditEvent.Target = source.Target?.ToString();

        if (source.Description != null && source.Description.Count > 0)
        {
            auditEvent.DescriptionRaw =
                JsonConvert.SerializeObject(GetSafeDescription(source.Description), new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
        }

        return auditEvent;
    }

    private static IList<string> GetSafeDescription(IEnumerable<string> description)
    {
        const int maxLength = 15000;

        var currentLength = 0;
        var safe = new List<string>();

        foreach (var d in description.Where(r => r != null))
        {
            if (currentLength + d.Length <= maxLength)
            {
                currentLength += d.Length;
                safe.Add(d);
            }
            else
            {
                safe.Add(d.Substring(0, maxLength - currentLength - 3) + "...");
                break;
            }
        }

        return safe;
    }
}
