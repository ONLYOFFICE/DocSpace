import filesStore from "../store";
import store from "client/store";
import { desktopConstants } from "@docspace/common/desktop";

export function encryptionUploadDialog(callback) {
  const filter = filesStore.settingsStore.extsWebEncrypt
    .map((f) => "*" + f)
    .join(" ");

  const data = {
    cryptoEngineId: desktopConstants.cryptoEngineId,
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
  return store.auth.getEncryptionAccess(file.id).then((keys) => {
    let promise = new Promise((resolve, reject) => {
      try {
        window.AscDesktopEditor.cloudCryptoCommand(
          "share",
          {
            cryptoEngineId: desktopConstants.cryptoEngineId,
            file: [file.viewUrl],
            keys: keys,
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
