import React from "react";
import { inject, observer, Provider as MobxProvider } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import throttle from "lodash/throttle";

import stores from "../../../store/index";
import i18n from "./i18n";
import SelectFileDialogModalView from "./ModalView";
import SelectFileDialogAsideView from "./AsideView";

import utils from "@appserver/components/utils";
//import SelectFolderDialog from "../SelectFolderDialog";
import { getFolder } from "@appserver/common/api/files";
import { FilterType } from "@appserver/common/constants";

const { desktop } = utils.device;

import store from "studio/store";

const { auth: authStore } = store;

class SelectFileDialogBody extends React.Component {
  constructor(props) {
    super(props);
    const {
      folderId,
      storeFolderId,
      fileInfo,
      filter,
      creationButtonPrimary,
      t,
    } = props;

    this.buttonName = creationButtonPrimary
      ? t("Common:Create")
      : t("Common:SaveButton");

    this.state = {
      isVisible: false,
      selectedFolder: storeFolderId || "",
      passedId: folderId,
      selectedFile: fileInfo || "",
      fileName: (fileInfo && fileInfo.title) || "",
      filesList: [],
      hasNextPage: true,
      isNextPageLoading: false,
      displayType: this.getDisplayType(),
      page: 0,
      filterParams: this.getFilterParameters(),
      isAvailableFolderList: true,
    };
    this.throttledResize = throttle(this.setDisplayType, 300);
    this.newFilter = filter.clone();
    this._isLoadNextPage = false;
  }

  getFilterParameters = () => {
    const {
      isImageOnly,
      isDocumentsOnly,
      isArchiveOnly,
      isPresentationOnly,
      isTablesOnly,
      isMediaOnly,
      searchParam = "",
      ByExtension,
    } = this.props;

    if (isImageOnly) {
      return { filterType: FilterType.ImagesOnly, filterValue: searchParam };
    }
    if (isDocumentsOnly) {
      return { filterType: FilterType.DocumentsOnly, filterValue: searchParam };
    }
    if (isArchiveOnly) {
      return { filterType: FilterType.ArchiveOnly, filterValue: searchParam };
    }
    if (isPresentationOnly) {
      return {
        filterType: FilterType.PresentationsOnly,
        filterValue: searchParam,
      };
    }
    if (isTablesOnly) {
      return {
        filterType: FilterType.SpreadsheetsOnly,
        filterValue: searchParam,
      };
    }
    if (isMediaOnly) {
      return { filterType: FilterType.MediaOnly, filterValue: searchParam };
    }

    if (ByExtension) {
      return { filterType: FilterType.ByExtension, filterValue: searchParam };
    }

    return { filterType: FilterType.FilesOnly, filterValue: "" };
  };

  setFilter = () => {
    const { filterParams } = this.state;
    const { withSubfolders = true } = this.props;

    this.newFilter.filterType = filterParams.filterType;
    this.newFilter.search = filterParams.filterValue;
    this.newFilter.withSubfolders = withSubfolders;
  };

  componentDidMount() {
    authStore.init(true); // it will work if authStore is not initialized

    window.addEventListener("resize", this.throttledResize);
    this.setFilter();
  }
  componentWillUnmount() {
    const {
      resetTreeFolders,
      setExpandedPanelKeys,
      setDefaultSelectedFolder,
      setSelectedFolder,
      setFolderId,
      setFile,
    } = this.props;
    this.throttledResize && this.throttledResize.cancel();
    window.removeEventListener("resize", this.throttledResize);

    if (resetTreeFolders) {
      setExpandedPanelKeys(null);
      //setSelectedFolder(null);

      setFolderId(null);
      setFile(null);
    }
  }

  getDisplayType = () => {
    const displayType =
      window.innerWidth < desktop.match(/\d+/)[0] ? "aside" : "modal";

    return displayType;
  };

  setDisplayType = () => {
    const displayType = this.getDisplayType();

    this.setState({ displayType: displayType });
  };

  onClickInput = () => {
    this.setState({
      isVisible: true,
    });
  };

  onCloseSelectFolderDialog = () => {
    this.setState({
      isVisible: false,
    });
  };

  onSelectFolder = (id) => {
    const { setFolderId } = this.props;

    if (id) {
      setFolderId(id);

      this.setState({
        selectedFolder: id,
        hasNextPage: true,
        filesList: [],
        page: 0,
      });
    } else
      this.setState({
        isAvailableFolderList: false,
      });
  };

  onSelectFile = (e) => {
    const { filesList } = this.state;
    const { setFile } = this.props;
    const index = e.target.dataset.index || e.target.name;

    if (!index) return;
    setFile(filesList[+index]);
    this.setState({
      selectedFile: filesList[+index],
      fileName: filesList[+index].title,
    });
  };

  onClickSave = () => {
    const { onSetFileName, onClose, onSelectFile } = this.props;
    const { fileName, selectedFile } = this.state;

    onSetFileName && onSetFileName(fileName);
    onSelectFile && onSelectFile(selectedFile);
    onClose && onClose();
  };

