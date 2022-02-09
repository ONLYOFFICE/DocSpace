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
    [Singletone]
    public class Service
    {
        private ICacheNotify<ThumbnailRequest> CacheNotify { get; }

        public Service(ICacheNotify<ThumbnailRequest> cacheNotify)
        {
            CacheNotify = cacheNotify;
        }

        public void Start()
        {
            CacheNotify.Subscribe(BuildThumbnails, CacheNotifyAction.Insert);
            //Cache.Subscribe
        }

        public void Stop()
        {
            CacheNotify.Unsubscribe(CacheNotifyAction.Insert);
            //Cache.Subscribe
        }

        public void BuildThumbnails(ThumbnailRequest request)
        {
            foreach (var fileId in request.Files)
            {
                var fileData = new FileData<int>(request.Tenant, fileId, request.BaseUrl);

                Launcher.Queue.TryAdd(fileId, fileData);
            }
        }
    }
}