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


using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using ASC.Common;
using ASC.Files.Core;
using ASC.Web.Files.Services.WCFService;

namespace ASC.Api.Documents
{
    /// <summary>
    /// </summary>
    [DataContract(Name = "content", Namespace = "")]
    public class FolderContentWrapper<T>
    {
        /// <summary>
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<FileWrapper<T>> Files { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<FolderWrapper<T>> Folders { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public FolderWrapper<T> Current { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public object PathParts { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public int StartIndex { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public int Count { get; set; }

        /// <summary>
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public int Total { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="folderItems"></param>
        /// <param name="startIndex"></param>
        public FolderContentWrapper()
        {

        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static FolderContentWrapper<int> GetSample()
        {
            return new FolderContentWrapper<int>
            {
                Current = FolderWrapper<int>.GetSample(),
                Files = new List<FileWrapper<int>>(new[] { FileWrapper<int>.GetSample(), FileWrapper<int>.GetSample() }),
                Folders = new List<FolderWrapper<int>>(new[] { FolderWrapper<int>.GetSample(), FolderWrapper<int>.GetSample() }),
                PathParts = new
                {
                    key = "Key",
                    path = "//path//to//folder"
                },

                StartIndex = 0,
                Count = 4,
                Total = 4,
            };
        }
    }

    public class FolderContentWrapperHelper
    {
        private FileWrapperHelper FileWrapperHelper { get; }
        private FolderWrapperHelper FolderWrapperHelper { get; }

        public FolderContentWrapperHelper(
            FileWrapperHelper fileWrapperHelper,
            FolderWrapperHelper folderWrapperHelper)
        {
            FileWrapperHelper = fileWrapperHelper;
            FolderWrapperHelper = folderWrapperHelper;
        }

        public FolderContentWrapper<T> Get<T>(DataWrapper<T> folderItems, int startIndex)
        {
            var result = new FolderContentWrapper<T>
            {
                Files = folderItems.Entries.OfType<File<T>>().Select(FileWrapperHelper.Get).ToList(),
                Folders = folderItems.Entries.OfType<Folder<T>>().Select(FolderWrapperHelper.Get).ToList(),
                PathParts = folderItems.FolderPathParts,
                StartIndex = startIndex
            };

            result.Current = FolderWrapperHelper.Get(folderItems.FolderInfo);
            result.Count = result.Files.Count + result.Folders.Count;
            result.Total = folderItems.Total;

            return result;
        }
    }
    public static class FolderContentWrapperHelperExtention
    {
        public static DIHelper AddFolderContentWrapperHelperService(this DIHelper services)
        {
            services.TryAddScoped<FolderContentWrapperHelper>();
            return services
                .AddFileWrapperHelperService()
                .AddFolderWrapperHelperService();
        }
    }
}