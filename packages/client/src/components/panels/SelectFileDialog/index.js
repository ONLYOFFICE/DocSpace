import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import throttle from "lodash/throttle";
import SelectFileDialogAsideView from "./AsideView";
import utils from "@docspace/components/utils";
import { FilterType, FolderType } from "@docspace/common/constants";
import isEqual from "lodash/isEqual";
import SelectionPanel from "../SelectionPanel/SelectionPanelBody";
import toastr from "@docspace/components/toast/toastr";

const { desktop } = utils.device;
class SelectFileDialog extends React.Component {
  constructor(props) {
    super(props);
    const { filter, folderId, fileInfo } = props;

    this.state = {
      isVisible: false,
      selectedFileInfo: {},
      displayType: this.getDisplayType(),
      isAvailableFolderList: true,
      selectedFolderId: folderId,
      selectedFileInfo: fileInfo,
    };
    this.throttledResize = throttle(this.setDisplayType, 300);
    this.newFilter = filter.clone();
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
      filteredType,
      onSelectFolder,
      passedFoldersTree,
      displayType,
      folderId,
      withoutBasicSelection,
    } = this.props;

    !displayType && window.addEventListener("resize", this.throttledResize);

    this.setFilter();

    let resultingFolderTree, resultingId;

    const treeFolders = await this.props.fetchTreeFolders();
    const roomsFolder = treeFolders.find(
      (f) => f.rootFolderType == FolderType.Rooms
    );
    const hasSharedFolder =
      roomsFolder && roomsFolder.foldersCount ? true : false;

    try {
      [
        resultingFolderTree,
        resultingId,
      ] = await SelectionPanel.getBasicFolderInfo(
        treeFolders,
        filteredType,
        folderId,
        passedFoldersTree,
        hasSharedFolder
      );
    } catch (e) {
      toastr.error(e);

      return;
    }

    const tree = resultingFolderTree;

    if (tree.length === 0) {
      this.setState({ isAvailable: false });
      onSelectFolder && onSelectFolder(null);
      return;
    }

    if (!withoutBasicSelection) {
      onSelectFolder && onSelectFolder(resultingId);
    }

    this.setState({
      resultingFolderTree: tree,
      selectedFolderId: resultingId,
    });
  }

  componentDidUpdate(prevProps) {
    if (!isEqual(prevProps, this.props)) {
      this.setFilter();
    }
  }

  componentWillUnmount() {
    this.throttledResize && this.throttledResize.cancel();
    window.removeEventListener("resize", this.throttledResize);
  }

  getDisplayType = () => {
    const displayType = this.props.displayType
      ? this.props.displayType
      : window.innerWidth < desktop.match(/\d+/)[0]
      ? "aside"
      : "modal";

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
    const { displayType } = this.state;
    const { folderId } = this.props;
    const id = displayType === "aside" ? folder : folder[0];

    if (id !== folderId) {
      this.setState({
        selectedFolderId: id,
      });
    }
  };

  onSelectFile = (item, index) => {
    this.setState({
      selectedFileInfo: item,
    });
  };

  onClickSave = () => {
    const {
      onClose,
      onSelectFile,
      setFile,
      setFolderId,
      embedded,
    } = this.props;
    const { selectedFileInfo, selectedFolderId } = this.state;

    setFile(selectedFileInfo);

    if (selectedFileInfo.folderId.toString() === selectedFolderId.toString()) {
      setFolderId(selectedFolderId);
    }

    onSelectFile && onSelectFile(selectedFileInfo);
    onClose && !embedded && onClose();
  };

  render() {
    const {
      t,
      isPanelVisible,
      onClose,
      filteredType,
      withoutProvider,
      filesListTitle,
      theme,
      header,
      footer,
      dialogName,
      creationButtonPrimary,
      maxInputWidth,
      embedded,
    } = this.props;
    const {
      isVisible,
      displayType,
      isAvailableFolderList,
      resultingFolderTree,
      isLoadingData,
      selectedFileInfo,
      selectedFolderId,
    } = this.state;

    const buttonName = creationButtonPrimary
      ? t("Common:Create")
      : embedded
      ? t("Common:SelectFile")
      : t("Common:SaveButton");
    const name = dialogName ? dialogName : t("Common:SelectFile");

    // console.log("Render file-component");
    return displayType === "aside" ? (
      <SelectFileDialogAsideView
        t={t}
        theme={theme}
        isPanelVisible={isPanelVisible}
        isFolderPanelVisible={isVisible}
        onClose={onClose}
        withoutProvider={withoutProvider}
        folderId={selectedFolderId}
        resultingFolderTree={resultingFolderTree}
        onButtonClick={this.onClickSave}
        header={header}
        dialogName={name}
        footer={footer}
        isLoadingData={isLoadingData}
        primaryButtonName={buttonName}
        isAvailable={isAvailableFolderList}
        onSelectFolder={this.onSelectFolder}
        onSelectFile={this.onSelectFile}
        filesListTitle={filesListTitle}
        fileId={selectedFileInfo.id}
        newFilter={this.newFilter}
        filteredType={filteredType}
        onClickInput={this.onClickInput}
        onCloseSelectFolderDialog={this.onCloseSelectFolderDialog}
        maxInputWidth={maxInputWidth}
        embedded={embedded}
      />
    ) : (
      <SelectionPanel
        t={t}
        theme={theme}
        isPanelVisible={isPanelVisible}
        onClose={onClose}
        withoutProvider={withoutProvider}
        folderId={selectedFolderId}
        resultingFolderTree={resultingFolderTree}
        onButtonClick={this.onClickSave}
        header={header}
        dialogName={name}
        footer={footer}
        isLoadingData={isLoadingData}
        primaryButtonName={buttonName}
        isAvailable={isAvailableFolderList}
        onSelectFolder={this.onSelectFolder}
        onSelectFile={this.onSelectFile}
        filesListTitle={filesListTitle}
        fileId={selectedFileInfo.id}
        newFilter={this.newFilter}
      />
    );
  }
}
SelectFileDialog.propTypes = {
  onClose: PropTypes.func,
  isPanelVisible: PropTypes.bool.isRequired,
  onSelectFile: PropTypes.func.isRequired,
  filteredType: PropTypes.oneOf([
    "exceptSortedByTags",
    "exceptPrivacyTrashArchiveFolders",
    "roomsOnly",
    "userFolderOnly",
  ]),
  id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  withoutProvider: PropTypes.bool,
  headerName: PropTypes.string,
  filesListTitle: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
};

SelectFileDialog.defaultProps = {
  id: "",
  filesListTitle: "",
  withoutProvider: false,
};

export default inject(
  ({
    auth,
    filesStore,
    treeFoldersStore,
    selectedFolderStore,
    selectFileDialogStore,
  }) => {
    const {
      folderId: id,
      fileInfo,
      setFolderId,
      setFile,
    } = selectFileDialogStore;

    const { setExpandedPanelKeys, fetchTreeFolders } = treeFoldersStore;
    const { filter } = filesStore;
    const { id: storeFolderId } = selectedFolderStore;

    const { settingsStore } = auth;
    const { theme } = settingsStore;
    const folderId = id ? id : storeFolderId;

    return {
      fileInfo,
      setFile,
      setFolderId,
      filter,
      storeFolderId,

      folderId,
      theme: theme,
      setExpandedPanelKeys,
      fetchTreeFolders,
    };
  }
)(
  observer(
    withTranslation(["SelectFile", "Common", "Translations"])(SelectFileDialog)
  )
);
