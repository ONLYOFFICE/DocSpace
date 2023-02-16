import React from "react";
import { inject, observer } from "mobx-react";

import ToggleButton from "@docspace/components/toggle-button";
import Heading from "@docspace/components/heading";
import Box from "@docspace/components/box";
import StyledSettings from "./StyledSettings";

const PersonalSettings = ({
  storeOriginalFiles,
  confirmDelete,
  updateIfExist,
  forceSave,

  isVisitor,
  //favoritesSection,
  //recentSection,

  setUpdateIfExist,
  setStoreOriginal,

  setConfirmDelete,

  setForceSave,

  setFavoritesSetting,
  setRecentSetting,

  t,
  showTitle,
  createWithoutDialog,
  setCreateWithoutDialog,

  showAdminSettings,
}) => {
  const [isLoadingFavorites, setIsLoadingFavorites] = React.useState(false);
  const [isLoadingRecent, setIsLoadingRecent] = React.useState(false);

  const onChangeOriginalCopy = React.useCallback(() => {
    setStoreOriginal(!storeOriginalFiles, "storeOriginalFiles");
  }, [setStoreOriginal, storeOriginalFiles]);

  const onChangeDeleteConfirm = React.useCallback(() => {
    setConfirmDelete(!confirmDelete, "confirmDelete");
  }, [setConfirmDelete, confirmDelete]);

  const onChangeUpdateIfExist = React.useCallback(() => {
    setUpdateIfExist(!updateIfExist, "updateIfExist");
  }, [setUpdateIfExist, updateIfExist]);

  const onChangeForceSave = React.useCallback(() => {
    setForceSave(!forceSave);
  }, [setForceSave, forceSave]);

  const onChangeFavorites = React.useCallback(
    (e) => {
      setIsLoadingFavorites(true);
      setFavoritesSetting(e.target.checked, "favoritesSection")
        .catch((err) => toastr.error(err))
        .finally(() => setIsLoadingFavorites(false));
    },
    [setIsLoadingFavorites, setFavoritesSetting]
  );

  const onChangeRecent = React.useCallback(
    (e) => {
      setIsLoadingRecent(true);
      setRecentSetting(e.target.checked, "recentSection")
        .catch((err) => toastr.error(err))
        .finally(() => setIsLoadingRecent(false));
    },
    [setIsLoadingRecent, setRecentSetting]
  );

  const onChangeCheckbox = () => {
    setCreateWithoutDialog(!createWithoutDialog);
  };

  return (
    <StyledSettings
      showTitle={showTitle}
      hideAdminSettings={!showAdminSettings}
    >
      <Box className="settings-section">
        {showTitle && (
          <Heading className="heading" level={2} size="xsmall">
            {t("Common:Common")}
          </Heading>
        )}
        <ToggleButton
          className="toggle-btn"
          label={t("Common:DontAskAgain")}
          onChange={onChangeCheckbox}
          isChecked={createWithoutDialog}
        />
        <ToggleButton
          className="toggle-btn"
          label={t("OriginalCopy")}
          onChange={onChangeOriginalCopy}
          isChecked={storeOriginalFiles}
        />
        {!isVisitor && (
          <ToggleButton
            className="toggle-btn"
            label={t("DisplayNotification")}
            onChange={onChangeDeleteConfirm}
            isChecked={confirmDelete}
          />
        )}
      </Box>

      {/* <Box className="settings-section">
        <Heading className="heading" level={2} size="xsmall">
          {t("AdditionalSections")}
        </Heading>
        <ToggleButton
          isDisabled={isLoadingRecent}
          className="toggle-btn"
          label={t("DisplayRecent")}
          onChange={onChangeRecent}
          isChecked={recentSection}
        />

        <ToggleButton
          isDisabled={isLoadingFavorites}
          className="toggle-btn"
          label={t("DisplayFavorites")}
          onChange={onChangeFavorites}
          isChecked={favoritesSection}
        />
        <ToggleButton
          isDisabled={true}
          className="toggle-btn"
          label={t("DisplayTemplates")}
          onChange={(e) => console.log(e)}
          isChecked={false}
        />
      </Box> */}

      <Box className="settings-section">
        <Heading className="heading" level={2} size="xsmall">
          {t("StoringFileVersion")}
        </Heading>
        {!isVisitor && (
          <ToggleButton
            className="toggle-btn"
            label={t("UpdateOrCreate")}
            onChange={onChangeUpdateIfExist}
            isChecked={updateIfExist}
          />
        )}
        <ToggleButton
          className="toggle-btn"
          label={t("KeepIntermediateVersion")}
          onChange={onChangeForceSave}
          isChecked={forceSave}
        />
      </Box>
    </StyledSettings>
  );
};

export default inject(
  ({ auth, settingsStore, treeFoldersStore, filesStore }) => {
    const {
      storeOriginalFiles,
      confirmDelete,
      updateIfExist,
      forcesave,

      setUpdateIfExist,
      setStoreOriginal,

      setConfirmDelete,

      setForceSave,

      favoritesSection,
      recentSection,
      setFavoritesSetting,
      setRecentSetting,
    } = settingsStore;

    const { myFolderId, commonFolderId } = treeFoldersStore;
    const { setCreateWithoutDialog, createWithoutDialog } = filesStore;

    return {
      storeOriginalFiles,
      confirmDelete,
      updateIfExist,
      forceSave: forcesave,

      myFolderId,
      commonFolderId,
      isVisitor: auth.userStore.user.isVisitor,
      favoritesSection,
      recentSection,

      setUpdateIfExist,
      setStoreOriginal,

      setConfirmDelete,

      setForceSave,

      setFavoritesSetting,
      setRecentSetting,
      myFolderId,
      commonFolderId,
      setCreateWithoutDialog,
      createWithoutDialog,
    };
  }
)(observer(PersonalSettings));
