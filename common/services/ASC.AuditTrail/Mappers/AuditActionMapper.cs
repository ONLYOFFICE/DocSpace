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