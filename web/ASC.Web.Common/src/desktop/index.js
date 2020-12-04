import { toastr } from "asc-web-common";
import isEmpty from "lodash/isEmpty";
import omit from "lodash/omit";
//import { getEncryptionAccess } from "../store/auth/actions";

export const desktopConstants = Object.freeze({
  domain: window.location.origin,
  provider: "AppServer",
  guid: "{FFF0E1EB-13DB-4678-B67D-FF0A41DBBCEF}",
});

const domain = window.location.origin;
const provider = "AppServer";
const guid = "{FFF0E1EB-13DB-4678-B67D-FF0A41DBBCEF}";

export function regDesktop(user, isEncryption, keys, setEncryptionKeys) {
  const data = {
    displayName: user.displayName,
    email: user.email,
    domain: desktopConstants.domain,
    provider: desktopConstants.provider,
    userId: user.id,
  };

  let extendedData;

  if (isEncryption) {
    extendedData = {
      ...data,
      encryptionKeys: {
        cryptoEngineId: desktopConstants.guid,
      },
    };

    if (!isEmpty(keys)) {
      const filteredKeys = omit(keys, ["userId"]);
      extendedData = {
        ...extendedData,
        encryptionKeys: { ...extendedData.encryptionKeys, ...filteredKeys },
      };
    }
  } else {
    extendedData = { ...data };
  }

  window.AscDesktopEditor.execCommand(
    "portal:login",
    JSON.stringify(extendedData)
  );

  if (isEncryption) {
    window.cloudCryptoCommand = (type, params, callback) => {
      switch (type) {
        case "encryptionKeys": {
          setEncryptionKeys(params);
          break;
        }
        case "relogin": {
          toastr.info("Encryption keys must be re-entered");
          //relogin();
          break;
        }
        case "getsharingkeys":
          console.log(
            "%c%s",
            "color: green; font: 1.1em/1 bold;",
            "Get sharing keys"
          );
          toastr.info("get sharing keys");
          break;
        default:
          break;
      }
    };
  }

  window.onSystemMessage = (e) => {
    let message = e.opMessage;
    switch (e.type) {
      case "operation":
        if (!message) {
          switch (e.opType) {
            case 0:
              message = "Preparing file for encryption";
              break;
            case 1:
              message = "Encrypting file";
              break;
            default:
              message = "Loading...";
          }
        }
        toastr.info(message);
        break;
      default:
        break;
    }
  };
}

export function relogin() {
  setTimeout(() => {
    const data = {
      domain: desktopConstants.domain,
      onsuccess: "reload",
    };
    window.AscDesktopEditor.execCommand("portal:logout", JSON.stringify(data));
  }, 1000);
}

export function checkPwd() {
  const data = {
    domain: desktopConstants.domain,
    emailInput: "login",
    pwdInput: "password",
  };
  window.AscDesktopEditor.execCommand("portal:checkpwd", JSON.stringify(data));
}

export function logout() {
  const data = {
    domain: desktopConstants.domain,
  };
  window.AscDesktopEditor.execCommand("portal:logout", JSON.stringify(data));
}

// export function setEncryptionAccess(file, callback) {
//   getEncryptionAccess(file.id).then((keys) => {
//     console.log(
//       "%c%s",
//       "color: green; font: 1.1em/1 bold;",
//       "Fetch keys: ",
//       keys
//     );
//     window.AscDesktopEditor.cloudCryptoCommand(
//       "share",
//       {
//         "cryptoEngineId": guid,
//         "file": [file.viewUrl],
//         "keys": [
//           {
//             "publicKey":
//               "-----BEGIN PUBLIC KEY-----&#xAMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEApEKtEiSlig1Ue3JF6ajv&#xAtlWXEDdd/zcBmKUpkVtgi3gvCbGFB2VnUZRgOBWLQ8Bx+VU5beFlg0+/jUNSIzs1&#xAMwjGFa17CV8CxaZmtwTZjDwkfozWopttwxHfRIaOV8t2ZFB2V2qoGBCC4vxeF2/t&#xAMkNOgAnhVjH8Pq3uy5oOwzlZgU5u93ly12Jpa/bl2xGiXAqJpPH8s7ceSWBe/0Ky&#xAiDRz1DtRMs2elWQ6ag+tZwBk3Ee+j+ffK62d2n/B6ksY9oZ/joyzaHzjgeKI4+3E&#xAxW0Wh4zt/EEuypc6ySVd6+3WafRRqvQm+tXpolX6NL9oeCsyj0YrQGVcg6qm7BXn&#xABQIDAQAB&#xA-----END PUBLIC KEY-----&#xA",
//             "userId": "66faa6e4-f133-11ea-b126-00ffeec8b4ef",
//           },
//         ],
//       },
//       (obj) => {
//         console.log(
//           "%c%s",
//           "color: green; font: 1.1em/1 bold;",
//           "obj item: ",
//           obj
//         );
//         let fileItem = null;
//         if (obj.isCrypto !== false) {
//           let bytes = obj.bytes;
//           let filename = file.title;
//           fileItem = new File([bytes], filename);
//         }
//         console.log(
//           "%c%s",
//           "color: green; font: 1.1em/1 bold;",
//           "File item: ",
//           fileItem
//         );
//         if (typeof callback == "function") {
//           callback(fileItem);
//         }
//       }
//     );
//   });
// }
