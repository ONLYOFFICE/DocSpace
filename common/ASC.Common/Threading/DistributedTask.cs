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

namespace ASC.Common.Threading
{
    public class DistributedTask
    {
        public Action<DistributedTask> Publication { get; set; }

        protected internal DistributedTaskCache DistributedTaskCache { get; internal set; }

        public int InstanceId
        {
            get
            {
                return DistributedTaskCache.InstanceId;
            }
            set
            {
                DistributedTaskCache.InstanceId = value;
            }
        }
        public string Id
        {
            get
            {
                return DistributedTaskCache.Id;
            }
            protected set
            {
                DistributedTaskCache.Id = value?.ToString() ?? "";
            }
        }

        public DistributedTaskStatus Status
        {
            get
            {
                return Enum.Parse<DistributedTaskStatus>(DistributedTaskCache.Status);
            }
            set
            {
                DistributedTaskCache.Status = value.ToString();
            }
        }

        public Exception Exception
        {
            get
            {
                return new Exception(DistributedTaskCache.Exception);
            }
            set
            {
                DistributedTaskCache.Exception = value?.ToString() ?? "";
            }
        }

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

            if (prop == null) return default;

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
}
