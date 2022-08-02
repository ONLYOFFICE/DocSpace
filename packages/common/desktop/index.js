import toastr from "@docspace/components/toast/toastr";
import isEmpty from "lodash/isEmpty";
import omit from "lodash/omit";

export const desktopConstants = Object.freeze({
  domain: window.location.origin,
  provider: "onlyoffice",
  cryptoEngineId: "{FFF0E1EB-13DB-4678-B67D-FF0A41DBBCEF}",
});

export function regDesktop(
  user,
  isEncryption,
  keys,
  setEncryptionKeys,
  isEditor,
  getEncryptionAccess,
  t
) {
  const data = {
    displayName: user.displayName,
    email: user.email,
    domain: desktopConstants.domain,
    provider: desktopConstants.provider,
    userId: user.id,
  };

  console.log(
    "regDesktop date=",
    data,
    `isEncryption=${isEncryption} keys=${keys} isEditor=${isEditor}`
  );

  let extendedData;

  if (isEncryption) {
    extendedData = {
      ...data,
      encryptionKeys: {
        cryptoEngineId: desktopConstants.cryptoEngineId,
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
        case "updateEncryptionKeys": {
          setEncryptionKeys(params);
          break;
        }
        case "relogin": {
          toastr.info(t("Common:EncryptionKeysReload"));
          //relogin();
          break;
        }
        case "getsharingkeys":
          if (!isEditor || typeof getEncryptionAccess !== "function") {
            callback({});
            return;
          }
          getEncryptionAccess(callback);
          break;
        default:
          break;
      }
    };

    console.log("Created window.cloudCryptoCommand", window.cloudCryptoCommand);
  }

  window.onSystemMessage = (e) => {
    let message = e.opMessage;
    switch (e.type) {
      case "operation":
        if (!message) {
          switch (e.opType) {
            case 0:
              message = t("Common:EncryptionFilePreparing");
              break;
            case 1:
              message = t("Common:EncryptingFile");
              break;
            default:
              message = t("Common:LoadingProcessing");
          }
        }
        toastr.info(message);
        break;
      default:
        break;
    }
  };

  console.log("Created window.onSystemMessage", window.onSystemMessage);
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