  loadNextPage = () => {
    // const {
    //   setSelectedNode,
    //   setSelectedFolder,
    //   setExpandedPanelKeys,
    // } = this.props;
    const { selectedFolder, page } = this.state;

    if (this._isLoadNextPage) return;

    this._isLoadNextPage = true;

    const pageCount = 30;
    this.newFilter.page = page;
    this.newFilter.pageCount = pageCount;

    this.setState({ isNextPageLoading: true }, () => {
      getFolder(selectedFolder, this.newFilter)
        .then((data) => {
          let newFilesList = page
            ? this.state.filesList.concat(data.files)
            : data.files;

          //TODO:  it will need if passed the folder id - need to come up with a different solution.

          // const newPathParts = SelectFolderDialog.convertPathParts(
          //
          //   data.pathParts
          // );

          //setExpandedPanelKeys(newPathParts);

          // setSelectedNode([selectedFolder + ""]);
          // setSelectedFolder({
          //   folders: data.folders,
          //   ...data.current,
          //   pathParts: newPathParts,
          //   ...{ new: data.new },
          // });
          this.setState({
            hasNextPage: newFilesList.length < data.total,
            isNextPageLoading: false,
            filesList: newFilesList,
            page: page + 1,
          });
        })
        .catch((error) => console.log(error))
        .finally(() => (this._isLoadNextPage = false));
    });
  };
  render() {
    const {
      t,
      isPanelVisible,
      onClose,
      zIndex,
      foldersType,
      withoutProvider,
      titleFilesList,
      loadingLabel,
      folderId,
      onSetFileName,
      tReady,
      headerName,
      foldersList,
      theme,
    } = this.props;
    const {
      isVisible,
      filesList,
      hasNextPage,
      isNextPageLoading,
      selectedFolder,
      displayType,
      selectedFile,
      fileName,
      passedId,
      isAvailableFolderList,
    } = this.state;

    const loadingText = loadingLabel
      ? loadingLabel
      : `${t("Common:LoadingProcessing")} ${t("Common:LoadingDescription")}`;

    return displayType === "aside" ? (
      <SelectFileDialogAsideView
        theme={theme}
        t={t}
        isPanelVisible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        isVisible={isVisible}
        withoutProvider={withoutProvider}
        foldersType={foldersType}
        filesList={filesList}
        onSelectFile={this.onSelectFile}
        onClickInput={this.onClickInput}
        onClickSave={this.onClickSave}
        onCloseSelectFolderDialog={this.onCloseSelectFolderDialog}
        onSelectFolder={this.onSelectFolder}
        hasNextPage={hasNextPage}
        isNextPageLoading={isNextPageLoading}
        loadNextPage={this.loadNextPage}
        selectedFolder={selectedFolder}
        headerName={headerName}
        loadingText={loadingText}
        selectedFile={selectedFile}
        folderId={folderId}
        onSetFileName={onSetFileName}
        fileName={fileName}
        displayType={displayType}
        isTranslationsReady={tReady}
        passedId={passedId}
        titleFilesList={titleFilesList}
        isAvailableFolderList={isAvailableFolderList}
        primaryButtonName={this.buttonName}
      />
    ) : (
      <SelectFileDialogModalView
        theme={theme}
        t={t}
        isPanelVisible={isPanelVisible}
        onClose={onClose}
        onSelectFolder={this.onSelectFolder}
        onSelectFile={this.onSelectFile}
        foldersType={foldersType}
        onClickSave={this.onClickSave}
        filesList={filesList}
        hasNextPage={hasNextPage}
        isNextPageLoading={isNextPageLoading}
        loadNextPage={this.loadNextPage}
        selectedFolder={selectedFolder}
        withoutProvider={withoutProvider}
        headerName={headerName}
        loadingText={loadingText}
        selectedFile={selectedFile}
        folderId={folderId}
        passedId={passedId}
        titleFilesList={titleFilesList}
        primaryButtonName={this.buttonName}
        foldersList={foldersList}
      />
    );
  }
}
SelectFileDialogBody.propTypes = {
  onClose: PropTypes.func.isRequired,
  isPanelVisible: PropTypes.bool.isRequired,
  onSelectFile: PropTypes.func.isRequired,
  foldersType: PropTypes.oneOf([
    "common",
    "third-party",
    "exceptSortedByTags",
    "exceptPrivacyTrashFolders",
  ]),
  folderId: PropTypes.string,
  withoutProvider: PropTypes.bool,
  creationButtonPrimary: PropTypes.bool,
  headerName: PropTypes.string,
  titleFilesList: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  zIndex: PropTypes.number,
};

SelectFileDialogBody.defaultProps = {
  folderId: "",
  titleFilesList: "",
  withoutProvider: false,
  zIndex: 310,
  creationButtonPrimary: false,
};

const SelectFileDialogWrapper = inject(
  ({
    filesStore,
    selectedFilesStore,
    treeFoldersStore,
    selectedFolderStore,
  }) => {
    const {
      folderId: storeFolderId,
      fileInfo,
      setFolderId,
      setFile,
    } = selectedFilesStore;

    const { setSelectedNode, setExpandedPanelKeys } = treeFoldersStore;
    const { filter } = filesStore;
    const { setSelectedFolder, id } = selectedFolderStore;
    return {
      storeFolderId: storeFolderId || id,
      fileInfo,
      setFile,
      setFolderId,
      setSelectedFolder,
      setSelectedNode,
      filter,
      setExpandedPanelKeys,
    };
  }
)(
  observer(
    withTranslation(["SelectFile", "Common", "Translations"])(
      SelectFileDialogBody
    )
  )
);
class SelectFileDialog extends React.Component {
  render() {
    return (
      <MobxProvider auth={authStore} {...stores}>
        <I18nextProvider i18n={i18n}>
          <SelectFileDialogWrapper {...this.props} />
        </I18nextProvider>
      </MobxProvider>
    );
  }
}

export default SelectFileDialog;
