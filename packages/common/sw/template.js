importScripts(
  "https://storage.googleapis.com/workbox-cdn/releases/6.3.0/workbox-sw.js"
);

workbox.setConfig({
  debug: false,
});

// This will trigger the importScripts() for workbox.strategies and its dependencies:
const { precacheAndRoute, cleanupOutdatedCaches } = workbox.precaching;
const { setCacheNameDetails, clientsClaim } = workbox.core;
const { registerRoute } = workbox.routing;
const { googleFontsCache, imageCache, offlineFallback } = workbox.recipes;
const { StaleWhileRevalidate } = workbox.strategies;

// SETTINGS

// Claiming control to start runtime caching asap
clientsClaim();

// PRECACHING

const prefix = "appserver";

// Setting custom cache name
setCacheNameDetails({
  prefix: prefix,
  precache: "precache",
  runtime: "runtime",
  suffix: "v1.0.0",
});

// We inject manifest here using "workbox-build" in workbox-inject.js
const precachRoutes = self.__WB_MANIFEST;

precacheAndRoute(precachRoutes);

// Remove cache from the previous WB versions
cleanupOutdatedCaches();

// STATIC RESOURCES

googleFontsCache({ cachePrefix: `${prefix}-gfonts` });

// TRANSLATIONS

registerRoute(
  ({ url }) => url.pathname.indexOf("/locales/") !== -1,
  // Use cache but update in the background.
  new StaleWhileRevalidate({
    // Use a custom cache name.
    cacheName: `${prefix}-translation`,
  })
);

// CONTENT

imageCache({ cacheName: `${prefix}-images`, maxEntries: 60 });

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
