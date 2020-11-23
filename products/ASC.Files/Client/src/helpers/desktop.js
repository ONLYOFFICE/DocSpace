import store from "../store/store";
import { getEncryptedFormats } from "../store/files/selectors";

export function encryptionUploadDialog(callback) {
  const state = store.getState();
  const filter = getEncryptedFormats(state).map((f) => "*" + f);
  const data = {
    cryptoEngineId: "{FFF0E1EB-13DB-4678-B67D-FF0A41DBBCEF}",
    filter: filter,
  };
  window.AscDesktopEditor.cloudCryptoCommand(
    "upload",
    JSON.stringify(data),
    function (obj) {
      let bytes = obj.bytes;
      let filename = obj.name;
      let file = new File([bytes], filename);

      if (typeof callback == "function") {
        callback(file, obj.isCrypto !== false);
      }
    }
  );
}
