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

namespace ASC.Web.Files.Utils;

[Scope]
public class SocketManager
{
    private readonly SignalrServiceClient _signalrServiceClient;
    private readonly FileDtoHelper _filesWrapperHelper;
    private readonly TenantManager _tenantManager;

    public SocketManager(
        IOptionsSnapshot<SignalrServiceClient> optionsSnapshot,
        FileDtoHelper filesWrapperHelper,
        TenantManager tenantManager
        )
    {
        _signalrServiceClient = optionsSnapshot.Get("files");
        _filesWrapperHelper = filesWrapperHelper;
        _tenantManager = tenantManager;
    }

    public void StartEdit<T>(T fileId)
    {
        var room = GetFileRoom(fileId);
        _signalrServiceClient.StartEdit(fileId, room);
    }

    public void StopEdit<T>(T fileId)
    {
        var room = GetFileRoom(fileId);
        _signalrServiceClient.StopEdit(fileId, room);
    }

    public async Task CreateFileAsync<T>(File<T> file)
    {
        var room = GetFolderRoom(file.ParentId);
        var serializerSettings = new JsonSerializerOptions()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        serializerSettings.Converters.Add(new ApiDateTimeConverter());
        serializerSettings.Converters.Add(new FileEntryWrapperConverter());
        var data = JsonSerializer.Serialize(await _filesWrapperHelper.GetAsync(file), serializerSettings);

        _signalrServiceClient.CreateFile(file.Id, room, data);
    }

    public async Task UpdateFileAsync<T>(File<T> file)
    {
        var room = GetFolderRoom(file.ParentId);
        var serializerSettings = new JsonSerializerOptions()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        serializerSettings.Converters.Add(new ApiDateTimeConverter());
        serializerSettings.Converters.Add(new FileEntryWrapperConverter());
        var data = JsonSerializer.Serialize(await _filesWrapperHelper.GetAsync(file), serializerSettings);

        _signalrServiceClient.UpdateFile(file.Id, room, data);
    }

    public void DeleteFile<T>(File<T> file)
    {
        var room = GetFolderRoom(file.ParentId);
        _signalrServiceClient.DeleteFile(file.Id, room);
    }

    private string GetFileRoom<T>(T fileId)
    {
        var tenantId = _tenantManager.GetCurrentTenant().Id;

        return $"{tenantId}-FILE-{fileId}";
    }

    private string GetFolderRoom<T>(T folderId)
    {
        var tenantId = _tenantManager.GetCurrentTenant().Id;

        return $"{tenantId}-DIR-{folderId}";
    }
}
