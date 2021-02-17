import React from "react";
import styled from "styled-components";
import { Heading, ToggleButton } from "asc-web-components";
import { Error403, Error520 } from "asc-web-common";
import ConnectClouds from "./ConnectedClouds";
import { inject, observer } from "mobx-react";

const StyledSettings = styled.div`
  display: grid;
  grid-gap: 12px;

  .toggle-btn {
    position: relative;
  }

  .heading {
    margin-bottom: 1px;
    margin-top: 26px;
  }

  .toggle-btn:first-child {
    margin-top: -3px;
  }
`;

const SectionBodyContent = ({
  setting,
  isLoading,
  storeForceSave,
  setStoreForceSave,
  enableThirdParty,
  setEnableThirdParty,
  storeOriginalFiles,
  setStoreOriginal,
  confirmDelete,
  setConfirmDelete,
  updateIfExist,
  setUpdateIfExist,
  forceSave,
  setForceSave,
  isAdmin,
  isErrorSettings,
  settingsTree,

  t,
}) => {
  const onChangeStoreForceSave = () => {
    setStoreForceSave(!storeForceSave, "storeForceSave");
  };

  const onChangeThirdParty = () => {
    setEnableThirdParty(!enableThirdParty, "enableThirdParty");
  };

  const renderAdminSettings = () => {
    return Object.keys(settingsTree).length === 0 || isLoading ? null : (
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

  const onChangeOriginalCopy = () => {
    setStoreOriginal(!storeOriginalFiles, "storeOriginalFiles");
  };

  const onChangeDeleteConfirm = () => {
    setConfirmDelete(!confirmDelete, "confirmDelete");
  };

  const onChangeUpdateIfExist = () => {
    setUpdateIfExist(!updateIfExist, "updateIfExist");
  };

  const onChangeForceSave = () => {
    setForceSave(!forceSave, "forceSave");
  };

  const renderCommonSettings = () => {
    return Object.keys(settingsTree).length === 0 || isLoading ? null : (
      <StyledSettings>
        <ToggleButton
          className="toggle-btn"
          label={t("OriginalCopy")}
          onChange={onChangeOriginalCopy}
          isChecked={storeOriginalFiles}
        />
        <ToggleButton
          className="toggle-btn"
          label={t("DisplayNotification")}
          onChange={onChangeDeleteConfirm}
          isChecked={confirmDelete}
        />
        <ToggleButton
          isDisabled={true}
          className="toggle-btn"
          label={t("DisplayRecent")}
          onChange={(e) => console.log(e)}
          isChecked={false}
        />
        <ToggleButton
          isDisabled={true}
          className="toggle-btn"
          label={t("DisplayFavorites")}
          onChange={(e) => console.log(e)}
          isChecked={false}
        />
        <ToggleButton
          isDisabled={true}
          className="toggle-btn"
          label={t("DisplayTemplates")}
          onChange={(e) => console.log(e)}
          isChecked={false}
        />
        <Heading className="heading" level={2} size="small">
          {t("StoringFileVersion")}
        </Heading>
        <ToggleButton
          className="toggle-btn"
          label={t("UpdateOrCreate")}
          onChange={onChangeUpdateIfExist}
          isChecked={updateIfExist}
        />
        <ToggleButton
          className="toggle-btn"
          label={t("KeepIntermediateVersion")}
          onChange={onChangeForceSave}
          isChecked={forceSave}
        />
      </StyledSettings>
    );
  };

  let content;

  if (setting === "admin" && isAdmin) content = renderAdminSettings();
  if (setting === "common") content = renderCommonSettings();
  if (setting === "thirdParty" && enableThirdParty) content = <ConnectClouds />;

  return isLoading ? null : (!enableThirdParty && setting === "thirdParty") ||
    (!isAdmin && setting === "admin") ? (
    <Error403 />
  ) : isErrorSettings ? (
    <Error520 />
  ) : (
    content
  );
};

export default inject(
  ({ auth, initFilesStore, settingsStore, treeFoldersStore }) => {
    const { isLoading } = initFilesStore;
    const { selectedTreeNode } = treeFoldersStore;
    const {
      settingsTree: settings,
      storeOriginalFiles,
      confirmDelete,
      updateIfExist,
      forcesave,
      storeForcesave,
      enableThirdParty,
      setUpdateIfExist,
      setStoreOriginal,
      setEnableThirdParty,
      setConfirmDelete,
      setStoreForceSave,
      setForceSave,
    } = settingsStore;

    const settingsTree = Object.keys(settings).length !== 0 ? settings : {};

    return {
      isAdmin: auth.isAdmin,
      isLoading,
      selectedTreeNode,
      settingsTree,
      storeOriginalFiles,
      confirmDelete,
      updateIfExist,
      forceSave: forcesave,
      storeForceSave: storeForcesave,
      enableThirdParty,

      setUpdateIfExist,
      setStoreOriginal,
      setEnableThirdParty,
      setConfirmDelete,
      setStoreForceSave,
      setForceSave,
    };
  }
)(observer(SectionBodyContent));
