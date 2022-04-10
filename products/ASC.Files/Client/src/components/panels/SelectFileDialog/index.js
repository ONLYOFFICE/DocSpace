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
import SelectionPanel from "../SelectionPanel/SelectionPanelBody";
import toastr from "studio/toastr";
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

    this.state = {
      isVisible: false,
      folderId: storeFolderId || "",
      //passedId: folderId,
      selectedFile: fileInfo || "",
      fileName: (fileInfo && fileInfo.title) || "",
      files: [],
      hasNextPage: true,
      isNextPageLoading: false,
      displayType: this.getDisplayType(),
      page: 0,
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
    const { withSubfolders = true } = this.props;

    const filterParams = this.getFilterParameters();
    this.newFilter.filterType = filterParams.filterType;
    this.newFilter.search = filterParams.filterValue;
    this.newFilter.withSubfolders = withSubfolders;
  };

  async componentDidMount() {
    const {
      treeFolders,
      foldersType,
      id,
      onSetBaseFolderPath,
      onSelectFolder,
      foldersList,
      treeFromInput,
      isSetFolderImmediately,
      setSelectedNode,
      setSelectedFolder,
      setExpandedPanelKeys,
      displayType,
    } = this.props;
    !displayType && window.addEventListener("resize", this.throttledResize);

    this.setFilter();

    let timerId = setTimeout(() => {
      this.setState({ isInitialLoader: true });
    }, 1000);

    let resultingFolderTree, resultingId;

    try {
      [
        resultingFolderTree,
        resultingId,
      ] = await SelectionPanel.getBasicFolderInfo(
        treeFolders,
        foldersType,
        id,
        onSetBaseFolderPath,
        onSelectFolder,
        foldersList,
        isSetFolderImmediately,
        setSelectedNode,
        setSelectedFolder,
        setExpandedPanelKeys
      );

      clearTimeout(timerId);
      timerId = null;
    } catch (e) {
      toastr.error(e);

      clearTimeout(timerId);
      timerId = null;
      this.setState({ isInitialLoader: false });

      return;
    }

    const tree = treeFromInput ? treeFromInput : resultingFolderTree;

    if (tree.length === 0) {
      this.setState({ isAvailable: false });
      onSelectFolder(null);
      return;
    }

    this.setState({
      resultingFolderTree: tree,
      isInitialLoader: false,

      folderId: treeFromInput ? id : resultingId,
    });
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

  onSelectFolder = (folder) => {
    const {
      // setFolderId,
      setSelectedNode,
      setExpandedPanelKeys,
      setSelectedFolder,
    } = this.props;

    // setFolderId(id);

    this.setState({
      folderId: folder[0],
      hasNextPage: true,
      files: [],
      page: 0,
    });

    SelectionPanel.setFolderObjectToTree(
      folder[0],
      setSelectedNode,
      setExpandedPanelKeys,
      setSelectedFolder
    );
  };

  onSelectFile = (item, index) => {
    const { files } = this.state;
    const { setFile } = this.props;
    console.log("item", item);

    setFile(files[+index]);

    this.setState({
      selectedFile: files[+index],
      fileName: files[+index].title,
    });
  };

  onClickSave = () => {
    const { onSetFileNameAndLocation, onClose, onSelectFile } = this.props;
    const { fileName, selectedFile, folderId } = this.state;

    onSetFileNameAndLocation && onSetFileNameAndLocation(fileName, folderId);
    onSelectFile && onSelectFile(selectedFile);
    onClose && onClose();
  };

  _loadNextPage = () => {
    const { files, page, folderId } = this.state;

    if (this._isLoadNextPage) return;

    this._isLoadNextPage = true;

    const pageCount = 30;
    this.newFilter.page = page;
    this.newFilter.pageCount = pageCount;

    this.setState({ isNextPageLoading: true }, async () => {
      try {
        const data = await getFolder(folderId, this.newFilter);

        const finalData = [...data.files];

        const newFilesList = [...files].concat(finalData);
        console.log("newFilesList", newFilesList);
        const hasNextPage = newFilesList.length < data.total - 1;

        this._isLoadNextPage = false;
        this.setState((state) => ({
          isDataLoading: false,
          hasNextPage: hasNextPage,
          isNextPageLoading: false,
          page: state.page + 1,
          files: newFilesList,
        }));
      } catch (e) {
        toastr.error(e);
        this.setState({
          isDataLoading: false,
        });
      }
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
      filesListTitle,
      onSetFileName,
      tReady,
      headerName,
      theme,
      header,
      footer,
      dialogName,
      creationButtonPrimary,
    } = this.props;
    const {
      isVisible,
      files,
      hasNextPage,
      isNextPageLoading,
      selectedFolder,
      displayType,
      selectedFile,
      fileName,
      passedId,
      isAvailableFolderList,
      resultingFolderTree,
      isLoadingData,
      page,
      folderId,
    } = this.state;

    const buttonName = creationButtonPrimary
      ? t("Common:Create")
      : t("Common:SaveButton");

    console.log("filesListTitle", filesListTitle);
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
        files={files}
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
        selectedFile={selectedFile}
        folderId={folderId}
        onSetFileName={onSetFileName}
        fileName={fileName}
        displayType={displayType}
        isTranslationsReady={tReady}
        passedId={passedId}
        filesListTitle={filesListTitle}
        isAvailableFolderList={isAvailableFolderList}
        primaryButtonName={buttonName}
      />
    ) : (
      <SelectionPanel
        t={t}
        theme={theme}
        isPanelVisible={isPanelVisible}
        onClose={onClose}
        withoutProvider={withoutProvider}
        folderId={folderId}
        resultingFolderTree={resultingFolderTree}
        onButtonClick={this.onClickSave}
        header={header}
        dialogName={dialogName}
        footer={footer}
        isLoadingData={isLoadingData}
        primaryButtonName={buttonName}
        isAvailable={isAvailableFolderList}
        onSelectFolder={this.onSelectFolder}
        files={files}
        isNextPageLoading={isNextPageLoading}
        page={page}
        hasNextPage={hasNextPage}
        loadNextPage={this._loadNextPage}
        onSelectFile={this.onSelectFile}
        filesListTitle={filesListTitle}
        fileId={selectedFile.id}
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

    const {
      setSelectedNode,
      setExpandedPanelKeys,
      treeFolders,
    } = treeFoldersStore;
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
      treeFolders,
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
