if ("function" === typeof importScripts) {
  importScripts(
    "https://storage.googleapis.com/workbox-cdn/releases/5.1.2/workbox-sw.js"
  );
  /* global workbox */
  if (workbox) {
    console.log("Workbox is loaded");

    // Force development builds -> { debug: true } or production builds { debug: false }
    workbox.setConfig({ debug: false });

    // Updating SW lifecycle to update the app after user triggered refresh
    workbox.core.skipWaiting();
    workbox.core.clientsClaim();

    /* injection point for manifest files.  */
    workbox.precaching.precacheAndRoute(self.__WB_MANIFEST);

    // Image caching
    workbox.routing.registerRoute(
      // Cache image files.
      ({ request }) => request.destination === "image",
      // Use the cache if it's available.
      new workbox.strategies.CacheFirst({
        // Use a custom cache name.
        cacheName: "image-cache",
        plugins: [
          new workbox.expiration.ExpirationPlugin({
            // Cache only 60 images.
            maxEntries: 60,
            // Cache for a maximum of a month.
            maxAgeSeconds: 30 * 24 * 60 * 60, // 30 Days
          }),
        ],
      })
    );

    // Font caching
    workbox.routing.registerRoute(
      new RegExp("https://fonts.(?:.googlepis|gstatic).com/(.*)"),
      new workbox.strategies.CacheFirst({
        cacheName: "googleapis",
        plugins: [
          new workbox.expiration.ExpirationPlugin({
            // Cache only 60 images.
            maxEntries: 60,
            // Cache for a maximum of a month.
            maxAgeSeconds: 30 * 24 * 60 * 60, // 30 Days
          }),
        ],
      })
    );

    // CSS caching
    workbox.routing.registerRoute(
      // Cache style resources, i.e. CSS files.
      ({ request }) => request.destination === "style",
      // Use cache but update in the background.
      new workbox.strategies.StaleWhileRevalidate({
        // Use a custom cache name.
        cacheName: "css-cache",
      })
    );

    // scripts caching
    workbox.routing.registerRoute(
      // Cache style resources, i.e. CSS files.
      ({ request }) => request.destination === "script",
      // Use cache but update in the background.
      new workbox.strategies.StaleWhileRevalidate({
        // Use a custom cache name.
        cacheName: "script-cache",
      })
    );

    // translations caching
    workbox.routing.registerRoute(
      ({ url }) => url.pathname.endsWith("/translation.json"),
      // Use cache but update in the background.
      new workbox.strategies.StaleWhileRevalidate({
        // Use a custom cache name.
        cacheName: "translation-cache",
      })
    );

    workbox.routing.registerRoute(
      // Cache API Request
      new RegExp("/api/2.0/(modules|people/@self|(.*)/info(.json|$))"),
      new workbox.strategies.StaleWhileRevalidate({
        cacheName: "api-cache",
        plugins: [
          new workbox.expiration.ExpirationPlugin({
            maxEntries: 100,
            maxAgeSeconds: 30 * 60, // 30 Minutes
          }),
        ],
      })
    );
  } else {
    console.log("Workbox could not be loaded. No Offline support");
  }
}
