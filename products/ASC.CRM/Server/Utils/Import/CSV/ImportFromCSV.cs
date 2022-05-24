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


using System;
using System.IO;
using System.Linq;
using System.Text.Json;

using ASC.Common;
using ASC.Common.Threading;
using ASC.Common.Threading.Progress;
using ASC.Core;
using ASC.CRM.Core.Enums;

using LumenWorks.Framework.IO.Csv;

namespace ASC.Web.CRM.Classes
{
    [Scope]
    public class ImportFromCSV
    {
        private readonly ImportDataOperation _importDataOperation;
        private readonly int _tenantId;
        private readonly object _syncObj = new object();
        private readonly DistributedTaskQueue _importQueue;
        public readonly int MaxRoxCount = 10000;

        public ImportFromCSV(TenantManager tenantProvider,
                             IDistributedTaskQueueFactory factory,
                             ImportDataOperation importDataOperation)
        {
            _tenantId = tenantProvider.GetCurrentTenant().Id;
            _importQueue = factory.CreateQueue<ImportDataOperation>();
            _importDataOperation = importDataOperation;
        }


        public int GetQuotas()
        {
            return MaxRoxCount;
        }

        public CsvReader CreateCsvReaderInstance(Stream CSVFileStream, ImportCSVSettings importCsvSettings)
        {
            var result = new CsvReader(
                new StreamReader(CSVFileStream, importCsvSettings.Encoding, true),
                importCsvSettings.HasHeader, importCsvSettings.DelimiterCharacter, importCsvSettings.QuoteType, '"', '#', ValueTrimmingOptions.UnquotedOnly)
            { SkipEmptyLines = true, SupportsMultiline = true, DefaultParseErrorAction = ParseErrorAction.AdvanceToNextLine, MissingFieldAction = MissingFieldAction.ReplaceByEmpty };

            return result;
        }

        public String GetRow(Stream CSVFileStream, int index, String jsonSettings)
        {
            var importCSVSettings = new ImportCSVSettings(jsonSettings);

            CsvReader csv = CreateCsvReaderInstance(CSVFileStream, importCSVSettings);

            int countRows = 0;

            index++;

            while (countRows++ != index && csv.ReadNextRecord()) ;

            return JsonSerializer.Serialize(new
            {
                data = csv.GetCurrentRowFields(false).ToArray(),
                isMaxIndex = csv.EndOfStream
            });
        }

        public JsonDocument GetInfo(Stream CSVFileStream, String jsonSettings)
        {
            var importCSVSettings = new ImportCSVSettings(jsonSettings);

            CsvReader csv = CreateCsvReaderInstance(CSVFileStream, importCSVSettings);

            csv.ReadNextRecord();

            var firstRowFields = csv.GetCurrentRowFields(false);

            String[] headerRowFields = csv.GetFieldHeaders().ToArray();

            if (!importCSVSettings.HasHeader)
                headerRowFields = firstRowFields;

            return JsonDocument.Parse(JsonSerializer.Serialize(new
            {
                headerColumns = headerRowFields,
                firstContactFields = firstRowFields,
                isMaxIndex = csv.EndOfStream
            }));
        }

        protected String GetKey(EntityType entityType)
        {
            return String.Format("{0}_{1}", _tenantId, (int)entityType);
        }

        public IProgressItem GetStatus(EntityType entityType)
        {
            var operation = _importQueue.GetAllTasks<ImportDataOperation>().FirstOrDefault(x => x.Id == GetKey(entityType));

            return operation;
        }

        public IProgressItem Start(EntityType entityType, String CSVFileURI, String importSettingsJSON)
        {
            lock (_syncObj)
            {
                var operation = _importQueue.GetAllTasks<ImportDataOperation>().FirstOrDefault(x => x.Id == GetKey(entityType));

                if (operation != null && operation.IsCompleted)
                {
                    _importQueue.DequeueTask(operation.Id);

                    operation = null;

                }

                if (operation == null)
                {
                    _importDataOperation.Configure(entityType, CSVFileURI, importSettingsJSON);
                    _importQueue.EnqueueTask(_importDataOperation);
                }

                return operation;
            }
        }
    }
}