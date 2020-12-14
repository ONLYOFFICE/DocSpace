import store from "../store/store";
import { store as commonStore } from "asc-web-common";
import { getEncryptedFormats } from "../store/files/selectors";
import { desktopConstants } from "asc-web-common";

const { getEncryptionAccess } = commonStore.auth.actions;

export function encryptionUploadDialog(callback) {
  const state = store.getState();
  const filter = getEncryptedFormats(state)
    .map((f) => "*" + f)
    .join(" ");

  const data = {
    cryptoEngineId: desktopConstants.guid,
    filter: filter,
  };

  window.AscDesktopEditor.cloudCryptoCommand("upload", data, function (obj) {
    let bytes = obj.bytes;
    let filename = obj.name;
    let file = new File([bytes], filename);

    if (typeof callback == "function") {
      callback(file, obj.isCrypto !== false);
    }
  });
}

export function setEncryptionAccess(file) {
  return getEncryptionAccess(file.id).then((keys) => {
    let promise = new Promise((resolve, reject) => {
      try {
        window.AscDesktopEditor.cloudCryptoCommand(
          "share",
          {
            "cryptoEngineId": desktopConstants.guid,
            "file": [file.viewUrl],
            "keys": keys,
          },
          (obj) => {
            let file = null;
            if (obj.isCrypto) {
              let bytes = obj.bytes;
              let filename = "temp_name";
              file = new File([bytes], filename);
            }
            resolve(file);
          }
        );
      } catch (e) {
        reject(e);
      }
    });
    return promise;
  });
}
