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

namespace ASC.Files.ThumbnailBuilder
{
    [Singletone(Additional = typeof(WorkerExtension))]
    public class Launcher : IHostedService
    {
        internal static readonly ConcurrentDictionary<object, FileData<int>> Queue 
            = new ConcurrentDictionary<object, FileData<int>>();

        private Worker _worker;
        private readonly Service Service;

        public Launcher(Service service, Worker worker)
        {
            Service = service;
            _worker = worker;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _worker.Start(cancellationToken);
            Service.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_worker != null)
            {
                _worker.Stop();
                _worker = null;
            }

            if (Service != null)
            {
                Service.Stop();
            }

            return Task.CompletedTask;
        }
    }
}