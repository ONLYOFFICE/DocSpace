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

using ASC.Core.Tenants;

namespace ASC.AuditTrail.Models.Mappings;

[Scope]
internal class EventTypeConverter : ITypeConverter<LoginEventQuery, LoginEvent>,
                                  ITypeConverter<AuditEventQuery, AuditEvent>
{
    private readonly UserFormatter _userFormatter;
    private readonly AuditActionMapper _auditActionMapper;
    private readonly MessageTarget _messageTarget;
    private readonly TenantUtil _tenantUtil;

    public EventTypeConverter(
        UserFormatter userFormatter,
        AuditActionMapper actionMapper,
        MessageTarget messageTarget,
        TenantUtil tenantUtil)
    {
        _userFormatter = userFormatter;
        _auditActionMapper = actionMapper;
        _messageTarget = messageTarget;
        _tenantUtil = tenantUtil;
    }

    public LoginEvent Convert(LoginEventQuery source, LoginEvent destination, ResolutionContext context)
    {
        var result = context.Mapper.Map<LoginEvent>(source.Event);

        if (source.Event.DescriptionRaw != null)
        {
            result.Description = JsonConvert.DeserializeObject<IList<string>>(source.Event.DescriptionRaw,
                new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
        }

        if (!(string.IsNullOrEmpty(source.FirstName) || string.IsNullOrEmpty(source.LastName)))
        {
            result.UserName = _userFormatter.GetUserName(source.FirstName, source.LastName);
        }
        else if (!string.IsNullOrEmpty(source.FirstName))
        {
            result.UserName = source.FirstName;
        }
        else if (!string.IsNullOrEmpty(source.LastName))
        {
            result.UserName = source.LastName;
        }
        else if (!string.IsNullOrWhiteSpace(result.Login))
        {
            result.UserName = result.Login;
        }
        else if (result.UserId == Core.Configuration.Constants.Guest.ID)
        {
            result.UserName = AuditReportResource.GuestAccount;
        }
        else
        {
            result.UserName = AuditReportResource.UnknownAccount;
        }

        result.ActionText = _auditActionMapper.GetActionText(_auditActionMapper.GetMessageMaps(result.Action), result);

        result.Date = _tenantUtil.DateTimeFromUtc(result.Date);
        result.IP = result.IP.Split(':').Length > 1 ? result.IP.Split(':')[0] : result.IP;

        return result;
    }

    public AuditEvent Convert(AuditEventQuery source, AuditEvent destination, ResolutionContext context)
    {
        var target = source.Event.Target;
        source.Event.Target = null;
        var result = context.Mapper.Map<AuditEvent>(source.Event);

        result.Target = _messageTarget.Parse(target);

        if (source.Event.DescriptionRaw != null)
        {
            result.Description = JsonConvert.DeserializeObject<IList<string>>(
               source.Event.DescriptionRaw,
               new JsonSerializerSettings
               {
                   DateTimeZoneHandling = DateTimeZoneHandling.Utc
               });
        }

        if (result.UserId == Core.Configuration.Constants.CoreSystem.ID)
        {
            result.UserName = AuditReportResource.SystemAccount;
        }
        else if (result.UserId == Core.Configuration.Constants.Guest.ID)
        {
            result.UserName = AuditReportResource.GuestAccount;
        }
        else if (!(string.IsNullOrEmpty(source.FirstName) || string.IsNullOrEmpty(source.LastName)))
        {
            result.UserName = _userFormatter.GetUserName(source.FirstName, source.LastName);
        }
        else if (!string.IsNullOrEmpty(source.FirstName))
        {
            result.UserName = source.FirstName;
        }
        else if (!string.IsNullOrEmpty(source.LastName))
        {
            result.UserName = source.LastName;
        }
        else
        {
            result.UserName = result.Initiator ?? AuditReportResource.UnknownAccount;
        }

        var map = _auditActionMapper.GetMessageMaps(result.Action);
        if (map != null)
        {
            result.ActionText = _auditActionMapper.GetActionText(map, result);
            result.ActionTypeText = _auditActionMapper.GetActionTypeText(map);
            result.Product = _auditActionMapper.GetProductText(map);
            result.Module = _auditActionMapper.GetModuleText(map);
        }


        result.Date = _tenantUtil.DateTimeFromUtc(result.Date);
        if (!string.IsNullOrEmpty(result.IP))
        {
            var ipSplited = result.IP.Split(':');
            if (ipSplited.Length > 1)
            {
                result.IP = ipSplited[0];
            }
        }

        if (map?.ProductType == ProductType.Documents)
        {
            var rawNotificationInfo = result.Description?.LastOrDefault();

            if (!string.IsNullOrEmpty(rawNotificationInfo) && rawNotificationInfo.StartsWith('{') && rawNotificationInfo.EndsWith('}'))
            {
                var notificationInfo = System.Text.Json.JsonSerializer.Deserialize<AdditionalNotificationInfo>(rawNotificationInfo);

                result.Context = result.Action == (int)MessageAction.RoomRenamed ? notificationInfo.RoomOldTitle :
                    !string.IsNullOrEmpty(notificationInfo.RoomTitle) ? notificationInfo.RoomTitle : notificationInfo.RootFolderTitle;
            }
        }

        if (string.IsNullOrEmpty(result.Context))
        {
            result.Context = result.Module;
        }

        return result;
    }
}
