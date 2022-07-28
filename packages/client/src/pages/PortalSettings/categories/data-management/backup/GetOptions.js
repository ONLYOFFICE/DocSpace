import { ThirdPartyStorages } from "@docspace/common/constants";

export const getOptions = (storageBackup) => {
  let googleStorageId = ThirdPartyStorages.GoogleId;
  let options = [];
  let availableStorage = {};
  let firstSet = false;
  let firstSetId = "";
  let selectedStorage = "";
  let selectedId = "";

  for (let item = 0; item < storageBackup.length; item++) {
    let obj = {
      [storageBackup[item].id]: {
        isSet: storageBackup[item].isSet,
        properties: storageBackup[item].properties,
        title: storageBackup[item].title,
        id: storageBackup[item].id,
      },
    };
    let titleObj = {
      key: storageBackup[item].id,
      label: storageBackup[item].title,
      disabled: false,
    };
    options.push(titleObj);

    availableStorage = { ...availableStorage, ...obj };

    if (!firstSet && storageBackup[item].isSet) {
      firstSet = true;
      firstSetId = storageBackup[item].id;
    }
  }

  if (!firstSet) {
    selectedStorage = availableStorage[googleStorageId].title;
    selectedId = availableStorage[googleStorageId].id;
  }

  if (firstSet) {
    selectedStorage = availableStorage[firstSetId].title;
    selectedId = availableStorage[firstSetId].id;
  }

  return {
    options,
    availableStorage,
    selectedStorage,
    selectedId,
  };
};
