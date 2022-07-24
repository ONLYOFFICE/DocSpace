export const onConvertFiles = (e, resolve) => {
  const items = e.dataTransfer.items;
  const files = [];

  const callItemFromQueue = (queue, callback) => {
    let i = 0;
    let queueLength = queue.length;

    if (!queue || !queue.length) {
      callback();
    }

    const callNext = (i) => {
      queue[i]((error) => {
        if (!error && i + 1 < queueLength) {
          i++;
          callNext(i);
        } else {
          callback(error);
        }
      });
    };
    callNext(i);
  };

  const readDirEntry = (dirEntry, callback) => {
    const entries = [];
    const dirReader = dirEntry.createReader();

    const getEntries = (entriesCallback) => {
      dirReader.readEntries((moreEntries) => {
        if (moreEntries.length) {
          Array.prototype.push.apply(entries, moreEntries);
          getEntries(entriesCallback);
        } else {
          entriesCallback();
        }
      }, entriesCallback);
    };

    getEntries(() => {
      readEntries(entries, callback);
    });
  };

  const readEntry = (entry, callback) => {
    if (entry.isFile) {
      entry.file(
        (file) => {
          addFile(file, entry.fullPath);
          callback();
        },
        () => {
          callback();
        }
      );
    } else if (entry.isDirectory) {
      readDirEntry(entry, callback);
    }
  };

  const readEntries = (entries, callback) => {
    const queue = [];
    loop(entries, (entry) => {
      queue.push((func) => {
        readEntry(entry, func);
      });
    });
    callItemFromQueue(queue, () => callback());
  };

  const addFile = (file, relativePath) => {
    file.path = relativePath || "";
    files.push(file);
  };

  const loop = (items, callback) => {
    let itemsLength;

    if (items) {
      try {
        itemsLength = items.length;
      } catch (err) {
        itemsLength = null;
      }

      if (itemsLength === null || typeof itemsLength !== "number") {
        // Loop object items
        for (let key in items) {
          if (items.hasOwnProperty(key)) {
            if (callback(items[key], key) === false) {
              return;
            }
          }
        }
      } else {
        // Loop array items
        for (let i = 0; i < itemsLength; i++) {
          if (callback(items[i], i) === false) {
            return;
          }
        }
      }
    }
  };

  const readItems = (items, callback) => {
    const entries = [];
    loop(items, (item) => {
      const entry = item.webkitGetAsEntry();
      if (entry) {
        if (entry.isFile) {
          addFile(item.getAsFile(), entry.fullPath);
        } else {
          entries.push(entry);
        }
      }
    });

    if (entries.length) {
      readEntries(entries, callback);
    } else {
      callback();
    }
  };

  readItems(items, () => resolve(files));
};
