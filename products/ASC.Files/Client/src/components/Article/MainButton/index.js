import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from 'react-router';
import { MainButton, DropDownItem, toastr, utils } from "asc-web-components";
import { withTranslation, I18nextProvider } from "react-i18next";
import {
  setAction,
  fetchFiles,
  setTreeFolders,
} from "../../../store/files/actions";
import { isCanCreate, loopTreeFolders } from "../../../store/files/selectors";
import store from "../../../store/store";
import i18n from "../i18n";
import { utils as commonUtils, constants, api } from "asc-web-common";

const { changeLanguage } = commonUtils;
const { FileAction } = constants;

class PureArticleMainButtonContent extends React.Component {
  state = {
    files: [],
    uploadedFiles: 0,
    totalSize: 0,
    percent: 0
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

  updateFiles = () => {
    const { onLoading, filter, currentFolderId, treeFolders, setTreeFolders } = this.props;

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
      });
  }

  sendChunk = (files, location, requestsDataArray, isLatestFile, indexOfFile) => {
    const sendRequestFunc = (index) => {
      let newState = {};
      api.files
        .uploadFile(location, requestsDataArray[index])
        .then((res) => {
          let newPercent = this.state.percent;
          const percent = (newPercent +=
            (files[indexOfFile].size / this.state.totalSize) * 100);
          if (res.data.data && res.data.data.uploaded) {
            files[indexOfFile].uploaded = true;
            newState = { files, percent };
          }
          if (index + 1 !== requestsDataArray.length) {
            sendRequestFunc(index + 1);
          } else if (isLatestFile) {
            this.updateFiles();
            newState = Object.assign({}, newState, {
              uploadedFiles: this.state.uploadedFiles + 1,
            });
            return;
          } else {
            newState = Object.assign({}, newState, {
              uploadedFiles: this.state.uploadedFiles + 1,
            });
            this.startSessionFunc(indexOfFile + 1);
          }
        })
        .catch((err) => toastr.error(err))
        .finally(() => {
          if (newState.hasOwnProperty('files') || newState.hasOwnProperty('percent') || newState.hasOwnProperty('uploadedFiles')) {
            let progressVisible = true;
            let uploadedFiles = newState.uploadedFiles;
            let percent = newState.percent;
            if(newState.uploadedFiles === files.length) { 
              percent=100; 
              newState.percent = 0; 
              newState.uploadedFiles = 0; 
              progressVisible = false
            }
            this.setState(newState, () => {
              this.props.setProgressValue(percent);
              this.props.setProgressLabel(this.props.t("UploadingLabel", {file: uploadedFiles, totalFiles: files.length}));
              if(!progressVisible) {
                this.props.setProgressVisible(false);
              }
            });
          }
        });
    };

    sendRequestFunc(0);
  };

  startSessionFunc = indexOfFile => {
    const { files } = this.state;
    const { currentFolderId } = this.props;
    const file = files[indexOfFile];
    const isLatestFile = indexOfFile === files.length - 1;

    const fileName = file.name;
    const fileSize = file.size;
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
    const { t, setProgressVisible, setProgressLabel } = this.props;
    const files = e.target.files;
    //console.log("files", files);
    const newFiles = [];
    if(files) {
      let total = 0;
      for(let item of files) {
        if(item.size !== 0) {
          newFiles.push(item);
          total += item.size;
        } else {
          toastr.error(t("ErrorUploadMessage"));
        }
      }

      if(newFiles.length > 0) {
        this.setState({ files: newFiles, totalSize: total }, () => {
          setProgressVisible(true);
          setProgressLabel(this.props.t("UploadingLabel", {file: 0, totalFiles: newFiles.length}));
          this.startSessionFunc(0);
          //setProgressValue
          //setProgressContent
        });
      }
    }
  };

  shouldComponentUpdate(nextProps, nextState) {
    const { files, uploadedFiles, totalSize, percent } = this.state;
    if (nextProps.isCanCreate !== this.props.isCanCreate) {
      return true;
    }

    if (!utils.array.isArrayEqual(nextState.files, files)) {
      return true;
    }

    if (nextState.uploadedFiles !== uploadedFiles) {
      return true;
    }

    if (nextState.totalSize !== totalSize) {
      return true;
    }

    if (nextState.percent !== percent) {
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
