const precacheVersion = 1;
const precacheName = 'precache-v' + precacheVersion;
const precacheFiles = [
    '/',
    '/bg-error.png',
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
    fetch(e.request)
      .then((fetchResponse) => {
        e.waitUntil(
          update(e.request).then(refresh)
        );
        return fetchResponse;
      })
      .catch((err) => {
        return caches.match(e.request)
          .then((cachesResponse) =>{
            if(cachesResponse){
              return cachesResponse;
            }else{
              return useFallback();
            }
          })
      })
  );
});

function update(request) {
    return caches.open(precacheName).then((cache) =>
      {
        caches.match(request).then((cachesResponse) => {
          if(cachesResponse){
            fetch(request).then((response) =>
                cache.put(request, response.clone()).then(() => response)
            ).catch((err) => console.log(`[ServiceWorker] ${err}`));
          }
        })
      }
    );
}

const fallback = 
'<div style="cursor: default;background: url(/bg-error.png);width: 100%;height: 310px;padding: 315px 0 0;background-repeat: no-repeat;background-position: 50%;'+
            'margin-top: -325px;'+
            'position: absolute;'+
            'left: 0;'+
            'top: 50%;'+
            'z-index: 1;">'+
'<div style="width: 310px;'+
            'margin: 0 auto;'+
            'padding-left: 38px;'+
            'text-align: center;'+
            'font: normal 24px/35px Tahoma;'+
            'color: #275678;'+
            'position: relative;">\n'+
    '<p>No internet connection found.</p>\n'+
'</div>\n'+
'</div>';

function useFallback() {
  return Promise.resolve(new Response(fallback, { headers: {
      'Content-Type': 'text/html; charset=utf-8'
  }}));
}

function refresh(response) {

  //Send data update messages to all clients

  /*return self.clients.matchAll().then((clients) => {
      clients.forEach((client) => {
          const message = {
              type: 'refresh',
              url: response.url,
              eTag: response.headers.get('ETag')
          };
          client.postMessage(JSON.stringify(message));
      });
  });*/
}