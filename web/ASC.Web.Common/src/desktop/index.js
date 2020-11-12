import { toastr } from "asc-web-common";
import {
  setEncryptionKeys as setKeys,
  getEncryptionAccess,
} from "../store/auth/actions";
import { isDesktopClient, isEncryptionSupport } from "../store/auth/selectors";

const domain = window.location.origin;
const provider = "AppServer";
const guid = "{FFF0E1EB-13DB-4678-B67D-FF0A41DBBCEF}";
const desktop = isDesktopClient;
const encryption = isEncryptionSupport;
const encryptionKeyPair = {
  privateKeyEnc: "WL0scvx5V3AF9BiD",
  publicKey: "&#xAMIIBIjANBgkq",
};

if (encryption) {
  window.cloudCryptoCommand = (type, params, callback) => {
    switch (type) {
      case "encryptionKeys":
        setEncryptionKeys(params);
        break;
      case "relogin":
        Desktop.relogin();
        break;
      case "getsharingkeys":
        Desktop.setAccess();
        break;
      default:
        break;
    }
  };

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
        return toastr.info(message);
      default:
        break;
    }
  };
}

export function regDesktop(displayName, email, userId) {
  const data = {
    displayName,
    email,
    domain,
    provider,
    userId,
  };
  if (encryption) {
    if (encryptionKeyPair) {
      data.encryptionKeys.cryptoEngineId = guid;
      data.encryptionKeys = encryptionKeyPair;
    }
  }
  const execCommand = window.AscDesktopEditor.execCommand(
    "portal:login",
    JSON.stringify(data)
  );
  return execCommand;
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
