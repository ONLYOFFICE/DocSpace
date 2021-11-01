/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
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


using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;

using Microsoft.Extensions.Hosting;

namespace ASC.Files.ThumbnailBuilder
{
    [Singletone(Additional = typeof(WorkerExtension))]
    public class Launcher : IHostedService
    {
        private Worker worker;

        internal static readonly ConcurrentDictionary<object, FileData<int>> Queue = new ConcurrentDictionary<object, FileData<int>>();
        private Service Service { get; }

        public Launcher(Service service, Worker worker)
        {
            Service = service;
            this.worker = worker;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            worker.Start(cancellationToken);
            Service.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (worker != null)
            {
                worker.Stop();
                worker = null;
            }

            if (Service != null)
            {
                Service.Stop();
            }

            return Task.CompletedTask;
        }
    }
}