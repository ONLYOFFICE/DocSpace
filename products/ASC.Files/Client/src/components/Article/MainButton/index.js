import React from "react";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { MainButton, DropDownItem } from "asc-web-components";
import { withTranslation } from "react-i18next";
import { isMobile } from "react-device-detect";
import { constants, Loaders } from "asc-web-common";
import { encryptionUploadDialog } from "../../../helpers/desktop";
import { inject, observer } from "mobx-react";

const { FileAction } = constants;

class PureArticleMainButtonContent extends React.Component {
  onCreate = (e) => {
    this.goToHomePage();
    const format = e.currentTarget.dataset.format || null;
    this.props.setAction({
      type: FileAction.Create,
      extension: format,
      id: -1,
    });
  };

  onUploadFileClick = () => {
    if (this.props.isPrivacy) {
      encryptionUploadDialog((encryptedFile, encrypted) => {
        const { selectedFolderId, startUpload, t } = this.props;
        encryptedFile.encrypted = encrypted;
        this.goToHomePage();
        startUpload([encryptedFile], selectedFolderId, t);
      });
    } else {
      this.inputFilesElement.click();
    }
  };

  onUploadFolderClick = () => this.inputFolderElement.click();

  goToHomePage = () => {
    const { homepage, history, filter } = this.props;
    const urlFilter = filter.toUrlParams();
    history.push(`${homepage}/filter?${urlFilter}`);
  };

  onFileChange = (e) => {
    const { selectedFolderId, startUpload, t } = this.props;
    this.goToHomePage();
    startUpload(e.target.files, selectedFolderId, t);
  };

  onInputClick = (e) => (e.target.value = null);

  // shouldComponentUpdate(nextProps, nextState) {
  //   return (
  //     nextProps.canCreate !== this.props.canCreate ||
  //     nextProps.firstLoad !== this.props.firstLoad ||
  //     nextProps.isPrivacy !== this.props.isPrivacy
  //   );
  // }

  render() {
    //console.log("Files ArticleMainButtonContent render");
    const { t, canCreate, isDisabled, firstLoad, isPrivacy } = this.props;

    return firstLoad ? (
      <Loaders.Rectangle />
    ) : (
      <MainButton
        isDisabled={isDisabled ? isDisabled : !canCreate}
        isDropdown={true}
        text={t("Actions")}
      >
        <DropDownItem
          className="main-button_drop-down"
          icon="ActionsDocumentsIcon"
          label={t("NewDocument")}
          onClick={this.onCreate}
          data-format="docx"
        />
        <DropDownItem
          className="main-button_drop-down"
          icon="SpreadsheetIcon"
          label={t("NewSpreadsheet")}
          onClick={this.onCreate}
          data-format="xlsx"
        />
        <DropDownItem
          className="main-button_drop-down"
          icon="ActionsPresentationIcon"
          label={t("NewPresentation")}
          onClick={this.onCreate}
          data-format="pptx"
        />
        <DropDownItem
          className="main-button_drop-down"
          icon="CatalogFolderIcon"
          label={t("NewFolder")}
          onClick={this.onCreate}
        />
        <DropDownItem isSeparator />
        <DropDownItem
          className="main-button_drop-down"
          icon="ActionsUploadIcon"
          label={t("UploadFiles")}
          onClick={this.onUploadFileClick}
        />
        {!isMobile && (
          <DropDownItem
            className="main-button_drop-down"
            icon="ActionsUploadIcon"
            label={t("UploadFolder")}
            disabled={isPrivacy}
            onClick={this.onUploadFolderClick}
          />
        )}
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
      </MainButton>
    );
  }
}

const ArticleMainButtonContent = withTranslation("Article")(
  PureArticleMainButtonContent
);

ArticleMainButtonContent.propTypes = {
  history: PropTypes.object.isRequired,
};

export default inject(
  ({
    auth,
    filesStore,
    uploadDataStore,
    treeFoldersStore,
    selectedFolderStore,
  }) => {
    const { firstLoad, fileActionStore, filter, canCreate } = filesStore;
    const { isPrivacyFolder } = treeFoldersStore;
    const { id } = selectedFolderStore;
    const { startUpload } = uploadDataStore;

    return {
      homepage: auth.settingsStore.homepage,
      firstLoad,
      selectedFolderId: id,
      isPrivacy: isPrivacyFolder,
      filter,
      canCreate,

      setAction: fileActionStore.setAction,
      startUpload,
    };
  }
)(withRouter(observer(ArticleMainButtonContent)));
