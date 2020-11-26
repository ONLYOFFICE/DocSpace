import store from "../store/store";
import { store as commonStore } from "asc-web-common";
import { getEncryptedFormats } from "../store/files/selectors";

const domain = window.location.origin;
const guid = "{FFF0E1EB-13DB-4678-B67D-FF0A41DBBCEF}";
const { getEncryptionAccess } = commonStore.auth.actions;

export function encryptionUploadDialog(callback) {
  const state = store.getState();
  const filter = getEncryptedFormats(state)
    .map((f) => "*" + f)
    .join(" ");

  const data = {
    cryptoEngineId: guid,
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

export function setEncryptionAccess(file, callback) {
  debugger;
  getEncryptionAccess(file.id).then((keys) => {
    window.AscDesktopEditor.cloudCryptoCommand(
      "share",
      {
        "cryptoEngineId": guid,
        "file": [file.viewUrl],
        "keys": [
          {
            "publicKey":
              "-----BEGIN PUBLIC KEY-----&#xAMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEApEKtEiSlig1Ue3JF6ajv&#xAtlWXEDdd/zcBmKUpkVtgi3gvCbGFB2VnUZRgOBWLQ8Bx+VU5beFlg0+/jUNSIzs1&#xAMwjGFa17CV8CxaZmtwTZjDwkfozWopttwxHfRIaOV8t2ZFB2V2qoGBCC4vxeF2/t&#xAMkNOgAnhVjH8Pq3uy5oOwzlZgU5u93ly12Jpa/bl2xGiXAqJpPH8s7ceSWBe/0Ky&#xAiDRz1DtRMs2elWQ6ag+tZwBk3Ee+j+ffK62d2n/B6ksY9oZ/joyzaHzjgeKI4+3E&#xAxW0Wh4zt/EEuypc6ySVd6+3WafRRqvQm+tXpolX6NL9oeCsyj0YrQGVcg6qm7BXn&#xABQIDAQAB&#xA-----END PUBLIC KEY-----&#xA",
            "userId": "66faa6e4-f133-11ea-b126-00ffeec8b4ef",
          },
        ],
      },
      (obj) => {
        let fileItem = null;
        if (obj.isCrypto !== false) {
          let bytes = obj.bytes;
          let filename = "temp_name";
          fileItem = new File([bytes], filename);
        }

        if (typeof callback == "function") {
          callback(fileItem);
        }
      }
    );
  });
}
