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

namespace ASC.TelegramService.Core;

public class TelegramCommand
{
    public string CommandName { get; private set; }
    public string[] Args { get; private set; }
    public Message Message { get; private set; }
    public User User { get { return Message.From; } }
    public Chat Chat { get { return Message.Chat; } }

    public TelegramCommand(Message msg, string cmdName, string[] args = null)
    {
        Message = msg;
        CommandName = cmdName;
        Args = args;
    }
}

[Singletone]
public class CommandModule
{
    private readonly Regex _cmdReg = new Regex(@"^\/([^\s]+)\s?(.*)");
    private readonly Regex _argsReg = new Regex(@"[^""\s]\S*|"".+?""");

    private readonly Dictionary<string, MethodInfo> _commands = new Dictionary<string, MethodInfo>();
    private readonly Dictionary<string, Type> _contexts = new Dictionary<string, Type>();
    private readonly Dictionary<Type, ParamParser> _parsers = new Dictionary<Type, ParamParser>();

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CommandModule> _log;

    public CommandModule(ILogger<CommandModule> logger, IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _log = logger;

        var assembly = Assembly.GetExecutingAssembly();

        foreach (var t in assembly.GetExportedTypes())
        {
            if (t.IsAbstract)
            {
                continue;
            }

            if (t.IsSubclassOf(typeof(CommandContext)))
            {
                foreach (var method in t.GetRuntimeMethods())
                {
                    if (method.IsPublic && Attribute.IsDefined(method, typeof(CommandAttribute)))
                    {
                        var attr = method.GetCustomAttribute<CommandAttribute>();
                        _commands.Add(attr.Name, method);
                        _contexts.Add(attr.Name, t);
                    }
                }
            }

            if (t.IsSubclassOf(typeof(ParamParser)) && Attribute.IsDefined(t, typeof(ParamParserAttribute)))
            {
                _parsers.Add(t.GetCustomAttribute<ParamParserAttribute>().Type, (ParamParser)Activator.CreateInstance(t));
            }
        }
    }

    private TelegramCommand ParseCommand(Message msg)
    {
        var reg = _cmdReg.Match(msg.Text);
        var args = _argsReg.Matches(reg.Groups[2].Value);

        return new TelegramCommand(msg, reg.Groups[1].Value.ToLowerInvariant(), args.Count > 0 ? args.Select(a => a.Value).ToArray() : null);
    }

    private object[] ParseParams(MethodInfo cmd, string[] args)
    {
        var parsedParams = new List<object>();

        var cmdArgs = cmd.GetParameters();

        if (cmdArgs.Length > 0 && args == null || cmdArgs.Length != args.Length)
        {
            throw new Exception("Wrong parameters count");
        }

        for (var i = 0; i < cmdArgs.Length; i++)
        {
            var type = cmdArgs[i].ParameterType;
            var arg = args[i];
            if (type == typeof(string))
            {
                parsedParams.Add(arg);
                continue;
            }

            if (!_parsers.ContainsKey(type))
            {
                throw new Exception(string.Format("No parser found for type '{0}'", type));
            }

            parsedParams.Add(_parsers[type].FromString(arg));
        }

        return parsedParams.ToArray();
    }

    public async Task HandleCommand(Message msg, ITelegramBotClient client, int tenantId)
    {
        try
        {
            var cmd = ParseCommand(msg);

            if (!_commands.ContainsKey(cmd.CommandName))
            {
                throw new Exception($"No handler found for command '{cmd.CommandName}'");
            }

            var command = _commands[cmd.CommandName];
            var context = (CommandContext)_scopeFactory.CreateScope().ServiceProvider.GetService(_contexts[cmd.CommandName]);
            var param = ParseParams(command, cmd.Args);

            context.Context = cmd;
            context.Client = client;
            context.TenantId = tenantId;
            await Task.FromResult(command.Invoke(context, param));
        }
        catch (Exception ex)
        {
            _log.DebugCouldntHandle(msg.Text, ex);
        }
    }
}

public abstract class CommandContext
{
    public ITelegramBotClient Client { get; set; }
    public TelegramCommand Context { get; set; }
    public int TenantId { get; set; }

    protected async Task ReplyAsync(string message)
    {
        await Client.SendTextMessageAsync(Context.Chat, message);
    }
}

public abstract class ParamParser
{
    protected Type _type;

    protected ParamParser(Type type)
    {
        _type = type;
    }

    public abstract object FromString(string arg);
    public abstract string ToString(object arg);
}

public abstract class ParamParser<T> : ParamParser
{
    protected ParamParser() : base(typeof(T)) { }

    public abstract override object FromString(string arg);
    public abstract override string ToString(object arg);
}