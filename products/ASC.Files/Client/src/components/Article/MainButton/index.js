import React from "react";

import { inject, observer } from "mobx-react";

import MainButton from "@appserver/components/main-button";
import { withTranslation } from "react-i18next";
import { isMobile } from "react-device-detect";
import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@appserver/components/utils/device";
import Loaders from "@appserver/common/components/Loaders";
import { FileAction } from "@appserver/common/constants";
import { encryptionUploadDialog } from "../../../helpers/desktop";

import MobileView from "./MobileView";

const ArticleMainButtonContent = (props) => {
  const {
    t,
    isMobileArticle,
    showText,
    isDisabled,
    canCreate,
    isPrivacy,
    encryptedFile,
    encrypted,
    startUpload,
    setAction,
    setSelectFileDialogVisible,
    isArticleLoading,
    isFavoritesFolder,
    isShareFolder,
    isRecentFolder,
    isCommonFolder,
    isRecycleBinFolder,
  } = props;
  const inputFilesElement = React.useRef(null);
  const inputFolderElement = React.useRef(null);

  const [actions, setActions] = React.useState([]);
  const [uploadActions, setUploadActions] = React.useState([]);
  const [model, setModel] = React.useState([]);

  const onCreate = React.useCallback(
    (e) => {
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
        key: "separator",
      },
      ...uploadActions,
    ];

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
      {isMobileArticle ? (
        <>
          {!isFavoritesFolder &&
            !isRecentFolder &&
            !isCommonFolder &&
            !isShareFolder &&
            !isRecycleBinFolder &&
            !isArticleLoading &&
            canCreate && (
              <MobileView
                titleProp={t("Upload")}
                actionOptions={actions}
                buttonOptions={uploadActions}
              />
            )}
        </>
      ) : isArticleLoading ? (
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

export default inject(
  ({ auth, filesStore, dialogsStore, uploadDataStore, treeFoldersStore }) => {
    const {
      isLoaded,
      firstLoad,
      isLoading,
      fileActionStore,
      canCreate,
    } = filesStore;
    const {
      isPrivacyFolder,
      isFavoritesFolder,
      isRecentFolder,
      isCommonFolder,
      isRecycleBinFolder,
      isShareFolder,
    } = treeFoldersStore;
    const { startUpload } = uploadDataStore;
    const { setSelectFileDialogVisible } = dialogsStore;

    const isArticleLoading = (!isLoaded || isLoading) && firstLoad;

    return {
      showText: auth.settingsStore.showText,
      isMobileArticle: auth.settingsStore.isMobileArticle,

      isArticleLoading,
      isPrivacy: isPrivacyFolder,
      isFavoritesFolder,
      isRecentFolder,
      isCommonFolder,
      isRecycleBinFolder,
      isShareFolder,
      canCreate,

      setAction: fileActionStore.setAction,
      startUpload,

      setSelectFileDialogVisible,

      isLoading,
      isLoaded,
      firstLoad,
    };
  }
)(withTranslation(["Article", "Common"])(observer(ArticleMainButtonContent)));
