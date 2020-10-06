/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using System.Globalization;
using System.IO;
using System.Text;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Web.Core.Files;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Utility;

using CsvHelper;

using Microsoft.Extensions.Options;

namespace ASC.AuditTrail
{
    public class AuditReportCreator
    {
        private ILog Log { get; }
        private GlobalFolderHelper GlobalFolderHelper { get; }
        private FileUploader FileUploader { get; }
        private FilesLinkUtility FilesLinkUtility { get; }
        private CommonLinkUtility CommonLinkUtility { get; }

        public AuditReportCreator(GlobalFolderHelper globalFolderHelper, IOptionsMonitor<ILog> options, FileUploader fileUploader, FilesLinkUtility filesLinkUtility, CommonLinkUtility commonLinkUtility)
        {
            GlobalFolderHelper = globalFolderHelper;
            Log = options.CurrentValue;
            FileUploader = fileUploader;
            FilesLinkUtility = filesLinkUtility;
            CommonLinkUtility = commonLinkUtility;
        }

        public string CreateCsvReport<TEvent>(IEnumerable<TEvent> events, string reportName) where TEvent : BaseEvent
        {
            try
            {
                using (var stream = new MemoryStream())
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.RegisterClassMap(new BaseEventMap<TEvent>());

                    csv.WriteHeader<TEvent>();
                    csv.NextRecord();
                    csv.WriteRecords(events);
                    writer.Flush();

                    var file = FileUploader.Exec(GlobalFolderHelper.FolderMy.ToString(), reportName, stream.Length, stream, true);
                    var fileUrl = CommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.GetFileWebEditorUrl(file.ID));

                    fileUrl += string.Format("&options={{\"delimiter\":{0},\"codePage\":{1}}}",
                                             (int)FileUtility.CsvDelimiter.Comma,
                                             Encoding.UTF8.CodePage);
                    return fileUrl;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while generating login report: " + ex);
                throw;
            }
        }
    }

    public static class AuditReportCreatorExtension
    {
        public static DIHelper AddAuditReportCreatorService(this DIHelper services)
        {
            _ = services.TryAddScoped<AuditReportCreator>();
            return services
                .AddGlobalFolderHelperService()
                .AddFileUploaderService()
                .AddFilesLinkUtilityService()
                .AddCommonLinkUtilityService();
        }
    }
}