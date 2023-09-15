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

namespace ASC.AuditTrail.Mappers;

[Singletone]
public class AuditActionMapper
{
    public List<IProductActionMapper> Mappers { get; }
    private readonly ILogger<AuditActionMapper> _logger;

    public AuditActionMapper(ILogger<AuditActionMapper> logger)
    {
        _logger = logger;

        Mappers = new List<IProductActionMapper>()
            {
                new DocumentsActionMapper(),
                new LoginActionsMapper(),
                new OthersActionsMapper(),
                new PeopleActionMapper(),
                new SettingsActionsMapper()
            };
    }

    public string GetActionText(MessageMaps action, AuditEvent evt)
    {
        if (action == null)
        {
            _logger.ErrorThereIsNoActionText(action);

            return string.Empty;
        }

        try
        {
            var actionText = action.GetActionText();

            if (evt.Description == null || evt.Description.Count == 0)
            {
                return actionText;
            }

            var description = evt.Description
                                 .Select(t => t.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                 .Select(split => string.Join(", ", split.Select(ToLimitedText))).ToArray();


            return string.Format(actionText, description);
        }
        catch
        {
            //log.Error(string.Format("Error while building action text for \"{0}\" type of event", action));
            return string.Empty;
        }
    }

    public string GetActionText(MessageMaps action, LoginEvent evt)
    {
        if (action == null)
        {
            //log.Error(string.Format("There is no action text for \"{0}\" type of event", action));
            return string.Empty;
        }

        try
        {
            var actionText = action.GetActionText();

            if (evt.Description == null || evt.Description.Count == 0)
            {
                return actionText;
            }

            var description = evt.Description
                                 .Select(t => t.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                 .Select(split => string.Join(", ", split.Select(ToLimitedText))).ToArray();

            return string.Format(actionText, description);
        }
        catch
        {
            //log.Error(string.Format("Error while building action text for \"{0}\" type of event", action));
            return string.Empty;
        }
    }

    public string GetActionTypeText(MessageMaps action)
    {
        return action == null
                   ? string.Empty
                   : action.GetActionTypeText();
    }

    public string GetProductText(MessageMaps action)
    {
        return action == null
                   ? string.Empty
                   : action.GetProductText();
    }

    public string GetModuleText(MessageMaps action)
    {
        return action == null
                   ? string.Empty
                   : action.GetModuleText();
    }

    private string ToLimitedText(string text)
    {
        if (text == null)
        {
            return null;
        }

        return text.Length < 50 ? text : $"{text.Substring(0, 47)}...";
    }

    public MessageMaps GetMessageMaps(int actionInt)
    {
        var action = (MessageAction)actionInt;
        var mapper = Mappers.SelectMany(m => m.Mappers).FirstOrDefault(m => m.Actions.ContainsKey(action));
        if (mapper != null)
        {
            return mapper.Actions[action];
        }
        return null;
    }
}