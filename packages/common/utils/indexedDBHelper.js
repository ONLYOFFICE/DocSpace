const DB_VERSION = 1;

class IndexedDBHelper {
  constructor() {
    this.db = null;
  }

  init = async (userId, storeNames) => {
    return new Promise((resolve, reject) => {
      const idb =
        window?.indexedDB ||
        window?.webkitIndexedDB ||
        window?.mozIndexedDB ||
        window?.OIndexedDB ||
        window?.msIndexedDB;

      if (!idb) {
        this.setDB(null);
        reject();
      }

      const request = idb.open(`${userId}`, DB_VERSION);

      request.onupgradeneeded = (event) => {
        const db = event.target.result;
        storeNames.forEach((store) => {
          if (!db.objectStoreNames.contains(store)) {
            db.createObjectStore(store, { keyPath: "id" });
          }
        });
      };

      request.onerror = () => {
        console.error("Error", request.error);
        reject(request.error);
      };

      request.onsuccess = (event) => {
        this.setDB(event.target.result);
        resolve();
      };
    });
  };

  setDB = (db) => {
    this.db = db;
  };

  getDB = () => {
    return this.db;
  };

  deleteStore = (storeName) => {
    this.db.deleteObjectStore(storeName);
  };

  addItem = (storeName, item) => {
    return new Promise(async (resolve, reject) => {
      try {
        const transaction = this.db.transaction(storeName, "readwrite");

        const store = transaction.objectStore(storeName);

        await store.add(item);
        resolve();
      } catch (e) {
        reject(e);
      }
    });
  };

  getItem = (storeName, id) => {
    return new Promise(async (resolve, reject) => {
      try {
        const transaction = this.db.transaction(storeName, "readonly");

        const store = transaction.objectStore(storeName);

        const request = store.get(id);

        request.onsuccess = (event) => {
          resolve(event.target.result);
        };

        request.onerror = () => {
          console.error("Error", request.error);
          reject(request.error);
        };
      } catch (e) {
        reject(e);
      }
    });
  };
}

const indexedDbHelper = new IndexedDBHelper();

export default indexedDbHelper;
