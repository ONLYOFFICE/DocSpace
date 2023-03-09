const DB_VERSION = 1;
const MAX_COUNT_STORE = 30;

const idb =
  window?.indexedDB ||
  window?.webkitIndexedDB ||
  window?.mozIndexedDB ||
  window?.OIndexedDB ||
  window?.msIndexedDB;

class IndexedDBHelper {
  constructor() {
    this.db = null;
    this.ignoreIds = [];
    this.firstCheck = false;
  }

  init = async (userId, storeNames) => {
    return new Promise((resolve, reject) => {
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

  deleteDatabase = (dbName) => {
    idb.deleteDatabase(`${dbName}`);
  };

  clearStore = (storeName) => {
    return new Promise(async (resolve, reject) => {
      try {
        const transaction = this.db.transaction(storeName, "readwrite");

        const store = transaction.objectStore(storeName);

        await store.clear();
        resolve();
      } catch (e) {
        reject(e);
      }
    });
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
        this.ignoreIds.push(item.id);

        this.checkStore(store);
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

  checkStore = (store) => {
    let newIgnoreIds = [...this.ignoreIds];

    newIgnoreIds = newIgnoreIds.filter(
      (id, index) => newIgnoreIds.indexOf(id) === index
    );

    if (!this.firstCheck) {
      this.firstCheck = true;
      const countRequest = store.getAllKeys();

      countRequest.onsuccess = () => {
        newIgnoreIds = [...countRequest.result, ...newIgnoreIds];
        if (newIgnoreIds.length > MAX_COUNT_STORE) {
          store.delete(newIgnoreIds.shift());
        }

        this.ignoreIds = newIgnoreIds;
      };
    } else {
      if (newIgnoreIds.length > MAX_COUNT_STORE) {
        store.delete(newIgnoreIds.shift());
      }
      this.ignoreIds = newIgnoreIds;
    }
  };
}

const indexedDbHelper = new IndexedDBHelper();

export default indexedDbHelper;
