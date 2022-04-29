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

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Utils;

namespace ASC.Files.Core.Services.OFormService;

[Singletone]
public class OFormRequestManager
{
    private readonly OFormSettings _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly TempPath _tempPath;
    private readonly SemaphoreSlim _semaphoreSlim;
    private OFromRequestData _data;

    public OFormRequestManager(
        ConfigurationExtension configuration,
        IHttpClientFactory httpClientFactory,
        TempPath tempPath)
    {
        _semaphoreSlim = new SemaphoreSlim(1);
        _configuration = configuration.GetSetting<OFormSettings>("files:oform");
        _httpClientFactory = httpClientFactory;
        _tempPath = tempPath;
    }

    public async Task Init(CancellationToken cancellationToken)
    {
        await _semaphoreSlim.WaitAsync();

        try
        {
            using var httpClient = _httpClientFactory.CreateClient();
            using var response = await httpClient.GetAsync(_configuration.Url);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            using var combined = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);
            _data = JsonSerializer.Deserialize<OFromRequestData>(await response.Content.ReadAsStringAsync(combined.Token), options);
        }
        catch (Exception)
        {

        }
        finally
        {
            _semaphoreSlim.Release();
        }

    }

    public async Task<FileStream> Get(int id)
    {
        await _semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(_configuration.Period));

        try
        {
            if (_data == null) throw new Exception("not found");

            var item = _data.Data.FirstOrDefault(r => r.Id == id);
            if (item == null) throw new Exception("not found");

            var file = item.Attributes.File.Data.FirstOrDefault(f => f.Attributes.Ext == _configuration.Ext);
            if (file == null) throw new Exception("not found");

            var filePath = Path.Combine(_tempPath.GetTempPath(), file.Attributes.Name);

            if (!File.Exists(filePath))
            {
                await DownloadAndSave(file, filePath);
            }

            return File.OpenRead(filePath);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private async Task DownloadAndSave(OFromFileData fileData, string filePath)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        using var response = await httpClient.GetAsync(fileData.Attributes.Url);
        using var stream = await response.Content.ReadAsStreamAsync();
        using var fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read);
        await stream.CopyToAsync(fileStream);
    }
}
