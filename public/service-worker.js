const precacheVersion = 1;
const precacheName = 'precache-v' + precacheVersion;
const precacheFiles = [
    '/',
    '/api/2.0/modules', 
    '/api/2.0/people/info.json', 
    '/api/2.0/people/@self.json',
    '/api/2.0/settings.json',
];

self.addEventListener('install', (e) => {
  self.skipWaiting();

  e.waitUntil(
    caches.open(precacheName).then((cache) => {
      return cache.addAll(precacheFiles);
    }) 
  );
});

self.addEventListener('activate', (e) => {
  e.waitUntil(
    caches.keys().then((cacheNames) => {
      return Promise.all(cacheNames.map((thisCacheName) => {

        if (thisCacheName.includes("precache") && thisCacheName !== precacheName) {
          console.log('[ServiceWorker] Removing cached files from old cache - ', thisCacheName);
          return caches.delete(thisCacheName);
        }

      }));
    })
  );
});

self.addEventListener('fetch', (e) => {
  e.respondWith(
    caches.match(e.request).then((cachesResponse) => {
          if(cachesResponse){
              return cachesResponse;
          }
          return fetch(e.request)
            .then((fetchResponse) => {
                return fetchResponse;
            })
            .catch((err) => {
                return caches.match("/")
            });
      })
  );
});