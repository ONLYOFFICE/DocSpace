/*
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
    [Scope]
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
                using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
                {
                    csv.Configuration.RegisterClassMap(new BaseEventMap<TEvent>());

                    csv.WriteHeader<TEvent>();
                    csv.NextRecord();
                    csv.WriteRecords(events);
                    writer.Flush();

                    var file = FileUploader.ExecAsync(GlobalFolderHelper.FolderMy, reportName, stream.Length, stream, true).Result;
                    var fileUrl = CommonLinkUtility.GetFullAbsolutePath(FilesLinkUtility.GetFileWebEditorUrl(file.ID));

                    fileUrl += string.Format("&options={{\"codePage\":{0}}}", Encoding.UTF8.CodePage);
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
}