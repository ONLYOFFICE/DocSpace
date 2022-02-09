﻿/*
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

namespace ASC.TelegramService.Core
{
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
        private ILog Log { get; }

        private readonly Regex cmdReg = new Regex(@"^\/([^\s]+)\s?(.*)");
        private readonly Regex argsReg = new Regex(@"[^""\s]\S*|"".+?""");

        private readonly Dictionary<string, MethodInfo> commands = new Dictionary<string, MethodInfo>();
        private readonly Dictionary<string, Type> contexts = new Dictionary<string, Type>();
        private readonly Dictionary<Type, ParamParser> parsers = new Dictionary<Type, ParamParser>();

        private IServiceProvider ServiceProvider { get; }
        public CommandModule(IOptionsMonitor<ILog> options, IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Log = options.CurrentValue;

            var assembly = Assembly.GetExecutingAssembly();

            foreach (var t in assembly.GetExportedTypes())
            {
                if (t.IsAbstract) continue;

                if (t.IsSubclassOf(typeof(CommandContext)))
                {
                    foreach (var method in t.GetRuntimeMethods())
                    {
                        if (method.IsPublic && Attribute.IsDefined(method, typeof(CommandAttribute)))
                        {
                            var attr = method.GetCustomAttribute<CommandAttribute>();
                            commands.Add(attr.Name, method);
                            contexts.Add(attr.Name, t);
                        }
                    }
                }

                if (t.IsSubclassOf(typeof(ParamParser)) && Attribute.IsDefined(t, typeof(ParamParserAttribute)))
                {
                    parsers.Add(t.GetCustomAttribute<ParamParserAttribute>().Type, (ParamParser)Activator.CreateInstance(t));
                }
            }
        }

        private TelegramCommand ParseCommand(Message msg)
        {
            var reg = cmdReg.Match(msg.Text);
            var args = argsReg.Matches(reg.Groups[2].Value);

            return new TelegramCommand(msg, reg.Groups[1].Value.ToLowerInvariant(), args.Count > 0 ? args.Cast<Match>().Select(a => a.Value).ToArray() : null);
        }

        private object[] ParseParams(MethodInfo cmd, string[] args)
        {
            var parsedParams = new List<object>();

            var cmdArgs = cmd.GetParameters();

            if (cmdArgs.Any() && args == null || cmdArgs.Count() != args.Count()) throw new Exception("Wrong parameters count");
            for (var i = 0; i < cmdArgs.Count(); i++)
            {
                var type = cmdArgs[i].ParameterType;

                if (type == typeof(string))
                {
                    parsedParams.Add(args[i]);
                    continue;
                }

                if (!parsers.ContainsKey(type)) throw new Exception(string.Format("No parser found for type '{0}'", type));

                parsedParams.Add(parsers[cmdArgs[i].ParameterType].FromString(args[i]));
            }

            return parsedParams.ToArray();
        }

        public async Task HandleCommand(Message msg, TelegramBotClient client, int tenantId)
        {
            try
            {
                var cmd = ParseCommand(msg);

                if (!commands.ContainsKey(cmd.CommandName)) throw new Exception(string.Format("No handler found for command '{0}'", cmd.CommandName));

                var command = commands[cmd.CommandName];
                var context = (CommandContext)ServiceProvider.CreateScope().ServiceProvider.GetService(contexts[cmd.CommandName]);
                var param = ParseParams(command, cmd.Args);

                context.Context = cmd;
                context.Client = client;
                context.TenantId = tenantId;
                await Task.FromResult(command.Invoke(context, param));
            }
            catch (Exception ex)
            {
                Log.DebugFormat("Couldn't handle ({0}) message ({1})", msg.Text, ex.Message);
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
        protected Type type;

        protected ParamParser(Type type)
        {
            this.type = type;
        }

        public abstract object FromString(string arg);
        public abstract string ToString(object arg);
    }

    public abstract class ParamParser<T> : ParamParser
    {
        public ParamParser() : base(typeof(T)) { }

        public override abstract object FromString(string arg);
        public override abstract string ToString(object arg);
    }
}
