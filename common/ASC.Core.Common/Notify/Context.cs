/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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

namespace ASC.Notify;

public sealed class Context : INotifyRegistry
{
    public const string SysRecipient = "_#" + SysRecipientId + "#_";
    internal const string SysRecipientId = "SYS_RECIPIENT_ID";
    internal const string SysRecipientName = "SYS_RECIPIENT_NAME";
    internal const string SysRecipientAddress = "SYS_RECIPIENT_ADDRESS";

    private readonly Dictionary<string, ISenderChannel> _channels = new Dictionary<string, ISenderChannel>(2);

    public NotifyEngine NotifyEngine { get; private set; }
    public INotifyRegistry NotifyService => this;
    public DispatchEngine DispatchEngine { get; private set; }

    public event Action<Context, INotifyClient> NotifyClientRegistration;

    private ILog Logger { get; set; }

    public Context(IServiceProvider serviceProvider)
    {
        var options = serviceProvider.GetService<IOptionsMonitor<ILog>>();
        Logger = options.CurrentValue;
        NotifyEngine = new NotifyEngine(this, serviceProvider);
        DispatchEngine = new DispatchEngine(this, serviceProvider.GetService<IConfiguration>(), options);
    }

    void INotifyRegistry.RegisterSender(string senderName, ISink senderSink)
    {
        lock (_channels)
        {
            _channels[senderName] = new SenderChannel(this, senderName, null, senderSink);
        }
    }

    void INotifyRegistry.UnregisterSender(string senderName)
    {
        lock (_channels)
        {
            _channels.Remove(senderName);
        }
    }

    ISenderChannel INotifyRegistry.GetSender(string senderName)
    {
        lock (_channels)
        {
            _channels.TryGetValue(senderName, out var channel);

            return channel;
        }
    }

    INotifyClient INotifyRegistry.RegisterClient(INotifySource source, IServiceScope serviceScope)
    {
        //ValidateNotifySource(source);
        var client = new NotifyClientImpl(this, source, serviceScope);
        NotifyClientRegistration?.Invoke(this, client);

        return client;
    }

    private void ValidateNotifySource(INotifySource source)
    {
        foreach (var a in source.GetActionProvider().GetActions())
        {
            IEnumerable<string> senderNames;
            lock (_channels)
            {
                senderNames = _channels.Values.Select(s => s.SenderName);
            }
            foreach (var s in senderNames)
            {
                try
                {
                    var pattern = source.GetPatternProvider().GetPattern(a, s);
                    if (pattern == null)
                    {
                        throw new NotifyException($"In notify source {source.Id} pattern not found for action {a.ID} and sender {s}");
                    }
                }
                catch (Exception error)
                {
                    Logger.ErrorFormat("Source: {0}, action: {1}, sender: {2}, error: {3}", source.Id, a.ID, s, error);
                }
            }
        }
    }
}
