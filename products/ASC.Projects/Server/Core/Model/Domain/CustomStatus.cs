/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ASC.Common;
using ASC.Common.Utils;
using ASC.Projects.Core.Domain;
using ASC.Projects.Resources;

using Microsoft.AspNetCore.Hosting;

namespace ASC.Projects
{
    public abstract class CustomStatus
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string ImageType { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public int Order { get; set; }
        public bool IsDefault { get; set; }
        public bool? Available { get; set; }
    }

    [Scope]
    public class CustomStatusHelper
    {
        private IWebHostEnvironment HostEnvironment { get; set; }

        public CustomStatusHelper(IWebHostEnvironment hostEnvironment)
        {
            HostEnvironment = hostEnvironment;
        }

        public List<CustomTaskStatus> GetDefaults()
        {
            return new List<CustomTaskStatus>
            {
                GetDefault(TaskStatus.Open),
                GetDefault(TaskStatus.Closed)
            };
        }

        public CustomTaskStatus GetDefault(TaskStatus status)
        {
            switch (status)
            {
                case TaskStatus.Open:
                    return GetDefault(status, TaskResource.Open, "inbox.svg");
                case TaskStatus.Closed:
                    return GetDefault(status, TaskResource.Closed, "check_tick.svg");
            }
            return null;
        }

        private CustomTaskStatus GetDefault(TaskStatus status, string title, string image, bool available = true)
        {
            return new CustomTaskStatus
            {
                StatusType = status,
                Id = -(int)status,
                Title = title,
                Image = GetImageBase64Content(image),
                ImageType = "image/svg+xml",
                Color = "#83888d",
                IsDefault = true,
                Available = available
            };
        }
        protected string GetImageBase64Content(string path)//todo
        {
            return path;
           /* path = "/skins/default/images/svg/projects/" + path;
            var serverPath = CrossPlatform.PathCombine(HostEnvironment.ContentRootPath, path);
            if (string.IsNullOrEmpty(serverPath)) return "";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(File.ReadAllText(serverPath)));*/
        }

    }

    public class CustomTaskStatus : CustomStatus
    {
        public TaskStatus StatusType { get; set; }

        public bool CanChangeAvailable { get { return StatusType != TaskStatus.Open; } set { } }
    }
}