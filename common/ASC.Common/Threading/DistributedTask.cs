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

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ASC.Common.Threading;

[ProtoContract(IgnoreUnknownSubTypes = true)]
[ProtoInclude(100, typeof(DistributedTaskProgress))]
public class DistributedTask
{
    [ProtoMember(10)]
    protected string _exeption;

    [ProtoMember(11)]
    protected Dictionary<string, string> _props;

    public Action<DistributedTask> Publication { get; set; }

    [ProtoMember(1)]
    public int InstanceId { get; set; }

    [ProtoMember(2)]
    public string Id { get; set; }

    [ProtoMember(3)]
    public DistributedTaskStatus Status { get; set; }
 
    public Exception Exception
    {
        get => new Exception(_exeption);
        set => _exeption = value?.Message ?? "";
    }


    public DistributedTask()
    {
        Id = Guid.NewGuid().ToString();

        _exeption = String.Empty;
        _props = new Dictionary<string, string>();
    }
    
    public void PublishChanges()
    {
        if (Publication == null)
        {
            throw new InvalidOperationException("Publication not found.");
        }

        Publication(this);
    }

    public string this[string propName]
    {
        get
        {
            return _props[propName];
        }
        set
        {
            _props[propName] = value;
        }
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();  
    }
}