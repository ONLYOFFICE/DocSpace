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


using System.Text.Json;
using System.Threading.Tasks;

using ASC.Api.Core;
using ASC.Api.Documents;
using ASC.Common;
using ASC.Core;
using ASC.Core.Notify.Signalr;
using ASC.Files.Core;

using Microsoft.Extensions.Options;

namespace ASC.Web.Files.Utils
{
    [Scope]
    public class SocketManager
    {
        private readonly SignalrServiceClient _signalrServiceClient;
        private FileWrapperHelper FilesWrapperHelper { get; }
        private TenantManager TenantManager { get; }
        public IDaoFactory DaoFactory { get; }

        public SocketManager(
            IOptionsSnapshot<SignalrServiceClient> optionsSnapshot,
            FileWrapperHelper filesWrapperHelper,
            TenantManager tenantManager,
            IDaoFactory daoFactory
            )
        {
            _signalrServiceClient = optionsSnapshot.Get("files");
            FilesWrapperHelper = filesWrapperHelper;
            TenantManager = tenantManager;
            DaoFactory = daoFactory;
        }

        public void StartEdit<T>(T fileId)
        {
            var room = GetFileRoom(fileId);
            _signalrServiceClient.StartEdit(fileId, room);
        }

        public async Task StopEditAsync<T>(T fileId)
        {
            var room = GetFileRoom(fileId);
            var file = await DaoFactory.GetFileDao<T>().GetFileStableAsync(fileId);

            var serializerSettings = new JsonSerializerOptions()
            {
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            serializerSettings.Converters.Add(new ApiDateTimeConverter());
            serializerSettings.Converters.Add(new FileEntryWrapperConverter());
            var data = JsonSerializer.Serialize(await FilesWrapperHelper.GetAsync(file), serializerSettings);

            _signalrServiceClient.StopEdit(fileId, room, data);
        }

        public async Task CreateFileAsync<T>(File<T> file)
        {
            var room = GetFolderRoom(file.FolderID);
            var serializerSettings = new JsonSerializerOptions()
            {
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            serializerSettings.Converters.Add(new ApiDateTimeConverter());
            serializerSettings.Converters.Add(new FileEntryWrapperConverter());
            var data = JsonSerializer.Serialize(await FilesWrapperHelper.GetAsync(file), serializerSettings);

            _signalrServiceClient.CreateFile(file.ID, room, data);
        }

        public void DeleteFile<T>(File<T> file)
        {
            var room = GetFolderRoom(file.FolderID);
            _signalrServiceClient.DeleteFile(file.ID, room);
        }

        private string GetFileRoom<T>(T fileId)
        {
            var tenantId = TenantManager.GetCurrentTenant().TenantId;

            return $"{tenantId}-FILE-{fileId}";
        }

        private string GetFolderRoom<T>(T folderId)
        {
            var tenantId = TenantManager.GetCurrentTenant().TenantId;

            return $"{tenantId}-DIR-{folderId}";
        }
    }
}