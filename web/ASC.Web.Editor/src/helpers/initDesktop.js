import {
  setEncryptionKeys,
  getEncryptionAccess,
} from "@appserver/common/api/files";
import { regDesktop } from "@appserver/common/desktop";

const initDesktop = (cfg, user, fileId, t) => {
  const encryptionKeys = cfg?.editorConfig?.encryptionKeys;
  console.log("here");
  regDesktop(
    user,
    !!encryptionKeys,
    encryptionKeys,
    (keys) => {
      setEncryptionKeys(keys);
    },
    true,
    (callback) => {
      getEncryptionAccess(fileId)
        .then((keys) => {
          var data = {
            keys,
          };

          callback(data);
        })
        .catch((error) => {
          console.log(error);
          window?.toastr.error(
            typeof error === "string" ? error : error.message,
            null,
            0,
            true
          );
        });
    },
    t
  );
};

export default initDesktop;
