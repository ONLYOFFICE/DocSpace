import CrossSidebarReactSvgUrl from "PUBLIC_DIR/images/cross.sidebar.react.svg?url";
import MobileActionsRemoveReactSvgUrl from "PUBLIC_DIR/images/mobile.actions.remove.react.svg?url";
import React from "react";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import { isMobileOnly } from "react-device-detect";

import { mobile } from "@docspace/components/utils/device";

import MainButtonMobile from "@docspace/components/main-button-mobile";

const StyledMainButtonMobile = styled(MainButtonMobile)`
  position: fixed;

  z-index: 200;

  right: 24px;
  bottom: 24px;

  @media ${mobile} {
    right: 16px;
    bottom: 16px;
  }

  ${isMobileOnly &&
  css`
    right: 16px;
    bottom: 16px;
  `}
`;

const MobileView = ({
  t,
  titleProp,
  actionOptions,
  buttonOptions,
  isRooms,
  files,
  clearUploadData,
  setUploadPanelVisible,
  primaryProgressDataVisible,
  primaryProgressDataPercent,
  primaryProgressDataLoadingFile,
  primaryProgressDataAlert,
  primaryProgressDataErrors,
  clearPrimaryProgressData,
  secondaryProgressDataStoreVisible,
  secondaryProgressDataStorePercent,
  secondaryProgressDataStoreCurrentFile,
  secondaryProgressDataStoreCurrentFilesCount,
  clearSecondaryProgressData,
  onMainButtonClick,
  isRoomsFolder,
  mainButtonMobileVisible,
}) => {
  const [isOpenButton, setIsOpenButton] = React.useState(false);
  const [percentProgress, setPercentProgress] = React.useState(0);
  const [progressOptions, setProgressOptions] = React.useState([]);

  const [primaryNumEl, setPrimaryNumEl] = React.useState(0);
  const primaryCurrentFile = React.useRef(null);

  const openButtonToggler = React.useCallback(() => {
    setIsOpenButton((prevState) => !prevState);
  }, []);

  const showUploadPanel = React.useCallback(() => {
    setUploadPanelVisible && setUploadPanelVisible(true);
  }, [setUploadPanelVisible]);

  const clearUploadPanel = React.useCallback(() => {
    clearUploadData && clearUploadData();
    clearPrimaryProgressData && clearPrimaryProgressData();
  }, [clearUploadData, clearPrimaryProgressData]);

  React.useEffect(() => {
    let currentPrimaryNumEl = primaryNumEl;

    const uploadedFileCount = files.filter(
      (item) => item.percent === 100 && !item.cancel
    ).length;
    const fileLength = files.filter((item) => !item.cancel).length;

    if (primaryCurrentFile.current === null && primaryProgressDataLoadingFile) {
      primaryCurrentFile.current = primaryProgressDataLoadingFile.uniqueId;
      currentPrimaryNumEl = 0;
    }

    if (primaryCurrentFile.current !== null && primaryProgressDataLoadingFile) {
      if (
        primaryCurrentFile.current !== primaryProgressDataLoadingFile.uniqueId
      ) {
        currentPrimaryNumEl++;
        primaryCurrentFile.current = primaryProgressDataLoadingFile.uniqueId;
      }
    }

    const currentSecondaryProgressItem =
      (secondaryProgressDataStoreCurrentFilesCount *
        secondaryProgressDataStorePercent) /
      100;

    const newProgressOptions = [
      {
        key: "primary-progress",
        open: primaryProgressDataVisible,
        label: t("UploadPanel:Uploads"),
        icon: CrossSidebarReactSvgUrl,
        percent: primaryProgressDataPercent,
        status:
          primaryProgressDataPercent === 100 && !primaryProgressDataErrors
            ? t("FilesUploaded")
            : `${uploadedFileCount}/${fileLength}`,
        onClick: showUploadPanel,
        onCancel: clearUploadPanel,
      },
      {
        key: "secondary-progress",
        open: secondaryProgressDataStoreVisible,
        label: t("Common:OtherOperations"),
        icon: MobileActionsRemoveReactSvgUrl,
        percent: secondaryProgressDataStorePercent,
        status: `${Math.round(
          currentSecondaryProgressItem
        )}/${secondaryProgressDataStoreCurrentFilesCount}`,
        onCancel: clearSecondaryProgressData,
      },
    ];

    let newPercentProgress =
      primaryProgressDataPercent + secondaryProgressDataStorePercent;

    if (primaryProgressDataVisible && secondaryProgressDataStoreVisible) {
      newPercentProgress =
        ((currentPrimaryNumEl + currentSecondaryProgressItem) /
          (files.length + secondaryProgressDataStoreCurrentFilesCount)) *
        100;
    }

    if (primaryProgressDataPercent === 100) {
      currentPrimaryNumEl = 0;
      primaryCurrentFile.current = null;
    }

    setPrimaryNumEl(currentPrimaryNumEl);
    setPercentProgress(newPercentProgress);
    setProgressOptions([...newProgressOptions]);
  }, [
    files.length,
    showUploadPanel,
    clearUploadPanel,
    primaryProgressDataVisible,
    primaryProgressDataPercent,
    primaryProgressDataLoadingFile,
    primaryProgressDataErrors,
    secondaryProgressDataStoreVisible,
    secondaryProgressDataStorePercent,
    secondaryProgressDataStoreCurrentFile,
    secondaryProgressDataStoreCurrentFilesCount,
  ]);

  return (
    <>
      {mainButtonMobileVisible && (
        <StyledMainButtonMobile
          actionOptions={actionOptions}
          isOpenButton={isOpenButton}
          onUploadClick={openButtonToggler}
          onClose={openButtonToggler}
          buttonOptions={buttonOptions}
          percent={percentProgress}
          progressOptions={progressOptions}
          title={titleProp}
          withoutButton={isRooms}
          alert={primaryProgressDataAlert}
          withMenu={!isRooms}
          onClick={onMainButtonClick}
          onAlertClick={showUploadPanel}
          withAlertClick={isRoomsFolder}
        />
      )}
    </>
  );
};

export default inject(({ uploadDataStore, treeFoldersStore }) => {
  const { isRoomsFolder } = treeFoldersStore;
  const {
    files,
    setUploadPanelVisible,
    secondaryProgressDataStore,
    primaryProgressDataStore,
    clearUploadData,
  } = uploadDataStore;

  const {
    visible: primaryProgressDataVisible,
    percent: primaryProgressDataPercent,
    loadingFile: primaryProgressDataLoadingFile,
    alert: primaryProgressDataAlert,
    errors: primaryProgressDataErrors,
    clearPrimaryProgressData,
  } = primaryProgressDataStore;

  const {
    visible: secondaryProgressDataStoreVisible,
    percent: secondaryProgressDataStorePercent,
    currentFile: secondaryProgressDataStoreCurrentFile,
    filesCount: secondaryProgressDataStoreCurrentFilesCount,
    clearSecondaryProgressData,
  } = secondaryProgressDataStore;

  return {
    files,
    clearUploadData,
    setUploadPanelVisible,
    primaryProgressDataVisible,
    primaryProgressDataPercent,
    primaryProgressDataLoadingFile,
    primaryProgressDataAlert,
    primaryProgressDataErrors,
    clearPrimaryProgressData,
    secondaryProgressDataStoreVisible,
    secondaryProgressDataStorePercent,
    secondaryProgressDataStoreCurrentFile,
    secondaryProgressDataStoreCurrentFilesCount,
    clearSecondaryProgressData,
    isRoomsFolder,
  };
})(observer(MobileView));
