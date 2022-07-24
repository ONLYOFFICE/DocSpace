import React from "react";

import { inject, observer } from "mobx-react";
import { runInAction } from "mobx";

import ToggleButton from "@docspace/components/toggle-button";

import StyledSettings from "./StyledSettings";

const AdminSettings = ({
  storeForceSave,
  setStoreForceSave,
  enableThirdParty,
  setEnableThirdParty,
  treeFolders,
  myFolderId,
  commonFolderId,
  getSubfolders,
  t,
}) => {
  const onChangeStoreForceSave = React.useCallback(() => {
    setStoreForceSave(!storeForceSave);
  }, [setStoreForceSave, storeForceSave]);

  const onChangeThirdParty = React.useCallback(() => {
    setEnableThirdParty(!enableThirdParty, "enableThirdParty").then(
      async () => {
        const myFolders = await getSubfolders(myFolderId);
        const commonFolders = await getSubfolders(commonFolderId);
        const myNode = treeFolders.find((x) => x.id === myFolderId);
        const commonNode = treeFolders.find((x) => x.id === commonFolderId);

        runInAction(() => {
          myNode.folders = myFolders;
          commonNode.folders = commonFolders;
        });
      }
    );
  }, [
    setEnableThirdParty,
    enableThirdParty,
    getSubfolders,
    treeFolders,
    myFolderId,
    commonFolderId,
  ]);

  return (
    <StyledSettings>
      <ToggleButton
        className="toggle-btn"
        label={t("IntermediateVersion")}
        onChange={onChangeStoreForceSave}
        isChecked={storeForceSave}
      />
      <ToggleButton
        className="toggle-btn"
        label={t("ThirdPartyBtn")}
        onChange={onChangeThirdParty}
        isChecked={enableThirdParty}
      />
    </StyledSettings>
  );
};

export default inject(({ settingsStore, treeFoldersStore }) => {
  const {
    enableThirdParty,
    setEnableThirdParty,
    storeForcesave,
    setStoreForceSave,
  } = settingsStore;

  const {
    treeFolders,
    myFolderId,
    commonFolderId,
    getSubfolders,
  } = treeFoldersStore;

  return {
    storeForceSave: storeForcesave,
    setStoreForceSave,
    enableThirdParty,
    setEnableThirdParty,
    treeFolders,
    myFolderId,
    commonFolderId,
    getSubfolders,
  };
})(observer(AdminSettings));
