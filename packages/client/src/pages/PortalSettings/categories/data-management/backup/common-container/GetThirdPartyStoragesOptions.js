import { ThirdPartyStorages } from "@docspace/common/constants";

export const getOptions = (storageBackup, needDefaultParameter = false) => {
  if (!storageBackup) return;

  let googleStorageId = ThirdPartyStorages.GoogleId;
  let comboBoxOptions = [];
  let storagesInfo = {};
  let isDefaultStorageExist = false;
  let isFirstSet = false;
  let firstSetId = "";
  let selectedStorageTitle = "";
  let selectedStorageId = "";

  for (let item = 0; item < storageBackup.length; item++) {
    const backupElem = storageBackup[item];
    const { isSet, properties, title, id, current } = backupElem;

    comboBoxOptions.push({
      key: id,
      label: title,
      disabled: false,
    });

    storagesInfo = {
      ...storagesInfo,
      [id]: {
        isSet: isSet,
        properties: properties,
        title: title,
        id: id,
      },
    };

    if (needDefaultParameter && current) {
      isDefaultStorageExist = true;
      selectedStorageId = id;
      selectedStorageTitle = title;
    }

    if (!isFirstSet && isSet) {
      isFirstSet = true;
      firstSetId = id;
    }
  }

  if (!isDefaultStorageExist && !isFirstSet) {
    selectedStorageTitle = storagesInfo[googleStorageId].title;
    selectedStorageId = storagesInfo[googleStorageId].id;
  }

  if (!isDefaultStorageExist && isFirstSet) {
    selectedStorageTitle = storagesInfo[firstSetId].title;
    selectedStorageId = storagesInfo[firstSetId].id;
  }

  return {
    comboBoxOptions,
    storagesInfo,
    selectedStorageTitle,
    selectedStorageId,
  };
};
