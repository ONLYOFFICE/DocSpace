
using System;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Utils;

using Microsoft.Extensions.Hosting;

namespace ASC.Files.Core.Services.OFormService
{
    [Singletone]
    public sealed class OFormService : BackgroundService
    {
        private readonly TimeSpan _formPeriod;
        private readonly OFormRequestManager _oFormRequestManager;

        public OFormService(OFormRequestManager oFormRequestManager, ConfigurationExtension configurationExtension)
        {
            _oFormRequestManager = oFormRequestManager;
            _formPeriod = TimeSpan.FromSeconds(configurationExtension.GetSetting<OFormSettings>("files:oform").Period);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _oFormRequestManager.Init(stoppingToken);
                await Task.Delay(_formPeriod, stoppingToken);
            }
        }
    }
}
