import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { RequestLoader, Checkbox, toastr } from "asc-web-components";
import { PageLayout, utils, api } from "asc-web-common";
import { withTranslation, I18nextProvider } from 'react-i18next';
import i18n from "./i18n";

import {
  ArticleHeaderContent,
  ArticleBodyContent,
  ArticleMainButtonContent
} from "../../Article";
import {
  SectionHeaderContent,
  SectionBodyContent,
  SectionFilterContent,
  SectionPagingContent
} from "./Section";
import { setSelected, fetchFiles, setTreeFolders, getProgress, getFolder, setFilter, selectFile, deselectFile, setDragging } from "../../../store/files/actions";
import { loopTreeFolders, checkFolderType, canConvert } from "../../../store/files/selectors";
import store from "../../../store/store";
import { ConvertDialog } from "../../dialogs";

const { changeLanguage } = utils;

class PureHome extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isHeaderVisible: false,
      isHeaderIndeterminate: false,
      isHeaderChecked: false,
      isLoading: false,

      showProgressBar: false,
      progressBarValue: 0,
      progressBarLabel: "",
      overwriteSetting: false,
      uploadOriginalFormatSetting: false,
      hideWindowSetting: false,

      files: [],
      uploadedFiles: 0,
      totalSize: 0,
      percent: 0,

      showConvertDialog: false,
      convertFiles: [],
      convertFilesSize: 0,
      uploadStatus: null,
      uploaded: true,
      uploadToFolder: null
    };
  }

  renderGroupButtonMenu = () => {
    const { files, selection, selected, setSelected, folders } = this.props;

    const headerVisible = selection.length > 0;
    const headerIndeterminate =
      headerVisible &&
      selection.length > 0 &&
      selection.length < files.length + folders.length;
    const headerChecked =
      headerVisible && selection.length === files.length + folders.length;

    /*console.log(`renderGroupButtonMenu()
      headerVisible=${headerVisible} 
      headerIndeterminate=${headerIndeterminate} 
      headerChecked=${headerChecked}
      selection.length=${selection.length}
      files.length=${files.length}
      selected=${selected}`);*/

    let newState = {};

    if (headerVisible || selected === "close") {
      newState.isHeaderVisible = headerVisible;
      if (selected === "close") {
        setSelected("none");
      }
    }

    newState.isHeaderIndeterminate = headerIndeterminate;
    newState.isHeaderChecked = headerChecked;

    this.setState(newState);
  };

  updateFiles = (folderId) => {
    const { filter, treeFolders, setTreeFolders, currentFolderId } = this.props;

    this.onLoading(true);
    const newFilter = filter.clone();
    if (currentFolderId === folderId) {
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
        .finally(() => this.setState({ uploaded: true, isLoading: false }));
    } else {
      api.files
        .getFolder(folderId, newFilter)
        .then(data => {
          const path = data.pathParts;
          const newTreeFolders = treeFolders;
          const folders = data.folders;
          const foldersCount = data.count;
          loopTreeFolders(path, newTreeFolders, folders, foldersCount);
          setTreeFolders(newTreeFolders);
        })
        .catch(err => toastr.error(err))
        .finally(() => this.setState({ uploaded: true, isLoading: false }));
    }
  };

  updateConvertProgress = (newState, uploadStatus) => {
    let progressVisible = true;
    let uploadedFiles = newState.uploadedFiles;
    let percent = newState.percent;

    const totalFiles =
      uploadStatus === "cancel"
        ? this.state.files.length
        : this.state.files.length + this.state.convertFiles.length;

    if(newState.uploadedFiles === totalFiles) {
      percent = 100;
      newState.percent = 0;
      newState.uploadedFiles = 0;
      progressVisible = false;
    }
    newState.progressBarValue = percent;
    newState.progressBarLabel = this.props.t("UploadingLabel", {
      file: uploadedFiles,
      totalFiles,
    });

    this.setState(newState, () => {
      if(!progressVisible) {
        this.setProgressVisible(false);
      }
    });
  };

  getConvertProgress = (fileId, newState, isLatestFile, indexOfFile) => {
    const { uploadedFiles, uploadStatus, uploadToFolder } = this.state;
    api.files.getConvertFile(fileId).then(res => {
      if(res && res[0] && res[0].progress !== 100) {
        setTimeout(() => this.getConvertProgress(fileId, newState, isLatestFile, indexOfFile), 1000);
      } else {
        newState = {...newState, ...{ uploadedFiles: uploadedFiles + 1 }};
        this.updateConvertProgress(newState, uploadStatus);
        !isLatestFile && this.startSessionFunc(indexOfFile + 1);

        if(res[0].error) {
          toastr.error(res[0].error);
        }
        if(isLatestFile) {
          this.updateFiles(uploadToFolder);
          return;
        }
      }
    });
  }

  sendChunk = (files, location, requestsDataArray, isLatestFile, indexOfFile) => {
    const { uploadToFolder, totalSize, uploaded, uploadStatus, uploadedFiles } = this.state;
    const sendRequestFunc = (index) => {
      let newState = {};
      api.files
        .uploadFile(location, requestsDataArray[index])
        .then((res) => {
          let newPercent = this.state.percent;
          const currentFile = files[indexOfFile];
          const fileId = res.data.data.id;
          const percent = (newPercent += (currentFile.size / totalSize) * 100);

          if (res.data.data && res.data.data.uploaded) {
            newState = { percent };
          }

          if (index + 1 !== requestsDataArray.length) {
            sendRequestFunc(index + 1);
          } else if(uploaded) {
            api.files.convertFile(fileId).then(convertRes => {
              if(convertRes && convertRes[0] && convertRes[0].progress !== 100) {
                this.getConvertProgress(fileId, newState, isLatestFile, indexOfFile);
              }
            });
          } else if (isLatestFile) {
            if(uploaded) {
              this.updateFiles(uploadToFolder);
            } else {
              if(uploadStatus === "convert") {
                newState = { ...newState, ...{ uploadedFiles: uploadedFiles + 1, uploadStatus: null, uploaded: true }};
                this.updateConvertProgress(newState, uploadStatus);
                this.startSessionFunc(0);
              } else if (uploadStatus === "pending") {
                newState = {...newState, ...{ uploadedFiles: uploadedFiles + 1, uploaded: true, uploadStatus: null }};
                this.updateConvertProgress(newState, uploadStatus);
              } else {
                newState = {...newState, ...{ uploadedFiles: uploadedFiles + 1, uploadStatus: null }};
                this.updateConvertProgress(newState, uploadStatus);
                this.updateFiles(uploadToFolder);
              }
            }
          } else {
            newState = {...newState, ...{ uploadedFiles: uploadedFiles + 1 }};
            this.updateConvertProgress(newState, uploadStatus);
            this.startSessionFunc(indexOfFile + 1);
          }
        })
        .catch((err) => toastr.error(err))
    };

    sendRequestFunc(0);
  };

  startSessionFunc = (indexOfFile) => {
    const { files, convertFiles, uploaded, uploadToFolder } = this.state;

    const currentFiles = uploaded ? convertFiles : files;

    if(!uploaded && files.length === 0) {
      this.setState({ uploaded: true });
      return;
    }

    let file = files[indexOfFile];
    let isLatestFile = indexOfFile === files.length - 1;

    if(uploaded) {
      if(convertFiles.length) {
        file = convertFiles[indexOfFile];
        isLatestFile = indexOfFile === convertFiles.length - 1;
      } else {
        //Test return empty convert files
        return;
      }
    }

    const fileName = file.name;
    const fileSize = file.size;
    const relativePath = file.relativePath
      ? file.relativePath.slice(1, -file.name.length)
      : file.webkitRelativePath
      ? file.webkitRelativePath.slice(0, -file.name.length)
      : "";

    let location;
    const requestsDataArray = [];
    const chunkSize = 1024 * 1023; //~0.999mb
    const chunks = Math.ceil(file.size / chunkSize, chunkSize);
    let chunk = 0;

    api.files
      .startUploadSession(uploadToFolder, fileName, fileSize, relativePath)
      .then((res) => {
        location = res.data.location;
        while (chunk < chunks) {
          const offset = chunk * chunkSize;
          const formData = new FormData();
          formData.append("file", file.slice(offset, offset + chunkSize));
          requestsDataArray.push(formData);
          chunk++;
        }
      })
      .then(
        () => this.sendChunk(currentFiles, location, requestsDataArray, isLatestFile, indexOfFile, uploadToFolder)
      )
      .catch((err) => {
        this.setProgressVisible(false, 0);
        toastr.error(err);
      });
  };

  onDrop = (e, folderId) => {
    const items = e.dataTransfer.items;
    let files = [];

    const inSeries = (queue, callback) => {
      let i = 0;
      let length = queue.length;

      if (!queue || !queue.length) {
        callback();
      }

      const callNext = (i) => {
        if (typeof queue[i] === "function") {
          queue[i](() => i+1 < length ? callNext(i+1) : callback());
        }
      };
      callNext(i);
    };

    const readDirEntry = (dirEntry, callback) => {
      let entries = [];
      const dirReader = dirEntry.createReader();

      // keep quering recursively till no more entries
      const getEntries = (func) => {
        dirReader.readEntries((moreEntries) => {
          if (moreEntries.length) {
            entries = [...entries, ...moreEntries];
            getEntries(func);
          } else {
            func();
          }
        });
      };

      getEntries(() => readEntries(entries, callback));
    };

    const readEntry = (entry, callback) => {
      if (entry.isFile) {
        entry.file(file => {
          addFile(file, entry.fullPath);
          callback();
        });
      } else if (entry.isDirectory) {
        readDirEntry(entry, callback);
      }
    };

    const readEntries = (entries, callback) => {
      const queue = [];
      loop(entries, (entry) => {
        queue.push((func) => readEntry(entry, func));
      });
      inSeries(queue, () => callback());
    };

    const addFile = (file, relativePath) => {
      file.relativePath = relativePath || "";
      files.push(file);
    };

    const loop = (items, callback) => {
      let length;

      if (items) {
        length = items.length;
        // Loop array items
        for (let i = 0; i < length; i++) {
          callback(items[i], i);
        }
      }
    };

    const readItems = (items, func) => {
      const entries = [];
      loop(items, (item) => {
        const entry = item.webkitGetAsEntry();
        if (entry) {
          if (entry.isFile) {
            addFile(item.getAsFile(), entry.fullPath);
          } else {
            entries.push(entry);
          }
        }
      });

      if (entries.length) {
        readEntries(entries, func);
      } else {
        func();
      }
    };

    this.setState({ isLoading: true }, () => {
      this.props.setDragging(false);
      readItems(items, () => this.startUpload(files, folderId));
    });
  };

  startUpload = (files, folderId) => {
    const newFiles = [];
    let filesSize = 0;
    const convertFiles = [];
    let convertFilesSize = 0;

    for (let item of files) {
      if (item.size !== 0) {
        const parts = item.name.split(".");
        const ext = parts.length > 1 ? "." + parts.pop() : "";
        if(canConvert(ext)) {
          convertFiles.push(item);
          convertFilesSize += item.size;
        } else {
          newFiles.push(item);
          filesSize += item.size;
        }
      } else {
        toastr.error(this.props.t("ErrorUploadMessage"));
      }
    }

    const showConvertDialog = !!convertFiles.length;
    const uploadStatus = convertFiles.length ? "pending" : null;
    const uploadToFolder = folderId ? folderId : this.props.currentFolderId;

    const newState = {
      files: newFiles,
      filesSize,
      convertFiles,
      convertFilesSize,
      uploadToFolder,
      showConvertDialog,
      uploaded: false,
      uploadStatus
    };

    this.startUploadFiles(newState);
  };

  startUploadFiles = (state) => {
    const { files, filesSize, convertFiles, convertFilesSize } = state;

    if (files.length > 0 || convertFiles.length > 0) {
      const progressBarLabel = this.props.t("UploadingLabel", {
        file: 0,
        totalFiles: files.length + convertFiles.length
      });

      const totalSize = convertFilesSize + filesSize;
      const newState = {...state, ...{ totalSize, progressBarLabel, showProgressBar: true, isLoading: true }};
      this.setState(newState, () => this.startSessionFunc(0));
    } else if (this.state.isLoading) {
      this.setState({ isLoading: false });
    }
  };

  onSectionHeaderContentCheck = (checked) => {
    this.props.setSelected(checked ? "all" : "none");
  };

  onSectionHeaderContentSelect = (selected) => {
    this.props.setSelected(selected);
  };

  onClose = () => {
    const { selection, setSelected } = this.props;

    if (!selection.length) {
      setSelected("none");
      this.setState({ isHeaderVisible: false });
    } else {
      setSelected("close");
    }
  };

  onLoading = (status) => {
    this.setState({ isLoading: status });
  };

  setProgressVisible = (visible, timeout) => {
    const newTimeout = timeout ? timeout : 5000;
    if (visible) {
      this.setState({
        showProgressBar: visible,
        progressBarValue: 0,
        percent: 0
      });
    } else {
      setTimeout(
        () => this.setState({ showProgressBar: visible, progressBarValue: 0 }),
        newTimeout
      );
    }
  };
  setProgressValue = (value) => this.setState({ progressBarValue: value });
  setProgressLabel = (label) => this.setState({ progressBarLabel: label });

  onChangeOverwrite = () =>
    this.setState({ overwriteSetting: !this.state.overwriteSetting });
  onChangeOriginalFormat = () =>
    this.setState({
      uploadOriginalFormatSetting: !this.state.uploadOriginalFormatSetting,
    });
  onChangeWindowVisible = () =>
    this.setState({ hideWindowSetting: !this.state.hideWindowSetting });

  startFilesOperations = (progressBarLabel) => {
    this.setState({ isLoading: true, progressBarLabel, showProgressBar: true });
  };

  finishFilesOperations = (err) => {
    const timeout = err ? 0 : null;
    err && toastr.error(err);
    this.setState({isLoading: false}, () => this.setProgressVisible(false, timeout));
    this.onClose();
  };

  setNewFilter = () => {
    const { filter, selection, setFilter } = this.props;
    const newFilter = filter.clone();
    for(let item of selection) {
      const expandedIndex = newFilter.treeFolders.findIndex(x => x == item.id);
      if(expandedIndex !== -1) {
        newFilter.treeFolders.splice(expandedIndex, 1);
      }
    }

    setFilter(newFilter);
  };

  loopFilesOperations = (id, destFolderId, isCopy) => {
    const { getProgress, filter, currentFolderId, treeFolders, getFolder, isRecycleBinFolder } = this.props;
    getProgress().then(res => {
      const currentItem = res.find(x => x.id === id);
      if(currentItem && currentItem.progress !== 100) {
        this.setProgressValue(currentItem.progress);
        setTimeout(() => this.loopFilesOperations(id, destFolderId, isCopy), 1000);
      } else {
        getFolder(destFolderId).then(data => {
          let newTreeFolders = treeFolders;
          let path = data.pathParts.slice(0);
          let folders = data.folders;
          let foldersCount = data.current.foldersCount;
          loopTreeFolders(path, newTreeFolders, folders, foldersCount);

              if (!isCopy) {
                fetchFiles(currentFolderId, filter, store.dispatch)
                  .then((data) => {
                    if (!isRecycleBinFolder) {
                      newTreeFolders = treeFolders;
                      path = data.selectedFolder.pathParts.slice(0);
                      folders = data.selectedFolder.folders;
                      foldersCount = data.selectedFolder.foldersCount;
                      loopTreeFolders(
                        path,
                        newTreeFolders,
                        folders,
                        foldersCount
                      );
                      setTreeFolders(newTreeFolders);
                    }
                    this.setNewFilter();
                  })
                  .catch(err => this.finishFilesOperations(err))
                  .finally(() => {
                    this.setProgressValue(100);
                    this.finishFilesOperations();
                  });
              } else {
                this.setProgressValue(100);
                this.finishFilesOperations();
                setTreeFolders(newTreeFolders);
              }
            })
            .catch(err => this.finishFilesOperations(err));
        }
      })
      .catch((err) => this.finishFilesOperations(err));
  };

  setSelections = items => {
    const { selection, folders, files, selectFile, deselectFile, fileActionId } = this.props;

    if (selection.length > items.length) {
      //Delete selection
      const newSelection = [];
      let newFile = null;
      for (let item of items) {
        item = item.split("_");
        if (item[0] === "folder") {
          newFile = selection.find((x) => x.id === Number(item[1]) && !x.fileExst);
        } else if (item[0] === "file") {
          newFile = selection.find((x) => x.id === Number(item[1]) && x.fileExst);
        }
        if(newFile) {
          newSelection.push(newFile);
        }
      }

      for(let item of selection) {
        const element = newSelection.find(x => x.id === item.id && x.fileExst === item.fileExst);
        if(!element) {
          deselectFile(item);
        }
      }
    } else if (selection.length < items.length) {
      //Add selection
      for (let item of items) {
        let newFile = null;
        item = item.split("_");
        if (item[0] === "folder") {
          newFile = folders.find((x) => x.id === Number(item[1]) && !x.fileExst);
        } else if (item[0] === "file") {
          newFile = files.find((x) => x.id === Number(item[1]) && x.fileExst);
        }
        if(newFile && fileActionId !== newFile.id) {
          const existItem = selection.find(x => x.id === newFile.id && x.fileExst === newFile.fileExst);
          !existItem && selectFile(newFile);
        }
      }
    } else {
      return;
    }
  };

  setConvertDialogVisible = () => {
    const { files, uploadStatus, uploadToFolder, showConvertDialog } = this.state;
    if(uploadStatus === null) {
      const folderId = uploadToFolder;
      this.updateFiles(folderId);

      const progressBarLabel = this.props.t("UploadingLabel", {
        file: files.length,
        totalFiles: files.length
      });

      const newState = { progressBarLabel, uploadedFiles: 0, percent: 0, progressBarValue: 100, showConvertDialog: !showConvertDialog };

      this.setState(newState, () => this.setProgressVisible(false));
    } else if(!files.length) {
      this.setState({showProgressBar: false, showConvertDialog: !showConvertDialog, isLoading: false, uploadStatus: null});
    } else {
      const totalFiles = files;
      this.setState({totalFiles, showConvertDialog: !showConvertDialog, uploadStatus: "cancel"});
    }
  };

  onConvert = () => {
    let newState = {showConvertDialog: false, uploadStatus: "convert"};
    if(this.state.uploaded) {
      if(!this.state.showProgressBar) {
        newState = {...newState, ...{showProgressBar: true, progressBarValue: 0, percent: 0}};
      }

      this.startSessionFunc(0);
    }
    this.setState(newState);
  };

  componentDidUpdate(prevProps) {
    if (this.props.selection !== prevProps.selection) {
      this.renderGroupButtonMenu();
    }
  }

  render() {
    const {
      isHeaderVisible,
      isHeaderIndeterminate,
      isHeaderChecked,
      selected,
      isLoading,
      showProgressBar,
      progressBarValue,
      progressBarLabel,
      overwriteSetting,
      uploadOriginalFormatSetting,
      hideWindowSetting,
      showConvertDialog
    } = this.state;
    const { t } = this.props;

    const progressBarContent = (
      <div>
        <Checkbox
          onChange={this.onChangeOverwrite}
          isChecked={overwriteSetting}
          label={t("OverwriteSetting")}
        />
        <Checkbox
          onChange={this.onChangeOriginalFormat}
          isChecked={uploadOriginalFormatSetting}
          label={t("UploadOriginalFormatSetting")}
        />
        <Checkbox
          onChange={this.onChangeWindowVisible}
          isChecked={hideWindowSetting}
          label={t("HideWindowSetting")}
        />
      </div>
    );

    return (
      <>
        {showConvertDialog && (
          <ConvertDialog
            visible={showConvertDialog}
            onClose={this.setConvertDialogVisible}
            onConvert={this.onConvert}
          />
        )}
        <RequestLoader
          visible={isLoading}
          zIndex={256}
          loaderSize="16px"
          loaderColor={"#999"}
          label={`${t("LoadingProcessing")} ${t("LoadingDescription")}`}
          fontSize="12px"
          fontColor={"#999"}
        />
        <PageLayout
          withBodyScroll
          withBodyAutoFocus
          uploadFiles
          onDrop={this.onDrop}
          setSelections={this.setSelections}
          onMouseMove={this.onMouseMove}
          showProgressBar={showProgressBar}
          progressBarValue={progressBarValue}
          progressBarDropDownContent={progressBarContent}
          progressBarLabel={progressBarLabel}
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={
            <ArticleMainButtonContent
              onLoading={this.onLoading}
              startUpload={this.startUpload}
              setProgressVisible={this.setProgressVisible}
              setProgressValue={this.setProgressValue}
              setProgressLabel={this.setProgressLabel}
            />
          }
          articleBodyContent={
            <ArticleBodyContent
              onLoading={this.onLoading}
              isLoading={isLoading}
              onTreeDrop={this.onDrop}
            />
          }
          sectionHeaderContent={
            <SectionHeaderContent
              isHeaderVisible={isHeaderVisible}
              isHeaderIndeterminate={isHeaderIndeterminate}
              isHeaderChecked={isHeaderChecked}
              onCheck={this.onSectionHeaderContentCheck}
              onSelect={this.onSectionHeaderContentSelect}
              onClose={this.onClose}
              onLoading={this.onLoading}
              isLoading={isLoading}
              setProgressValue={this.setProgressValue}
              startFilesOperations={this.startFilesOperations}
              finishFilesOperations={this.finishFilesOperations}
              loopFilesOperations={this.loopFilesOperations}
            />
          }
          sectionFilterContent={
            <SectionFilterContent onLoading={this.onLoading} />
          }
          sectionBodyContent={
            <SectionBodyContent
              selected={selected}
              isLoading={isLoading}
              onLoading={this.onLoading}
              onChange={this.onRowChange}
              setProgressValue={this.setProgressValue}
              startFilesOperations={this.startFilesOperations}
              finishFilesOperations={this.finishFilesOperations}
              loopFilesOperations={this.loopFilesOperations}
              onDropZoneUpload={this.onDrop}
            />
          }
          sectionPagingContent={
            <SectionPagingContent onLoading={this.onLoading} />
          }
        />
      </>
    );
  }
}

const HomeContainer = withTranslation()(PureHome);

const Home = (props) => {
  changeLanguage(i18n);
  return (<I18nextProvider i18n={i18n}><HomeContainer {...props} /></I18nextProvider>);
}

Home.propTypes = {
  files: PropTypes.array,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
  const { treeFolders, filter, selectedFolder, selected, selection, folders, files, fileAction } = state.files;
  const { id } = selectedFolder;
  const indexOfTrash = 3;

  return {
    files,
    folders,
    selection,
    selected,
    isLoaded: state.auth.isLoaded,
    currentFolderId: id,
    filter,
    treeFolders,
    isRecycleBinFolder: checkFolderType(id, indexOfTrash, treeFolders),
    fileActionId: fileAction.id
  };
}

export default connect(
  mapStateToProps,
  { setSelected, setTreeFolders, getProgress, getFolder, setFilter, selectFile, deselectFile, setDragging }
)(withRouter(Home));
