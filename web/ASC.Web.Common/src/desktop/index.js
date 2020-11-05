const domain = window.location.origin;
const provider = "AppServer";

if (
  window["AscDesktopEditor"] &&
  typeof window.AscDesktopEditor.cloudCryptoCommand === "function"
) {
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
