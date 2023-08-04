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
    public NotifyServiceCfgProcess Process { get; set; }
    public List<NotifyServiceCfgSender> Senders { get; set; }
    public List<NotifyServiceCfgScheduler> Schedulers { get; set; }

    public void Init(IServiceProvider serviceProvider)
    {
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

        if (Schedulers != null)
        {
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
