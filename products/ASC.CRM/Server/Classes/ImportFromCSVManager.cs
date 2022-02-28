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


#region Import

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using ASC.Common;
using ASC.CRM.Core.Enums;
using ASC.MessagingSystem;
using ASC.Web.Core.Utility;

#endregion

namespace ASC.Web.CRM.Classes
{
    [Scope]
    public class ImportFromCSVManager
    {
        private MessageService _messageService;
        private ImportFromCSV _importFromCSV;
        private Global _global;

        public ImportFromCSVManager(Global global,
                                    ImportFromCSV importFromCSV,
                                    MessageService messageService)
        {
            _global = global;
            _importFromCSV = importFromCSV;
            _messageService = messageService;
        }

        public void StartImport(EntityType entityType, String CSVFileURI, String importSettingsJSON)
        {
            _importFromCSV.Start(entityType, CSVFileURI, importSettingsJSON);

            _messageService.Send(GetMessageAction(entityType));
        }

        public Task<FileUploadResult> ProcessUploadFakeAsync(string fileTemp, string importSettingsJSON)
        {
            var fileUploadResult = new FileUploadResult();

            if (String.IsNullOrEmpty(fileTemp) || String.IsNullOrEmpty(importSettingsJSON)) return Task.FromResult(fileUploadResult);

            return InternalProcessUploadFakeAsync(fileTemp, importSettingsJSON);
        }

        private async Task<FileUploadResult> InternalProcessUploadFakeAsync(string fileTemp, string importSettingsJSON)
        {
            var fileUploadResult = new FileUploadResult();

            if (!await _global.GetStore().IsFileAsync("temp", fileTemp)) return fileUploadResult;

            JsonDocument jObject;

            //Read contents
            using (Stream storeStream = await _global.GetStore().GetReadStreamAsync("temp", fileTemp))
            {
                using (var CSVFileStream = new MemoryStream())
                {
                    //Copy
                    var buffer = new byte[4096];
                    int readed;
                    while ((readed = storeStream.Read(buffer, 0, 4096)) != 0)
                    {
                        CSVFileStream.Write(buffer, 0, readed);
                    }
                    CSVFileStream.Position = 0;

                    jObject = _importFromCSV.GetInfo(CSVFileStream, importSettingsJSON);
                }
            }

            var jsonDocumentAsDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(jObject.ToString());

            jsonDocumentAsDictionary.Add("assignedPath", fileTemp);

            fileUploadResult.Success = true;
            fileUploadResult.Data = Global.EncodeTo64(JsonSerializer.Serialize(jsonDocumentAsDictionary));

            return fileUploadResult;
        }

        private static MessageAction GetMessageAction(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Contact:
                    return MessageAction.ContactsImportedFromCSV;
                case EntityType.Task:
                    return MessageAction.CrmTasksImportedFromCSV;
                case EntityType.Opportunity:
                    return MessageAction.OpportunitiesImportedFromCSV;
                case EntityType.Case:
                    return MessageAction.CasesImportedFromCSV;
                default:
                    throw new ArgumentException("entityType");
            }
        }
    }
}