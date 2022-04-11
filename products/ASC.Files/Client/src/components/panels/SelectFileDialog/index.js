import React from "react";
import { inject, observer, Provider as MobxProvider } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import throttle from "lodash/throttle";
import stores from "../../../store/index";
import i18n from "./i18n";
import SelectFileDialogAsideView from "./AsideView";
import utils from "@appserver/components/utils";
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
      storeFolderId,
      fileInfo,
      filter,
      id,
      ignoreSelectedFolderTree,
    } = props;

    const resultingId = ignoreSelectedFolderTree ? id : id || storeFolderId;
    this.state = {
      isVisible: false,
      folderId: resultingId || "",
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
      setSelectedNode,
      setSelectedFolder,
      setExpandedPanelKeys,
      displayType,
    } = this.props;
    const { folderId } = this.state;
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
        folderId,
        onSetBaseFolderPath,
        onSelectFolder,
        foldersList,
        true,
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

      folderId: resultingId,
    });
  }
  componentWillUnmount() {
    const { setExpandedPanelKeys, setFolderId, setFile } = this.props;
    this.throttledResize && this.throttledResize.cancel();
    window.removeEventListener("resize", this.throttledResize);

    setExpandedPanelKeys(null);
    //setSelectedFolder(null);

    setFolderId(null);
    setFile(null);
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
      setSelectedNode,
      setExpandedPanelKeys,
      setSelectedFolder,
    } = this.props;
    const { displayType } = this.state;

    const id = displayType === "aside" ? folder : folder[0];

    this.setState({
      folderId: id,
      hasNextPage: true,
      files: [],
      page: 0,
    });

    const isFilesModule =
      window.location.href.indexOf("products/files") !== -1 &&
      window.location.href.indexOf("doceditor") === -1;

    !isFilesModule &&
      SelectionPanel.setFolderObjectToTree(
        id,
        setSelectedNode,
        setExpandedPanelKeys,
        setSelectedFolder
      );
  };

  onSelectFile = (item, index) => {
    const { files } = this.state;
    const { setFile } = this.props;

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

    if (this._isLoadNextPage || !folderId) return;

    this._isLoadNextPage = true;

    const pageCount = 30;
    this.newFilter.page = page;
    this.newFilter.pageCount = pageCount;

    this.setState({ isNextPageLoading: true }, async () => {
      try {
        const data = await getFolder(folderId, this.newFilter);

        const finalData = [...data.files];

        const newFilesList = [...files].concat(finalData);

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
      foldersType,
      withoutProvider,
      filesListTitle,
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
      displayType,
      selectedFile,
      isAvailableFolderList,
      resultingFolderTree,
      isLoadingData,
      page,
      folderId,
    } = this.state;

    const buttonName = creationButtonPrimary
      ? t("Common:Create")
      : t("Common:SaveButton");

    return displayType === "aside" ? (
      <SelectFileDialogAsideView
        t={t}
        theme={theme}
        isPanelVisible={isPanelVisible}
        isFolderPanelVisible={isVisible}
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
        foldersType={foldersType}
        onClickInput={this.onClickInput}
        onCloseSelectFolderDialog={this.onCloseSelectFolderDialog}
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
  id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  withoutProvider: PropTypes.bool,
  ignoreSelectedFolderTree: PropTypes.bool,
  headerName: PropTypes.string,
  filesListTitle: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
};

SelectFileDialogBody.defaultProps = {
  id: "",
  filesListTitle: "",
  withoutProvider: false,
  ignoreSelectedFolderTree: false,
};

const SelectFileDialogWrapper = inject(
  ({
    filesStore,
    selectedFilesStore,
    treeFoldersStore,
    selectedFolderStore,
  }) => {
    const { fileInfo, setFolderId, setFile } = selectedFilesStore;

    const {
      setSelectedNode,
      setExpandedPanelKeys,
      treeFolders,
    } = treeFoldersStore;
    const { filter } = filesStore;
    const { setSelectedFolder, id } = selectedFolderStore;

    return {
      fileInfo,
      setFile,
      setFolderId,
      setSelectedFolder,
      setSelectedNode,
      filter,
      setExpandedPanelKeys,
      treeFolders,
      storeFolderId: id,
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
