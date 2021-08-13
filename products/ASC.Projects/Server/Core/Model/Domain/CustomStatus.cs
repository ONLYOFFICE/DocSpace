/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
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