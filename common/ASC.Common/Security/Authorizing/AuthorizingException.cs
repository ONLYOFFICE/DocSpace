namespace ASC.Common.Security.Authorizing;

[Serializable]
public class AuthorizingException : Exception
{
    public ISubject Subject { get; internal set; }
    public IAction[] Actions { get; internal set; }
    public override string Message => _message;

    private readonly string _message;

    public AuthorizingException(string message)
        : base(message) { }

    public AuthorizingException(ISubject subject, IAction[] actions)
    {
        if (actions == null || actions.Length == 0)
        {
            throw new ArgumentNullException(nameof(actions));
        }

        ArgumentNullException.ThrowIfNull(subject);

        Subject = subject;
        Actions = actions;
        var sactions = "";

        Array.ForEach(actions, action => { sactions += action.ToString() + ", "; });

        _message = string.Format(
            "\"{0}\" access denied \"{1}\"",
            subject,
            sactions
            );
    }

    public AuthorizingException(ISubject subject, IAction[] actions, ISubject[] denySubjects, IAction[] denyActions) =>
        _message = FormatErrorMessage(subject, actions, denySubjects, denyActions);

    protected AuthorizingException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        _message = info.GetValue("_Message", typeof(string)) as string;
        Subject = info.GetValue("Subject", typeof(ISubject)) as ISubject;
        Actions = info.GetValue("Actions", typeof(IAction[])) as IAction[];
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Subject", Subject, typeof(ISubject));
        info.AddValue("_Message", _message, typeof(string));
        info.AddValue("Actions", Actions, typeof(IAction[]));
        base.GetObjectData(info, context);
    }

    internal static string FormatErrorMessage(ISubject subject, IAction[] actions, ISubject[] denySubjects,
                                              IAction[] denyActions)
    {
        ArgumentNullException.ThrowIfNull(subject);

        if (actions == null || actions.Length == 0)
        {
            throw new ArgumentNullException(nameof(actions));
        }
        if (denySubjects == null || denySubjects.Length == 0)
        {
            throw new ArgumentNullException(nameof(denySubjects));
        }
        if (denyActions == null || denyActions.Length == 0)
        {
            throw new ArgumentNullException(nameof(denyActions));
        }
        if (actions.Length != denySubjects.Length || actions.Length != denyActions.Length)
        {
            throw new ArgumentException();
        }

        var sb = new StringBuilder();
        for (var i = 0; i < actions.Length; i++)
        {
            var action = actions[i];
            var denyAction = denyActions[i];
            var denySubject = denySubjects[i];

            string reason;
            if (denySubject != null && denyAction != null)
            {
                reason = $"{action.Name}:{(denySubject is IRole ? "role:" : "") + denySubject.Name} access denied {denyAction.Name}.";
            }
            else
            {
                reason = $"{action.Name}: access denied.";
            }
            if (i != actions.Length - 1)
            {
                reason += ", ";
            }

            sb.Append(reason);
        }
        var reasons = sb.ToString();
        var sactions = "";
        Array.ForEach(actions, action => { sactions += action.ToString() + ", "; });

        var message = $"\"{(subject is IRole ? "role:" : "") + subject.Name}\" access denied \"{sactions}\". Cause: {reasons}.";
        return message;
    }
}
