import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import { MainButton, DropDownItem, toastr } from "asc-web-components";
import { withTranslation, I18nextProvider } from "react-i18next";
import {
  setAction,
  fetchFiles,
  setTreeFolders,
} from "../../../store/files/actions";
import { isCanCreate, loopTreeFolders } from "../../../store/files/selectors";
import store from "../../../store/store";
import i18n from "../i18n";
import { utils, constants, api } from "asc-web-common";

const { changeLanguage } = utils;
const { FileAction } = constants;

class PureArticleMainButtonContent extends React.Component {
  state = {
    uploadPercentage: 0,
  };

  onCreate = (format) => {
    this.props.setAction({
      type: FileAction.Create,
      extension: format,
      id: -1,
    });
  };

  onUploadFileClick = () => this.inputFilesElement.click();

  onUploadFolderClick = () => this.inputFolderElement.click();

  sendChunk = (
    files,
    location,
    requestsDataArray,
    isLatestFile,
    indexOfFile
  ) => {
    const {
      onLoading,
      filter,
      currentFolderId,
      treeFolders,
      setTreeFolders,
    } = this.props;

    const sendRequestFunc = (i) => {
      api.files.uploadFile(location, requestsDataArray[i]).then(() => {
        if (i + 1 !== requestsDataArray.length) {
          sendRequestFunc(i + 1);
        } else if (isLatestFile) {
          onLoading(true);
          const newFilter = filter.clone();
          fetchFiles(currentFolderId, newFilter, store.dispatch, treeFolders)
            .then((data) => {
              const path = data.selectedFolder.pathParts;
              const newTreeFolders = treeFolders;
              const folders = data.selectedFolder.folders;
              const foldersCount = data.selectedFolder.foldersCount;
              loopTreeFolders(path, newTreeFolders, folders, foldersCount);
              setTreeFolders(newTreeFolders);
            })
            .catch((err) => toastr.error(err))
            .finally(() => {
              onLoading(false);
              return;
            });
        } else {
          this.startSessionFunc(files, indexOfFile + 1);
        }
      });
    };

    sendRequestFunc(0);
  };

  startSessionFunc = (files, indexOfFile) => {
    const { currentFolderId } = this.props;
    const file = files[indexOfFile];
    const isLatestFile = indexOfFile === files.length - 1;

    const fileName = file.name;
    const fileSize = file.size;
    //if(fileSize === 0) {toastr.error("Size 0"); return}
    const relativePath = file.webkitRelativePath
      ? file.webkitRelativePath.slice(0, -file.name.length)
      : "";

    let location;
    const requestsDataArray = [];
    const chunkSize = 1024 * 1023; //~0.999mb
    const chunks = Math.ceil(file.size / chunkSize, chunkSize);
    let chunk = 0;

    api.files
      .startUploadSession(currentFolderId, fileName, fileSize, relativePath)
      .then((res) => {
        location = res.data.location;
        while (chunk < chunks) {
          const offset = chunk * chunkSize;
          //console.log("current chunk..", chunk);
          //console.log("file blob from offset...", offset);
          //console.log(file.slice(offset, offset + chunkSize));

          const formData = new FormData();
          formData.append("file", file.slice(offset, offset + chunkSize));
          requestsDataArray.push(formData);
          chunk++;
        }
      })
      .then(() =>
        this.sendChunk(
          files,
          location,
          requestsDataArray,
          isLatestFile,
          indexOfFile
        )
      );
  };

  onFileChange = (e) => {
    const files = e.target.files;
    //console.log("files", files);
    this.startSessionFunc(files, 0);
  };

  shouldComponentUpdate(nextProps, nextState) {
    if (nextProps.isCanCreate !== this.props.isCanCreate) {
      return true;
    }

    if (nextState.uploadPercentage !== this.state.uploadPercentage) {
      return true;
    }
    return false;
  }



  render() {
    //console.log("Files ArticleMainButtonContent render");
    const { t, isCanCreate } = this.props;

    return (
      <MainButton
        isDisabled={!isCanCreate}
        isDropdown={true}
        text={t('Actions')}
      >
        <DropDownItem
          icon="ActionsDocumentsIcon"
          label={t('NewDocument')}
          onClick={this.onCreate.bind(this, 'docx')}
        />
        <DropDownItem
          icon="SpreadsheetIcon"
          label={t('NewSpreadsheet')}
          onClick={this.onCreate.bind(this, 'xlsx')}
        />
        <DropDownItem
          icon="ActionsPresentationIcon"
          label={t('NewPresentation')}
          onClick={this.onCreate.bind(this, 'pptx')}
        />
        <DropDownItem
          icon="CatalogFolderIcon"
          label={t('NewFolder')}
          onClick={this.onCreate}
        />
        <DropDownItem isSeparator />
        <DropDownItem
          icon="ActionsUploadIcon"
          label={t("UploadFiles")}
          onClick={this.onUploadFileClick}
        />
        <DropDownItem
          icon="ActionsUploadIcon"
          label={t("UploadFolder")}
          onClick={this.onUploadFolderClick}
        />
        <input
          id="customFile"
          className="custom-file-input"
          multiple
          type="file"
          onChange={this.onFileChange}
          ref={(input) => (this.inputFilesElement = input)}
          style={{ display: "none" }}
        />
        <input
          id="customFile"
          className="custom-file-input"
          webkitdirectory=""
          mozdirectory=""
          type="file"
          onChange={this.onFileChange}
          ref={(input) => (this.inputFolderElement = input)}
          style={{ display: "none" }}
        />
      </MainButton>
    );
  }
}

const ArticleMainButtonContentContainer = withTranslation()(PureArticleMainButtonContent);

const ArticleMainButtonContent = (props) => {
  changeLanguage(i18n);
  return (<I18nextProvider i18n={i18n}><ArticleMainButtonContentContainer {...props} /></I18nextProvider>);
};

ArticleMainButtonContent.propTypes = {
  isAdmin: PropTypes.bool,
  history: PropTypes.object.isRequired
};

const mapStateToProps = (state) => {
  const { selectedFolder, filter, treeFolders } = state.files;
  const { settings, user } = state.auth;

  return {
    settings,
    isCanCreate: isCanCreate(selectedFolder, user),
    currentFolderId: selectedFolder.id,
    filter,
    treeFolders,
  };
};

export default connect(mapStateToProps, { setAction, setTreeFolders })(
  withRouter(ArticleMainButtonContent)
);
