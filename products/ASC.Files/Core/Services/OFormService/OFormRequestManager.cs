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

namespace ASC.Files.Core.Services.OFormService
{
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
}
