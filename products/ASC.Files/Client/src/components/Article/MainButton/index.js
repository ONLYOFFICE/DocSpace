import React from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import MainButton from "@appserver/components/main-button";
import { withTranslation } from "react-i18next";
import { isMobile } from "react-device-detect";
import Loaders from "@appserver/common/components/Loaders";
import { FileAction, AppServerConfig } from "@appserver/common/constants";
import { encryptionUploadDialog } from "../../../helpers/desktop";
import { inject, observer } from "mobx-react";
import config from "../../../../package.json";
import { combineUrl } from "@appserver/common/utils";
import withLoader from "../../../HOCs/withLoader";

class ArticleMainButtonContent extends React.Component {
  onCreate = (e) => {
    // this.goToHomePage();
    const format = e.action || null;
    this.props.setAction({
      type: FileAction.Create,
      extension: format,
      id: -1,
    });
  };

  onShowSelectFileDialog = () => {
    const { setSelectFileDialogVisible, hideArticle } = this.props;
    hideArticle();
    setSelectFileDialogVisible(true);
  };

  onUploadFileClick = () => {
    if (this.props.isPrivacy) {
      encryptionUploadDialog((encryptedFile, encrypted) => {
        const { startUpload, t } = this.props;
        encryptedFile.encrypted = encrypted;
        this.goToHomePage();
        startUpload([encryptedFile], null, t);
      });
    } else {
      this.inputFilesElement.click();
    }
  };

  onUploadFolderClick = () => this.inputFolderElement.click();

  goToHomePage = () => {
    const { homepage, history, filter } = this.props;
    const urlFilter = filter.toUrlParams();
    history.push(
      combineUrl(AppServerConfig.proxyURL, homepage, `/filter?${urlFilter}`)
    );
  };

  onFileChange = (e) => {
    const { startUpload, t } = this.props;
    //this.goToHomePage();
    startUpload(e.target.files, null, t);
  };

  onInputClick = (e) => (e.target.value = null);

  render() {
    //console.log("Files ArticleMainButtonContent render");
    const { t, canCreate, isDisabled, isPrivacy } = this.props;

    const folderUpload = !isMobile
      ? [
          {
            className: "main-button_drop-down main-button_upload-folders",
            icon: "images/actions.upload.react.svg",
            label: t("UploadFolder"),
            disabled: isPrivacy,
            onClick: this.onUploadFolderClick,
          },
        ]
      : [];

    const formActions = !isMobile
      ? [
          {
            className: "main-button_drop-down main-button_new-form",
            icon: "images/form.react.svg",
            label: t("Translations:NewForm"),
            items: [
              {
                className:
                  "main-button_drop-down_sub main-button_new-form-from-blank",
                label: t("Translations:SubNewForm"),
                onClick: this.onCreate,
                action: "docxf",
              },
              {
                className:
                  "main-button_drop-down_sub main-button_new-form-from-file",
                label: t("Translations:SubNewFormFile"),
                onClick: this.onShowSelectFileDialog,
                disabled: isPrivacy,
              },
            ],
          },
        ]
      : [
          {
            className: "main-button_drop-down_sub main-button_new-form",
            icon: "images/form.react.svg",
            label: t("Translations:NewForm"),
            onClick: this.onCreate,
            action: "docxf",
          },
          {
            className:
              "main-button_drop-down_sub main-button_new-form-from-file",
            icon: "images/form.file.react.svg",
            label: t("Translations:NewFormFile"),
            onClick: this.onShowSelectFileDialog,
            disabled: isPrivacy,
          },
        ];

    const menuModel = [
      {
        className: "main-button_drop-down main-button_new-document",
        icon: "images/actions.documents.react.svg",
        label: t("NewDocument"),
        onClick: this.onCreate,
        action: "docx",
      },
      {
        className: "main-button_drop-down main-button_new-spreadsheet",
        icon: "images/spreadsheet.react.svg",
        label: t("NewSpreadsheet"),
        onClick: this.onCreate,
        action: "xlsx",
      },
      {
        className: "main-button_drop-down main-button_new-presentation",
        icon: "images/actions.presentation.react.svg",
        label: t("NewPresentation"),
        onClick: this.onCreate,
        action: "pptx",
      },
      ...formActions,
      {
        className: "main-button_drop-down main-button_new-folder",
        icon: "images/catalog.folder.react.svg",
        label: t("NewFolder"),
        onClick: this.onCreate,
      },
      {
        isSeparator: true,
      },
      {
        className: "main-button_drop-down main-button_upload-files",
        icon: "images/actions.upload.react.svg",
        label: t("UploadFiles"),
        onClick: this.onUploadFileClick,
      },
      ...folderUpload,
    ];

    return (
      <>
        <MainButton
          isDisabled={isDisabled ? isDisabled : !canCreate}
          isDropdown={true}
          text={t("Common:Actions")}
          model={menuModel}
          className="files-main-button"
        />
        <input
          id="customFileInput"
          className="custom-file-input"
          multiple
          type="file"
          onChange={this.onFileChange}
          onClick={this.onInputClick}
          ref={(input) => (this.inputFilesElement = input)}
          style={{ display: "none" }}
        />
        <input
          id="customFolderInput"
          className="custom-file-input"
          webkitdirectory=""
          mozdirectory=""
          type="file"
          onChange={this.onFileChange}
          onClick={this.onInputClick}
          ref={(input) => (this.inputFolderElement = input)}
          style={{ display: "none" }}
        />
      </>
    );
  }
}

ArticleMainButtonContent.propTypes = {
  history: PropTypes.object.isRequired,
};

export default inject(
  ({ auth, filesStore, uploadDataStore, treeFoldersStore, dialogsStore }) => {
    const { fileActionStore, filter, canCreate } = filesStore;
    const { isPrivacyFolder } = treeFoldersStore;
    const { startUpload } = uploadDataStore;
    const { setSelectFileDialogVisible } = dialogsStore;
    const { hideArticle } = auth.settingsStore;
    return {
      homepage: config.homepage,
      isPrivacy: isPrivacyFolder,
      filter,
      canCreate,

      setAction: fileActionStore.setAction,
      startUpload,
      setSelectFileDialogVisible,
      hideArticle,
    };
  }
)(
  withRouter(
    withTranslation(["Article", "Common", "Translations"])(
      withLoader(observer(ArticleMainButtonContent))(<Loaders.MainButton />)
    )
  )
);
