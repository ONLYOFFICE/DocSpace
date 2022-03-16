import React from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import MainButton from "@appserver/components/main-button";
import { withTranslation } from "react-i18next";
import { isMobile } from "react-device-detect";
import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@appserver/components/utils/device";
import Loaders from "@appserver/common/components/Loaders";
import { FileAction, AppServerConfig } from "@appserver/common/constants";
import { encryptionUploadDialog } from "../../../helpers/desktop";
import { inject, observer } from "mobx-react";
import config from "../../../../package.json";
import { combineUrl } from "@appserver/common/utils";

import MobileView from "./MobileView";

const ArticleMainButtonContent = ({
  t,
  isDisabled,
  canCreate,
  isPrivacy,
  encryptionUploadDialog,
  encryptedFile,
  encrypted,
  startUpload,
  setAction,
  setSelectFileDialogVisible,
  homepage,
  history,
  filter,
  sectionWidth,
  isArticleLoaded,
}) => {
  const inputFilesElement = React.useRef(null);
  const inputFolderElement = React.useRef(null);

  const [actions, setActions] = React.useState([]);
  const [uploadActions, setUploadActions] = React.useState([]);
  const [model, setModel] = React.useState([]);

  const onCreate = React.useCallback(
    (e) => {
      // this.goToHomePage();

      const format = e.action || null;
      setAction({
        type: FileAction.Create,
        extension: format,
        id: -1,
      });
    },
    [setAction]
  );

  const onShowSelectFileDialog = React.useCallback(() => {
    setSelectFileDialogVisible(true);
  }, [setSelectFileDialogVisible]);

  const goToHomePage = React.useCallback(() => {
    const urlFilter = filter.toUrlParams();
    history.push(
      combineUrl(AppServerConfig.proxyURL, homepage, `/filter?${urlFilter}`)
    );
  }, [homepage, history, filter]);

  const onFileChange = React.useCallback(
    (e) => {
      startUpload(e.target.files, null, t);
    },
    [startUpload, t]
  );

  const onUploadFileClick = React.useCallback(() => {
    if (isPrivacy) {
      encryptionUploadDialog((encryptedFile, encrypted) => {
        encryptedFile.encrypted = encrypted;
        goToHomePage();
        startUpload([encryptedFile], null, t);
      });
    } else {
      inputFilesElement.current.click();
    }
  }, [
    isPrivacy,
    encrypted,
    encryptedFile,
    encryptionUploadDialog,
    goToHomePage,
    startUpload,
  ]);

  const onUploadFolderClick = React.useCallback(() => {
    inputFolderElement.current.click();
  }, []);

  const onInputClick = React.useCallback((e) => (e.target.value = null), []);

  React.useEffect(() => {
    const folderUpload = !isMobile
      ? [
          {
            className: "main-button_drop-down",
            icon: "images/actions.upload.react.svg",
            label: t("UploadFolder"),
            disabled: isPrivacy,
            onClick: onUploadFolderClick,
            key: "upload-folder",
          },
        ]
      : [];

    const formActions =
      !isMobile && !isTabletUtils()
        ? [
            {
              className: "main-button_drop-down",
              icon: "images/form.react.svg",
              label: t("Translations:NewForm"),
              key: "new-form",
              items: [
                {
                  className: "main-button_drop-down_sub",
                  label: t("Translations:SubNewForm"),
                  onClick: onCreate,
                  action: "docxf",
                  key: "docxf",
                },
                {
                  className: "main-button_drop-down_sub",
                  label: t("Translations:SubNewFormFile"),
                  onClick: onShowSelectFileDialog,
                  disabled: isPrivacy,
                  key: "form-file",
                },
              ],
            },
          ]
        : [
            {
              className: "main-button_drop-down_sub",
              icon: "images/form.react.svg",
              label: t("Translations:NewForm"),
              onClick: onCreate,
              action: "docxf",
              key: "docxf",
            },
            {
              className: "main-button_drop-down_sub",
              icon: "images/form.file.react.svg",
              label: t("Translations:NewFormFile"),
              onClick: onShowSelectFileDialog,
              disabled: isPrivacy,
              key: "form-file",
            },
          ];

    const actions = [
      {
        className: "main-button_drop-down",
        icon: "images/actions.documents.react.svg",
        label: t("NewDocument"),
        onClick: onCreate,
        action: "docx",
        key: "docx",
      },
      {
        className: "main-button_drop-down",
        icon: "images/spreadsheet.react.svg",
        label: t("NewSpreadsheet"),
        onClick: onCreate,
        action: "xlsx",
        key: "xlsx",
      },
      {
        className: "main-button_drop-down",
        icon: "images/actions.presentation.react.svg",
        label: t("NewPresentation"),
        onClick: onCreate,
        action: "pptx",
        key: "pptx",
      },
      ...formActions,
      {
        className: "main-button_drop-down",
        icon: "images/catalog.folder.react.svg",
        label: t("NewFolder"),
        onClick: onCreate,
        key: "new-folder",
      },
    ];

    const uploadActions = [
      {
        className: "main-button_drop-down",
        icon: "images/actions.upload.react.svg",
        label: t("UploadFiles"),
        onClick: onUploadFileClick,
        key: "upload-files",
      },
      ...folderUpload,
    ];

    const menuModel = [
      ...actions,
      {
        isSeparator: true,
      },
      ...uploadActions,
    ];
    console.log("upd");
    setModel(menuModel);
    setActions(actions);
    setUploadActions(uploadActions);
  }, [
    t,
    isPrivacy,
    onCreate,
    onShowSelectFileDialog,
    onUploadFileClick,
    onUploadFolderClick,
  ]);

  return (
    <>
      {isMobile || isMobileUtils() || isTabletUtils() ? (
        <>
          {!isFavoritesFolder &&
            !isRecentFolder &&
            !isCommonFolder &&
            !isRecycleBinFolder && (
              <MobileView
                titleProp={t("Upload")}
                actionOptions={actions}
                buttonOptions={uploadActions}
                sectionWidth={sectionWidth}
              />
            )}
        </>
      ) : !isArticleLoaded ? (
        <Loaders.ArticleButton />
      ) : (
        <MainButton
          isDisabled={isDisabled ? isDisabled : !canCreate}
          isDropdown={true}
          text={t("Common:Actions")}
          model={model}
        />
      )}

      <input
        id="customFileInput"
        className="custom-file-input"
        multiple
        type="file"
        onChange={onFileChange}
        onClick={onInputClick}
        ref={inputFilesElement}
        style={{ display: "none" }}
      />
      <input
        id="customFolderInput"
        className="custom-file-input"
        webkitdirectory=""
        mozdirectory=""
        type="file"
        onChange={onFileChange}
        onClick={onInputClick}
        ref={inputFolderElement}
        style={{ display: "none" }}
      />
    </>
  );
};

ArticleMainButtonContent.propTypes = {
  history: PropTypes.object.isRequired,
};

export default inject(
  ({ auth, filesStore, dialogsStore, uploadDataStore, treeFoldersStore }) => {
    const { isArticleLoaded, fileActionStore, filter, canCreate } = filesStore;
    const {
      isPrivacyFolder,
      isFavoritesFolder,
      isRecentFolder,
      isCommonFolder,
      isRecycleBinFolder,
    } = treeFoldersStore;
    const { startUpload } = uploadDataStore;
    const { setSelectFileDialogVisible } = dialogsStore;

    return {
      homepage: config.homepage,
      isArticleLoaded,
      isPrivacy: isPrivacyFolder,
      isFavoritesFolder,
      isRecentFolder,
      isCommonFolder,
      isRecycleBinFolder,
      filter,
      canCreate,

      setAction: fileActionStore.setAction,
      startUpload,

      setSelectFileDialogVisible,
    };
  }
)(
  withRouter(
    withTranslation(["Article", "Common"])(observer(ArticleMainButtonContent))
  )
);
