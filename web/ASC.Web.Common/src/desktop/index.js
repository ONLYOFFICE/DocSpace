import { toastr } from "asc-web-common";
import {
  setEncryptionKeys as setKeys,
  //getEncryptionAccess,
} from "../store/auth/actions";
import assign from "lodash/assign";
import isEmpty from "lodash/isEmpty";

const domain = window.location.origin;
const provider = "AppServer";
const guid = "{FFF0E1EB-13DB-4678-B67D-FF0A41DBBCEF}";
const desktop = window["AscDesktopEditor"] !== undefined;

export function regDesktop(user, isEncryption, keys) {
  if (!desktop) return;

  const data = {
    displayName: user.displayName,
    email: user.email,
    domain,
    provider,
    userId: user.id,
  };

  if (isEncryption) {
    assign(data, {
      encryptionKeys: {
        cryptoEngineId: guid,
      },
    });

    if (!isEmpty(keys)) {
      assign(data, {
        encryptionKeys: {
          cryptoEngineId: guid,
          privateKeyEnc: keys.privateKeyEnc,
          publicKey: keys.publicKey,
        },
      });
    }
    //else assign(data, {});
  }

  window.AscDesktopEditor.execCommand("portal:login", JSON.stringify(data));

  window.onSystemMessage = (e) => {
    console.log("onSystemMessage: ", e);
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

  if (isEncryption) {
    window.cloudCryptoCommand = (type, params, callback) => {
      console.log("cloudCryptoCommand", params);
      switch (type) {
        case "encryptionKeys": {
          setEncryptionKeys(params);
          break;
        }
        case "relogin": {
          toastr.info("Encryption keys must be re-entered");
          relogin();
          break;
        }
        case "getsharingkeys":
          break;
        default:
          break;
      }
    };
  }
}

export function relogin() {
  setTimeout(() => {
    const data = {
      domain,
      onsuccess: "reload",
    };
    window.AscDesktopEditor.execCommand("portal:logout", JSON.stringify(data));
  }, 1000);
}

export function setEncryptionKeys(encryptionKeys) {
  console.log("encryptionKeys: ", encryptionKeys);

  if (!encryptionKeys.publicKey || !encryptionKeys.privateKeyEnc) {
    toastr.info("Empty encryption keys");
    return;
  }
  const data = {
    publicKey: encryptionKeys.publicKey,
    privateKeyEnc: encryptionKeys.privateKeyEnc,
  };
  return setKeys(data);
}

export function checkPwd() {
  const data = {
    domain,
    emailInput: "login",
    pwdInput: "password",
  };
  window.AscDesktopEditor.execCommand("portal:checkpwd", JSON.stringify(data));
}

export function logout() {
  const data = {
    domain,
  };
  window.AscDesktopEditor.execCommand("portal:logout", JSON.stringify(data));
}
