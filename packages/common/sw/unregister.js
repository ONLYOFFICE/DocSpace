function clearCaches() {
  try {
    if (!("caches" in window)) return;

    caches?.keys()?.then(function (keyList) {
      return Promise.all(
        keyList.map(function (key) {
          if (
            key.startsWith("workbox-") ||
            key.startsWith("wb6-") ||
            key.startsWith("appserver-")
          ) {
            return caches.delete(key);
          }
        })
      );
    });
  } catch (error) {
    console.error("clearCaches failed", error);
  }
}
export default function () {
  if (process.env.NODE_ENV !== "production" || !("serviceWorker" in navigator))
    return;

  clearCaches();

  return navigator.serviceWorker
    .getRegistrations()
    .then(function (registrations) {
      for (let registration of registrations) {
        registration
          .unregister()
          .then(function () {
            return self.clients?.matchAll() || [];
          })
          .then(function (clients) {
            clients.forEach((client) => {
              if (client.url && "navigate" in client) {
                client.navigate(client.url);
              }
            });
          })
          .catch((err) => {
            console.error(err);
          });
      }
    })
    .catch((err) => {
      console.error(err);
    });
}
