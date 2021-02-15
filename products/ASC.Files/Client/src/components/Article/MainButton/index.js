import React, { useEffect } from "react";
//import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { MainButton, DropDownItem } from "asc-web-components";
import { withTranslation, I18nextProvider } from "react-i18next";
import { isMobile } from "react-device-detect";
//import { setAction, startUpload } from "../../../store/files/actions";
// import {
//   canCreate,
//   getFilter,
//   getSelectedFolder,
//   getFirstLoad,
//   getIsPrivacyFolder,
// } from "../../../store/files/selectors";
import {
  utils as commonUtils,
  constants,
  //store as initStore,
  Loaders,
} from "asc-web-common";
import { createI18N } from "../../../helpers/i18n";
import { encryptionUploadDialog } from "../../../helpers/desktop";
import { inject, observer } from "mobx-react";

//const { settingsStore } = initStore;

//const { getSettings } = initStore.auth.selectors;
const i18n = createI18N({
  page: "Article",
  localesPath: "Article",
});

const { changeLanguage } = commonUtils;
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

const ArticleMainButtonContentContainer = withTranslation()(
  PureArticleMainButtonContent
);

const ArticleMainButtonContent = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);
  return (
    <I18nextProvider i18n={i18n}>
      <ArticleMainButtonContentContainer {...props} />
    </I18nextProvider>
  );
};

ArticleMainButtonContent.propTypes = {
  history: PropTypes.object.isRequired,
};

// const mapStateToProps = (state) => {
//   return {
//     canCreate: canCreate(state),
//     firstLoad: getFirstLoad(state),
//     settings: getSettings(state),
//     filter: getFilter(state),
//     selectedFolder: getSelectedFolder(state),
//     isPrivacy: getIsPrivacyFolder(state),
//   };
// };

export default inject(({ store, mainFilesStore }) => {
  const { filesStore } = mainFilesStore;
  const {
    firstLoad,
    fileActionStore,
    treeFoldersStore,
    filter,
    canCreate,
    uploadDataStore,
  } = filesStore;
  const { isPrivacyFolder } = treeFoldersStore;
  const { id } = filesStore.selectedFolderStore;
  const { startUpload } = uploadDataStore;

  return {
    homepage: store.settingsStore.homepage,
    firstLoad,
    selectedFolderId: id,
    isPrivacy: isPrivacyFolder,
    filter,
    canCreate,

    setAction: fileActionStore.setAction,
    startUpload,
  };
})(withRouter(observer(ArticleMainButtonContent)));
