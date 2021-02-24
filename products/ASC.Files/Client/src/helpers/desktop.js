import formatsStore from "../store/FormatsStore";
import { desktopConstants, store } from "asc-web-common";

const { authStore } = store;
const { docserviceStore } = formatsStore;

export function encryptionUploadDialog(callback) {
  const filter = docserviceStore.encryptedDocs.map((f) => "*" + f).join(" ");

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
  return authStore.getEncryptionAccess(file.id).then((keys) => {
    let promise = new Promise((resolve, reject) => {
      try {
        window.AscDesktopEditor.cloudCryptoCommand(
          "share",
          {
            cryptoEngineId: desktopConstants.guid,
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
