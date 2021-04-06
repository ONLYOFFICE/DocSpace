import {
  precacheAndRoute,
  cleanupOutdatedCaches,
  //createHandlerBoundToURL,
} from "workbox-precaching";
import { setCacheNameDetails } from "workbox-core";
import { clientsClaim } from "workbox-core";
import { /*NavigationRoute,*/ registerRoute } from "workbox-routing";
import { googleFontsCache, imageCache, offlineFallback } from "workbox-recipes";
import {
  //CacheFirst,
  //NetworkFirst,
  StaleWhileRevalidate,
} from "workbox-strategies";
//import { ExpirationPlugin } from "workbox-expiration";
//import { BroadcastUpdatePlugin } from "workbox-broadcast-update";

// SETTINGS

// Claiming control to start runtime caching asap
clientsClaim();

// Use to update the app after user triggered refresh (without prompt)
//self.skipWaiting();

// PRECACHING

// Setting custom cache name
setCacheNameDetails({ precache: "wb6-precache", runtime: "wb6-runtime" });

// We inject manifest here using "workbox-build" in workbox-inject.js
precacheAndRoute(self.__WB_MANIFEST);

// Remove cache from the previous WB versions
cleanupOutdatedCaches();

// NAVIGATION ROUTING

// This assumes /index.html has been precached.
// const navHandler = createHandlerBoundToURL("/index.html");
// const navigationRoute = new NavigationRoute(navHandler, {
//   denylist: [new RegExp("/out-of-spa/")], // Also might be specified explicitly via allowlist
// });
// registerRoute(navigationRoute);

// STATIC RESOURCES

googleFontsCache({ cachePrefix: "wb6-gfonts" });

// TRANSLATIONS

registerRoute(
  ({ url }) => url.pathname.indexOf("/locales/") !== -1,
  // Use cache but update in the background.
  new StaleWhileRevalidate({
    // Use a custom cache name.
    cacheName: "wb6-content-translation",
  })
);

// CONTENT

imageCache({ cacheName: "wb6-content-images", maxEntries: 60 });

// APP SHELL UPDATE FLOW

addEventListener("message", (event) => {
  if (event.data && event.data.type === "SKIP_WAITING") {
    self.skipWaiting();
  }
});

// FALLBACK

offlineFallback({
  pageFallback: "/static/offline/offline.html",
  imageFallback: "/static/offline/offline.svg",
  fontFallback: false,
});

// ALL OTHER EVENTS

// Receive push and show a notification
self.addEventListener("push", function (event) {
  console.log("[Service Worker]: Received push event", event);

  var notificationData = {};

  if (event.data.json()) {
    notificationData = event.data.json();
  } else {
    notificationData = {
      title: "Something Has Happened",
      message: "Something you might want to check out",
      icon: "/assets/img/pwa-logo.png",
    };
  }

  self.registration.showNotification(notificationData.title, notificationData);
});
