import rootFilesStore from "../store/";
import store from "studio/store";
import { desktopConstants } from "@appserver/common/desktop";

const { docserviceStore } = rootFilesStore.formatsStore;

export function encryptionUploadDialog(callback) {
  const filter = docserviceStore.encryptedDocs.map((f) => "*" + f).join(" ");

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
