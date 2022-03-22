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

namespace ASC.AuditTrail.Models.Mappings;

[Scope]
public class EventTypeConverter : ITypeConverter<LoginEventQuery, LoginEventDto>, 
                                  ITypeConverter<AuditEventQuery, AuditEventDto>
{
    private readonly UserFormatter _userFormatter;
    private readonly AuditActionMapper _auditActionMapper;
    private readonly MessageTarget _messageTarget;

    public EventTypeConverter(
        UserFormatter userFormatter,
        AuditActionMapper actionMapper,
        MessageTarget messageTarget)
    {
        _userFormatter = userFormatter;
        _auditActionMapper = actionMapper;
        _messageTarget = messageTarget;
    }

    public LoginEventDto Convert(LoginEventQuery source, LoginEventDto destination, ResolutionContext context)
    {
        var result = context.Mapper.Map<LoginEventDto>(source.Event);

        if (source.Event.DescriptionRaw != null)
        {
            result.Description = JsonConvert.DeserializeObject<IList<string>>(source.Event.DescriptionRaw,
                new JsonSerializerSettings
                {
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                });
        }

        result.UserName = (!string.IsNullOrEmpty(source.FirstName) && !string.IsNullOrEmpty(source.LastName))
                        ? _userFormatter.GetUserName(source.FirstName, source.LastName)
                        : !string.IsNullOrWhiteSpace(source.Event.Login)
                                ? source.Event.Login
                                : source.Event.UserId == Core.Configuration.Constants.Guest.ID
                                    ? AuditReportResource.GuestAccount
                                    : AuditReportResource.UnknownAccount;

        result.ActionText = _auditActionMapper.GetActionText(result);

        return result;
    }

    public AuditEventDto Convert(AuditEventQuery source, AuditEventDto destination, ResolutionContext context)
    {
        var result = context.Mapper.Map<AuditEventDto>(source.Event);

        if (source.Event.DescriptionRaw != null)
        {
            result.Description = JsonConvert.DeserializeObject<IList<string>>(
               JsonConvert.ToString(source.Event.DescriptionRaw),
               new JsonSerializerSettings
               {
                   DateTimeZoneHandling = DateTimeZoneHandling.Utc
               });
        }

        result.Target = _messageTarget.Parse(source.Event.Target);

        result.UserName = (source.FirstName != null && source.LastName != null) ? _userFormatter.GetUserName(source.FirstName, source.LastName) :
                source.Event.UserId == Core.Configuration.Constants.CoreSystem.ID ? AuditReportResource.SystemAccount :
                    source.Event.UserId == Core.Configuration.Constants.Guest.ID ? AuditReportResource.GuestAccount :
                        source.Event.Initiator ?? AuditReportResource.UnknownAccount;

        result.ActionText = _auditActionMapper.GetActionText(result);
        result.ActionTypeText = _auditActionMapper.GetActionTypeText(result);
        result.Product = _auditActionMapper.GetProductText(result);
        result.Module = _auditActionMapper.GetModuleText(result);

        return result;
    }
}
