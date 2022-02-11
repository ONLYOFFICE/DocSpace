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

namespace ASC.AuditTrail.Mappers;

[Singletone]
public class AuditActionMapper
{
    private readonly Dictionary<MessageAction, MessageMaps> _actions;
    private readonly ILog _logger;

    public AuditActionMapper(IOptionsMonitor<ILog> options)
    {
        _actions = new Dictionary<MessageAction, MessageMaps>();
        _logger = options.CurrentValue;

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
            _logger.Error(string.Format("There is no action text for \"{0}\" type of event", action));

            return string.Empty;
        }

        try
        {
            var actionText = _actions[(MessageAction)evt.Action].GetActionText();

                if (evt.Description == null || evt.Description.Count == 0) return actionText;

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

                if (evt.Description == null || evt.Description.Count == 0) return actionText;

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
        if (text == null) return null;
        return text.Length < 50 ? text : $"{text.Substring(0, 47)}...";
    }
}