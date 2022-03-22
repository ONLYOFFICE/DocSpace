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

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ASC.Common.Threading;

public class DistributedTask
{
    public Action<DistributedTask> Publication { get; set; }
    public int InstanceId
    {
        get => DistributedTaskCache.InstanceId;
        set => DistributedTaskCache.InstanceId = value;
    }
    public string Id
    {
        get => DistributedTaskCache.Id;
        protected set => DistributedTaskCache.Id = value ?? "";
    }
    public DistributedTaskStatus Status
    {
        get => Enum.Parse<DistributedTaskStatus>(DistributedTaskCache.Status);
        set => DistributedTaskCache.Status = value.ToString();
    }
    public Exception Exception
    {
        get => new Exception(DistributedTaskCache.Exception);
        set => DistributedTaskCache.Exception = value?.ToString() ?? "";
    }
    protected internal DistributedTaskCache DistributedTaskCache { get; internal set; }

    public DistributedTask()
    {
        DistributedTaskCache = new DistributedTaskCache
        {
            Id = Guid.NewGuid().ToString()
        };
    }

    public DistributedTask(DistributedTaskCache distributedTaskCache)
    {
        DistributedTaskCache = distributedTaskCache;
    }

    public T GetProperty<T>(string name)
    {
        var prop = DistributedTaskCache.Props.FirstOrDefault(r => r.Key == name);
        if (prop == null)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(prop.Value);
    }

    public void SetProperty(string name, object value)
    {
        var prop = new DistributedTaskCache.Types.DistributedTaskCacheProp()
        {
            Key = name,
            Value = JsonSerializer.Serialize(value)
        };

        var current = DistributedTaskCache.Props.SingleOrDefault(r => r.Key == name);
        if (current != null)
        {
            DistributedTaskCache.Props.Remove(current);
        }

        if (value != null)
        {
            DistributedTaskCache.Props.Add(prop);
        }
    }

    public void PublishChanges()
    {
        if (Publication == null)
        {
            throw new InvalidOperationException("Publication not found.");
        }

        Publication(this);
    }
}