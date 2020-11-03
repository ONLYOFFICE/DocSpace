export class Desktop {
  static regDesktop(displayName = null, email = null) {
    const data = {
      displayName: displayName,
      email: email,
      domain: window.location.origin,
      provider: "AppServer",
    };
    return window.AscDesktopEditor.execCommand(
      "portal:login",
      JSON.stringify(data)
    );
  }

  static logout() {
    const data = {
      domain: window.location.origin,
    };
    return window.AscDesktopEditor.execCommand(
      "portal:logout",
      JSON.stringify(data)
    );
  }

  static relogin() {
    return setTimeout(() => {
      const data = {
        domain: window.location.origin,
        onsuccess: "reload",
      };
      window.AscDesktopEditor.execCommand(
        "portal:logout",
        JSON.stringify(data)
      );
    }, 1000);
  }

  static setEncryptionKeys() {
    return {};
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
          return this.setEncryptionKeys();
        case "relogin":
          return {};
        case "getsharingkeys":
          return {};
        default:
          return;
      }
    });
    return cryptoCommand;
  }
}
