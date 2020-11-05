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
        return Desktop.setEncryptionKeys(params);
      case "relogin":
        return Desktop.relogin();
      case "getsharingkeys":
        return {};
      default:
        return;
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
      data.cryptoEngineId = "{FFF0E1EB-13DB-4678-B67D-FF0A41DBBCEF}";
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

  static setEncryptionKeys() {
    return {
      // publicKey: encryptionKeys.publicKey,
      // privateKeyEnc: encryptionKeys.privateKeyEnc
    };
  }

  static setAccess() {
    return {};
  }

  static encryptionUploadDialog() {
    return {};
  }

  static cloudCryptoCommand() {
    const cryptoCommand = (window.cloudCryptoCommand = (
      type,
      params,
      callback
    ) => {
      switch (type) {
        case "encryptionKeys":
          return this.setEncryptionKeys(params);
        case "relogin":
          return this.relogin();
        case "getsharingkeys":
          return {};
        default:
          return;
      }
    });
    return cryptoCommand;
  }
}
