import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import { RequestLoader, Checkbox, toastr } from "asc-web-components";
import { PageLayout, utils } from "asc-web-common";
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
import {
  setSelected,
  fetchFiles,
  setTreeFolders,
  getProgress,
  getFolder,
  setFilter,
  selectFile,
  deselectFile,
  setDragging,
  setProgressBarData,
  clearProgressData
} from "../../../store/files/actions";
import { loopTreeFolders, checkFolderType } from "../../../store/files/selectors";
import store from "../../../store/store";
import { ConvertDialog } from "../../dialogs";
import { startUpload, onConvert, setDialogVisible } from "./FilesUploader";

const { changeLanguage } = utils;

class PureHome extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isHeaderVisible: false,
      isHeaderIndeterminate: false,
      isHeaderChecked: false,
      isLoading: false,

      overwriteSetting: false,
      uploadOriginalFormatSetting: false,
      hideWindowSetting: false,

      files: [],
      uploadedFiles: 0,
      percent: 0,

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

  onDrop = (e, uploadToFolder) => {
    const items = e.dataTransfer.items;
    let files = [];
    const { t, currentFolderId, startUpload } = this.props;
    const folderId = uploadToFolder ? uploadToFolder : currentFolderId;

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

    this.props.setDragging(false);
    readItems(items, () => startUpload(files, folderId, t));
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

  onChangeOverwrite = () =>
    this.setState({ overwriteSetting: !this.state.overwriteSetting });
  onChangeOriginalFormat = () =>
    this.setState({
      uploadOriginalFormatSetting: !this.state.uploadOriginalFormatSetting,
    });
  onChangeWindowVisible = () =>
    this.setState({ hideWindowSetting: !this.state.hideWindowSetting });

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
    const { getProgress, filter, currentFolderId, treeFolders, getFolder, isRecycleBinFolder, progressData, setProgressBarData } = this.props;
    getProgress().then(res => {
      const currentItem = res.find(x => x.id === id);
      if(currentItem && currentItem.progress !== 100) {
        setProgressBarData({ label: progressData.label, percent: currentItem.progress, visible: true });
        setTimeout(() => this.loopFilesOperations(id, destFolderId, isCopy), 1000);
      } else {
        setProgressBarData({ label: progressData.label, percent: 100, visible: true });
        getFolder(destFolderId).then(data => {
          let newTreeFolders = treeFolders;
          let path = data.pathParts.slice(0);
          let folders = data.folders;
          let foldersCount = data.current.foldersCount;
          loopTreeFolders(path, newTreeFolders, folders, foldersCount);
          
              if (!isCopy || destFolderId === currentFolderId) {
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
                  .catch(err => {
                    toastr.error(err);
                    clearProgressData(store.dispatch);
                  })
                  .finally(() => setTimeout(() => clearProgressData(store.dispatch), 5000));
              } else {
                setProgressBarData({ label: progressData.label, percent: 100, visible: true });
                setTimeout(() => clearProgressData(store.dispatch), 5000);
                setTreeFolders(newTreeFolders);
              }
            })
            .catch(err => {
              toastr.error(err);
              clearProgressData(store.dispatch);
            });
        }
      })
      .catch((err) => {
        toastr.error(err);
        clearProgressData(store.dispatch);
      });
  };

  setSelections = items => {
    const { selection, folders, files, selectFile, deselectFile, fileActionId } = this.props;

    if (selection.length > items.length) {
      //Delete selection
      const newSelection = [];
      let newFile = null;
      for (let item of items) {
        if(!item) break   // temporary fall protection selection tile 
        
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
        if(!item) break // temporary fall protection selection tile 

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
      overwriteSetting,
      uploadOriginalFormatSetting,
      hideWindowSetting
    } = this.state;
    const { t, progressData, viewAs, convertDialogVisible } = this.props;

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
        {convertDialogVisible && (
          <ConvertDialog
            visible={convertDialogVisible}
            onClose={setDialogVisible}
            onConvert={onConvert}
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
          showProgressBar={progressData.visible}
          progressBarValue={progressData.percent}
          //progressBarDropDownContent={progressBarContent}
          progressBarLabel={progressData.label}
          viewAs={viewAs}
          articleHeaderContent={<ArticleHeaderContent />}
          articleMainButtonContent={
            <ArticleMainButtonContent
              onLoading={this.onLoading}
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
  const {
    treeFolders,
    filter,
    selectedFolder,
    selected,
    selection,
    folders,
    files,
    fileAction,
    progressData,
    viewAs,
    convertDialogVisible
  } = state.files;
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
    fileActionId: fileAction.id,
    progressData,
    viewAs,
    convertDialogVisible
  };
}

export default connect(mapStateToProps,
  { setSelected, setTreeFolders, getProgress, getFolder, setFilter, selectFile, deselectFile, setDragging, setProgressBarData, startUpload }
)(withRouter(Home));
