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

namespace ASC.Notify;

    [Singletone]
public sealed class Context : INotifyRegistry
{
    public const string SysRecipient = "_#" + _sysRecipientId + "#_";
    internal const string _sysRecipientId = "SYS_RECIPIENT_ID";
    internal const string _sysRecipientName = "SYS_RECIPIENT_NAME";
    internal const string _sysRecipientAddress = "SYS_RECIPIENT_ADDRESS";

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
