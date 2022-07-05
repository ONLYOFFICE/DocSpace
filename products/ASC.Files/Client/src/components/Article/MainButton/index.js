import React from "react";

import { inject, observer } from "mobx-react";

import MainButton from "@appserver/components/main-button";
import { withTranslation } from "react-i18next";
import { isMobile } from "react-device-detect";
import Loaders from "@appserver/common/components/Loaders";
import { AppServerConfig, FileAction } from "@appserver/common/constants";
import { encryptionUploadDialog } from "../../../helpers/desktop";
import { withRouter } from "react-router";

import MobileView from "./MobileView";
import { combineUrl } from "@appserver/common/utils";
import config from "../../../../package.json";
import withLoader from "../../../HOCs/withLoader";

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
    history,
    currentFolderId,
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

  const onShowGallery = () => {
    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/form-gallery/${currentFolderId}/`
      )
    );
  };

  React.useEffect(() => {
    const folderUpload = !isMobile
      ? [
          {
            id: "main-button_upload-folders",
            className: "main-button_drop-down",
            icon: "images/actions.upload.react.svg",
            label: t("UploadFolder"),
            disabled: isPrivacy,
            onClick: onUploadFolderClick,
            key: "upload-folder",
          },
        ]
      : [];

    const formActions = [
      {
        className: "main-button_drop-down",
        icon: "images/form.react.svg",
        label: t("Translations:NewForm"),
        key: "new-form",
        items: [
          {
            className: "main-button_drop-down_sub",
            icon: "images/form.react.svg",
            label: t("Translations:SubNewForm"),
            onClick: onCreate,
            action: "docxf",
            key: "docxf",
            withoutIcon: true,
          },
          {
            className: "main-button_drop-down_sub",
            icon: "images/form.file.react.svg",
            label: t("Translations:SubNewFormFile"),
            onClick: onShowSelectFileDialog,
            disabled: isPrivacy,
            key: "form-file",
            withoutIcon: true,
          },
          {
            className: "main-button_drop-down_sub",
            icon: "images/form.react.svg",
            label: t("Common:OFORMsGallery"),
            onClick: onShowGallery,
            disabled: isPrivacy,
            key: "form-gallery",
            withoutIcon: true,
          },
        ],
      },
    ];

    const actions = [
      {
        id: "main-button_new-document",
        className: "main-button_drop-down",
        icon: "images/actions.documents.react.svg",
        label: t("NewDocument"),
        onClick: onCreate,
        action: "docx",
        key: "docx",
      },
      {
        id: "main-button_new-spreadsheet",
        className: "main-button_drop-down",
        icon: "images/spreadsheet.react.svg",
        label: t("NewSpreadsheet"),
        onClick: onCreate,
        action: "xlsx",
        key: "xlsx",
      },
      {
        id: "main-button_new-presentation",
        className: "main-button_drop-down",
        icon: "images/actions.presentation.react.svg",
        label: t("NewPresentation"),
        onClick: onCreate,
        action: "pptx",
        key: "pptx",
      },
      ...formActions,
      {
        id: "main-button_new-folder",
        className: "main-button_drop-down",
        icon: "images/catalog.folder.react.svg",
        label: t("NewFolder"),
        onClick: onCreate,
        key: "new-folder",
      },
    ];

    const uploadActions = [
      {
        id: "main-button_upload-files",
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
    currentFolderId,
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
                t={t}
                titleProp={t("Upload")}
                actionOptions={actions}
                buttonOptions={uploadActions}
              />
            )}
        </>
      ) : (
        <MainButton
          id="files_main-button"
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
  ({
    auth,
    filesStore,
    dialogsStore,
    uploadDataStore,
    treeFoldersStore,
    selectedFolderStore,
  }) => {
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

    const currentFolderId = selectedFolderStore.id;

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
      currentFolderId,
    };
  }
)(
  withTranslation(["Article", "UploadPanel", "Common"])(
    withLoader(observer(withRouter(ArticleMainButtonContent)))(
      <Loaders.ArticleButton />
    )
  )
);
