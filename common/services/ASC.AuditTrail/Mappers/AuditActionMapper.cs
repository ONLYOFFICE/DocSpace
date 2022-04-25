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
    private readonly Dictionary<MessageAction, MessageMaps> _actions;
    private readonly ILog _logger;

    public AuditActionMapper(ILog logger)
    {
        _actions = new Dictionary<MessageAction, MessageMaps>();
        _logger = logger;

        _actions = _actions
            .Union(LoginActionsMapper.GetMaps())
            .Union(ProjectsActionsMapper.GetMaps())
            .Union(CrmActionMapper.GetMaps())
            .Union(PeopleActionMapper.GetMaps())
            .Union(DocumentsActionMapper.GetMaps())
            .Union(SettingsActionsMapper.GetMaps())
            .Union(OthersActionsMapper.GetMaps())
            .ToDictionary(x => x.Key, x => x.Value);
    }

    public string GetActionText(AuditEventDto evt)
    {
        var action = (MessageAction)evt.Action;
        if (!_actions.ContainsKey(action))
        {
            _logger.LogError("There is no action text for \"{0}\" type of event", action);

            return string.Empty;
        }

        try
        {
            var actionText = _actions[(MessageAction)evt.Action].GetActionText();

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

    public string GetActionText(LoginEventDto evt)
    {
        var action = (MessageAction)evt.Action;
        if (!_actions.ContainsKey(action))
        {
            //log.Error(string.Format("There is no action text for \"{0}\" type of event", action));
            return string.Empty;
        }

        try
        {
            var actionText = _actions[(MessageAction)evt.Action].GetActionText();

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

    public string GetActionTypeText(AuditEventDto evt)
    {
        var action = (MessageAction)evt.Action;

        return !_actions.ContainsKey(action)
                   ? string.Empty
                   : _actions[(MessageAction)evt.Action].GetActionTypeText();
    }

    public string GetProductText(AuditEventDto evt)
    {
        var action = (MessageAction)evt.Action;

        return !_actions.ContainsKey(action)
                   ? string.Empty
                   : _actions[(MessageAction)evt.Action].GetProduct();
    }

    public string GetModuleText(AuditEventDto evt)
    {
        var action = (MessageAction)evt.Action;

        return !_actions.ContainsKey(action)
                   ? string.Empty
                   : _actions[(MessageAction)evt.Action].GetModule();
    }

    private string ToLimitedText(string text)
    {
        if (text == null)
        {
            return null;
        }

        return text.Length < 50 ? text : $"{text.Substring(0, 47)}...";
    }
}