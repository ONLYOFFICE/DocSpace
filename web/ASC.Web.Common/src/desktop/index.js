import { toastr } from "asc-web-common";
import {
  setEncryptionKeys as setKeys,
  //getEncryptionAccess,
} from "../store/auth/actions";
import assign from "lodash/assign";
import isEmpty from "lodash/isEmpty";

//import { store } from "asc-web-common";

// const { isDesktopClient, isEncryptionSupport } = store.auth.selectors;

//const state = store.getState();

const domain = window.location.origin;
const provider = "AppServer";
const guid = "{FFF0E1EB-13DB-4678-B67D-FF0A41DBBCEF}";
const desktop = window["AscDesktopEditor"] !== undefined;
const encryption =
  (desktop &&
    typeof window.AscDesktopEditor.cloudCryptoCommand === "function") ||
  false;

const encryptionKeyPair = {
  privateKeyEnc: "1",
  publicKey: "2",
};

export function regDesktop(user, isEncryption) {
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

    if (!isEmpty(encryptionKeyPair)) {
      assign(data, {
        encryptionKeys: {
          cryptoEngineId: guid,
          privateKeyEnc: encryptionKeyPair.privateKeyEnc,
          publicKey: encryptionKeyPair.publicKey,
        },
      });
    } else assign(data, {});
  }
  console.log("regdesktop");

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
        return toastr.info(message);
      default:
        break;
    }
  };

  if (isEncryption) {
    window.cloudCryptoCommand = (type, params, callback) => {
      console.log("cloudCryptoCommand", params);
      switch (type) {
        case "encryptionKeys":
          setEncryptionKeys(params);
          break;
        case "relogin":
          relogin();
          break;
        case "getsharingkeys":
          break;
        default:
          break;
      }
    };
  }
}

export function relogin() {
  toastr.info("Encryption keys must be re-entered");
  const data = {
    domain,
    onsuccess: "reload",
  };
  const execCommand = setTimeout(() => {
    window.AscDesktopEditor.execCommand("portal:logout", JSON.stringify(data));
  }, 1000);
  return execCommand;
}

export function setEncryptionKeys(encryptionKeys) {
  console.log("encryptionKeys: ", encryptionKeys);
  if (!encryptionKeys.publicKey || !encryptionKeys.privateKeyEnc) {
    return toastr.info("Empty encryption keys");
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
  const execCommand = window.AscDesktopEditor.execCommand(
    "portal:checkpwd",
    JSON.stringify(data)
  );

  return execCommand;
}

export function logout() {
  const data = {
    domain,
  };
  const execCommand = window.AscDesktopEditor.execCommand(
    "portal:logout",
    JSON.stringify(data)
  );
  return execCommand;
}
