import { toastr } from "asc-web-common";
import { setEncryptionKeys, getEncryptionAccess } from "../store/auth/actions";

const domain = window.location.origin;
const provider = "AppServer";

const isDesktop = window["AscDesktopEditor"] || false;
const isEncryptionSupport =
  (window["AscDesktopEditor"] &&
    typeof window.AscDesktopEditor.cloudCryptoCommand === "function") ||
  false;

if (isDesktop && isEncryptionSupport) {
  window.cloudCryptoCommand = (type, params, callback) => {
    switch (type) {
      case "encryptionKeys":
        Desktop.setEncryptionKeys(params);
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

export class Desktop {
  static regDesktop(displayName, email, userId) {
    const data = {
      displayName,
      email,
      domain,
      provider,
      userId,
    };
    if (isEncryptionSupport) {
      data.encryptionKeys.cryptoEngineId =
        "{FFF0E1EB-13DB-4678-B67D-FF0A41DBBCEF}";
    } else {
      return;
    }

    const execCommand = window.AscDesktopEditor.execCommand(
      "portal:login",
      JSON.stringify(data)
    );
    return execCommand;
  }

  static checkPwd() {
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

  static relogin() {
    toastr.info("Encryption keys must be re-entered");
    const data = {
      domain,
      onsuccess: "reload",
    };
    const execCommand = setTimeout(() => {
      window.AscDesktopEditor.execCommand(
        "portal:logout",
        JSON.stringify(data)
      );
    }, 1000);
    return execCommand;
  }

  static logout() {
    const data = {
      domain,
    };
    const execCommand = window.AscDesktopEditor.execCommand(
      "portal:logout",
      JSON.stringify(data)
    );
    return execCommand;
  }

  static setEncryptionKeys(encryptionKeys) {
    if (!encryptionKeys.publicKey || !encryptionKeys.privateKeyEnc) {
      return toastr.info("Empty encryption keys");
    }
    const data = {
      publicKey: encryptionKeys.publicKey,
      privateKeyEnc: encryptionKeys.privateKeyEnc,
    };
    return setEncryptionKeys(data);
  }

  static setAccess(fileId) {
    return getEncryptionAccess(fileId);
  }

  static encryptionUploadDialog() {
    return {};
  }
}
