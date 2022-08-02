import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import throttle from "lodash/throttle";
import SelectFileDialogAsideView from "./AsideView";
import utils from "@docspace/components/utils";
import { FilterType } from "@docspace/common/constants";
import isEqual from "lodash/isEqual";
import SelectionPanel from "../SelectionPanel/SelectionPanelBody";
import toastr from "client/toastr";

const { desktop } = utils.device;
class SelectFileDialog extends React.Component {
  constructor(props) {
    super(props);
    const { filter } = props;

    this.state = {
      isVisible: false,
      files: [],
      displayType: this.getDisplayType(),
      isAvailableFolderList: true,
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
      treeFolders,
      foldersType,
      id,
      onSetBaseFolderPath,
      onSelectFolder,
      foldersList,
      treeFromInput,
      displayType,
      setFolderId,
      folderId,
    } = this.props;

    !displayType && window.addEventListener("resize", this.throttledResize);

    this.setFilter();

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
        true
      );
    } catch (e) {
      toastr.error(e);

      return;
    }

    const tree = treeFromInput ? treeFromInput : resultingFolderTree;

    if (tree.length === 0) {
      this.setState({ isAvailable: false });
      onSelectFolder(null);
      return;
    }

    setFolderId(resultingId);

    this.setState({
      resultingFolderTree: tree,
    });
  }

  componentDidUpdate(prevProps) {
    if (!isEqual(prevProps, this.props)) {
      this.setFilter();
    }
  }

  componentWillUnmount() {
    const {
      setFolderId,
      setFile,
      setExpandedPanelKeys,
      withoutResetFolderTree,
    } = this.props;
    this.throttledResize && this.throttledResize.cancel();
    window.removeEventListener("resize", this.throttledResize);

    if (!withoutResetFolderTree) {
      setExpandedPanelKeys(null);
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
    const { displayType } = this.state;
    const { setFolderId, setFile, folderId } = this.props;
    const id = displayType === "aside" ? folder : folder[0];

    if (id !== folderId) {
      setFolderId(id);
      setFile(null);
    }
  };

  onSelectFile = (item, index) => {
    const { setFile } = this.props;

    setFile(item);
  };

  onClickSave = () => {
    const {
      onSetFileNameAndLocation,
      onClose,
      onSelectFile,
      fileInfo,
      folderId,
    } = this.props;

    const fileName = fileInfo.title;

    onSetFileNameAndLocation && onSetFileNameAndLocation(fileName, folderId);
    onSelectFile && onSelectFile(fileInfo);
    onClose && onClose();
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
      maxInputWidth,
      folderId,
      fileInfo,
    } = this.props;
    const {
      isVisible,
      displayType,
      isAvailableFolderList,
      resultingFolderTree,
      isLoadingData,
    } = this.state;

    const buttonName = creationButtonPrimary
      ? t("Common:Create")
      : t("Common:SaveButton");
    const name = dialogName ? dialogName : t("SelectFile");

    // console.log("Render file-component");
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
        dialogName={name}
        footer={footer}
        isLoadingData={isLoadingData}
        primaryButtonName={buttonName}
        isAvailable={isAvailableFolderList}
        onSelectFolder={this.onSelectFolder}
        onSelectFile={this.onSelectFile}
        filesListTitle={filesListTitle}
        fileId={fileInfo?.id}
        newFilter={this.newFilter}
        foldersType={foldersType}
        onClickInput={this.onClickInput}
        onCloseSelectFolderDialog={this.onCloseSelectFolderDialog}
        maxInputWidth={maxInputWidth}
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
        dialogName={name}
        footer={footer}
        isLoadingData={isLoadingData}
        primaryButtonName={buttonName}
        isAvailable={isAvailableFolderList}
        onSelectFolder={this.onSelectFolder}
        onSelectFile={this.onSelectFile}
        filesListTitle={filesListTitle}
        fileId={fileInfo?.id}
        newFilter={this.newFilter}
      />
    );
  }
}
SelectFileDialog.propTypes = {
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
  withoutResetFolderTree: PropTypes.bool,
};

SelectFileDialog.defaultProps = {
  id: "",
  filesListTitle: "",
  withoutProvider: false,
  ignoreSelectedFolderTree: false,
  withoutResetFolderTree: false,
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

    const { treeFolders, setExpandedPanelKeys } = treeFoldersStore;
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
      treeFolders,
      storeFolderId,

      folderId,
      theme: theme,
      setExpandedPanelKeys,
    };
  }
)(
  observer(
    withTranslation(["SelectFile", "Common", "Translations"])(SelectFileDialog)
  )
);
