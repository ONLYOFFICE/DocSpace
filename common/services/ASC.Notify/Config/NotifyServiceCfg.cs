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

namespace ASC.Notify.Config;

[Singletone]
public class ConfigureNotifyServiceCfg : IConfigureOptions<NotifyServiceCfg>
{
    public ConfigureNotifyServiceCfg(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private readonly IServiceProvider _serviceProvider;

    public void Configure(NotifyServiceCfg options)
    {
        options.Init(_serviceProvider);
    }
}

[Singletone(typeof(ConfigureNotifyServiceCfg))]
public class NotifyServiceCfg
{
    public string ConnectionStringName { get; set; }
    public int StoreMessagesDays { get; set; }
    public string ServerRoot { get; set; }
    public NotifyServiceCfgProcess Process { get; set; }
    public List<NotifyServiceCfgSender> Senders { get; set; }
    public List<NotifyServiceCfgScheduler> Schedulers { get; set; }

    public void Init(IServiceProvider serviceProvider)
    {
        ServerRoot = string.IsNullOrEmpty(ServerRoot) ? "http://*/" : ServerRoot;

        Process.Init();

        foreach (var s in Senders)
        {
            try
            {
                s.Init(serviceProvider);
            }
            catch (Exception)
            {

            }
        }
        foreach (var s in Schedulers)
        {
            try
            {
                s.Init();
            }
            catch (Exception)
            {

            }
        }
    }
}

public class NotifyServiceCfgProcess
{
    public int MaxThreads { get; set; }
    public int BufferSize { get; set; }
    public int MaxAttempts { get; set; }
    public string AttemptsInterval { get; set; }

    public void Init()
    {
        if (MaxThreads == 0)
        {
            MaxThreads = Environment.ProcessorCount;
        }
    }
}

public class NotifyServiceCfgSender
{
    public string Name { get; set; }
    public string Type { get; set; }
    public Dictionary<string, string> Properties { get; set; }
    public INotifySender NotifySender { get; set; }

    public void Init(IServiceProvider serviceProvider)
    {
        var sender = (INotifySender)serviceProvider.GetService(System.Type.GetType(Type, true));
        sender.Init(Properties);
        NotifySender = sender;
    }
}

public class NotifyServiceCfgScheduler
{
    public string Name { get; set; }
    public string Register { get; set; }
    public MethodInfo MethodInfo { get; set; }

    public void Init()
    {
        var typeName = Register.Substring(0, Register.IndexOf(','));
        var assemblyName = Register.Substring(Register.IndexOf(','));
        var type = Type.GetType(string.Concat(typeName.AsSpan(0, typeName.LastIndexOf('.')), assemblyName), true);
        MethodInfo = type.GetMethod(typeName.Substring(typeName.LastIndexOf('.') + 1), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
    }
}
