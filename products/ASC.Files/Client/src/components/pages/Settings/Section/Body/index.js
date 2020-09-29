import React, { useEffect } from "react";
import styled from "styled-components";
import { connect } from "react-redux";
import { Heading, ToggleButton } from "asc-web-components";
import { Error403, Error520 } from "asc-web-common";

import {
  setUpdateIfExist,
  setStoreOriginal,
  setEnableThirdParty,
  setConfirmDelete,
  setStoreForceSave,
  setSelectedNode,
  setForceSave,
} from "../../../../../store/files/actions";
import { setDocumentTitle } from "../../../../../helpers/utils";

const StyledSettings = styled.div`
  display: grid;
  grid-gap: 10px;

  .toggle-btn {
    position: relative;
  }

  .heading {
    margin-bottom: 0;
    margin-top: 22px;
  }
`;

const SectionBodyContent = ({
  setting,
  isLoading,
  selectedTreeNode,
  setSelectedNode,
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
  t,
}) => {
  useEffect(() => {
    const title = setting[0].toUpperCase() + setting.slice(1);
    setDocumentTitle(t(`${title}`));
  }, [setting, setDocumentTitle]);

  useEffect(() => {
    if (setting !== selectedTreeNode[0]) {
      setSelectedNode([setting]);
    }
  }, [setting]);

  const onChangeStoreForceSave = () => {
    setStoreForceSave(!storeForceSave, "storeForceSave");
  };

  const onChangeThirdParty = () => {
    setEnableThirdParty(!enableThirdParty, "enableThirdParty");
  };

  const renderAdminSettings = () => {
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
    return (
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

  const renderClouds = () => {
    return <></>;
  };

  let content;
  if (setting === "admin" && isAdmin) content = renderAdminSettings();
  if (setting === "common") content = renderCommonSettings();
  if (setting === "thirdParty" && enableThirdParty) content = renderClouds();

  return isLoading ? null : (!enableThirdParty && setting === "thirdParty") ||
    (!isAdmin && setting === "admin") ? (
    <Error403 />
  ) : isErrorSettings ? (
    <Error520 />
  ) : (
    content
  );
};

function mapStateToProps(state) {
  const { settingsTree, selectedTreeNode, isLoading } = state.files;
  const { isAdmin } = state.auth.user;
  const {
    storeOriginalFiles,
    confirmDelete,
    updateIfExist,
    forceSave,
    storeForceSave,
    enableThirdParty,
    isErrorSettings,
  } = settingsTree;

  return {
    isAdmin,
    selectedTreeNode,
    storeOriginalFiles,
    confirmDelete,
    updateIfExist,
    forceSave,
    storeForceSave,
    enableThirdParty,
    isLoading,
  };
}

export default connect(mapStateToProps, {
  setUpdateIfExist,
  setStoreOriginal,
  setEnableThirdParty,
  setConfirmDelete,
  setStoreForceSave,
  setSelectedNode,
  setForceSave,
})(SectionBodyContent);
