import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import throttle from "lodash/throttle";
import SelectFolderDialogAsideView from "./AsideView";
import utils from "@docspace/components/utils";
import toastr from "@docspace/components/toast/toastr";
import SelectionPanel from "../SelectionPanel/SelectionPanelBody";
import { FilterType, FolderType } from "@docspace/common/constants";

const { desktop } = utils.device;

let treeFolders = [];
let selectedTreeNode = null;
class SelectFolderDialog extends React.Component {
  constructor(props) {
    super(props);
    const { id, displayType, filter } = this.props;
    this.newFilter = filter.clone();
    this.newFilter.filterType = FilterType.FilesOnly;
    this.newFilter.withSubfolders = false;
    this.state = {
      isLoadingData: false,
      displayType: displayType || this.getDisplayType(),
      isAvailable: true,
    };
    this.throttledResize = throttle(this.setDisplayType, 300);
  }

  async componentDidMount() {
    const {
      filteredType,
      onSetBaseFolderPath,
      onSelectFolder,
      passedFoldersTree,
      displayType,
      withFileSelectDialog = false,
      folderTree,
      setResultingFolderId,
      selectFolderInputExist,
      id,
      storeFolderId,
      withoutBasicSelection,
      setResultingFoldersTree,
      embedded,
    } = this.props;

    !displayType && window.addEventListener("resize", this.throttledResize);

    const initialFolderId = selectFolderInputExist ? id : storeFolderId;

    let resultingFolderTree, resultingId;

    if (!withFileSelectDialog || embedded) {
      treeFolders = await this.props.fetchTreeFolders();

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
          embedded ? null : initialFolderId,
          passedFoldersTree,
          hasSharedFolder
        );
      } catch (e) {
        toastr.error(e);

        return;
      }
    }

    const tree = withFileSelectDialog ? folderTree : resultingFolderTree;

    if (tree.length === 0) {
      this.setState({ isAvailable: false });
      onSelectFolder && onSelectFolder(null);
      return;
    }

    setResultingFoldersTree(tree);

    const resId = withFileSelectDialog ? id : resultingId;

    if (!withoutBasicSelection || embedded) {
      onSelectFolder && onSelectFolder(resId);
      onSetBaseFolderPath && onSetBaseFolderPath(resId);
    }

    setResultingFolderId(resId);
  }

  componentWillUnmount() {
    const { toDefault } = this.props;

    if (this.throttledResize) {
      this.throttledResize && this.throttledResize.cancel();
      window.removeEventListener("resize", this.throttledResize);
    }

    toDefault();
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

  onSelect = async (folder, treeNode) => {
    const {
      setResultingFolderId,
      resultingFolderId,
      onSelectTreeNode,
      setItemSecurity,
    } = this.props;

    if (+resultingFolderId === +folder[0]) return;
    if (!!onSelectTreeNode) selectedTreeNode = treeNode;

    setResultingFolderId(folder[0]);

    setItemSecurity(treeNode.security);
  };

  onClose = () => {
    const {
      setExpandedPanelKeys,
      onClose,

      selectFolderInputExist,
      withFileSelectDialog,
    } = this.props;

    if (
      !treeFolders.length &&
      !selectFolderInputExist &&
      !withFileSelectDialog
    ) {
      setExpandedPanelKeys(null);
    }
    onClose && onClose();
  };

  onCloseAside = () => {
    const { onClose } = this.props;
    onClose && onClose();
  };

  onButtonClick = (e) => {
    const {
      onSave,
      onSetNewFolderPath,
      onSelectFolder,
      onSelectTreeNode,
      withoutImmediatelyClose,
      onSubmit,

      providerKey,
      folderTitle,
      resultingFolderId,
      setSelectedItems,
    } = this.props;

    setSelectedItems();

    onSubmit && onSubmit(resultingFolderId, folderTitle, providerKey);
    onSave && onSave(e, resultingFolderId);
    onSetNewFolderPath && onSetNewFolderPath(resultingFolderId);
    onSelectFolder && onSelectFolder(resultingFolderId);
    onSelectTreeNode && onSelectTreeNode(selectedTreeNode);
    //setResultingFolderId(resultingFolderId);
    !withoutImmediatelyClose && this.onClose();
  };

  render() {
    const {
      t,
      isPanelVisible,
      zIndex,
      withoutProvider,
      withFileSelectDialog,
      header,
      dialogName,
      footer,
      buttonName,
      isDisableTree,
      resultingFolderId,
      folderTitle,
      expandedKeys,
      isDisableButton,
      currentFolderId,
      selectionFiles,
      resultingFolderTree,
      operationsType,
      securityItem,
    } = this.props;
    const { displayType, isLoadingData, isAvailable } = this.state;

    const primaryButtonName = buttonName
      ? buttonName
      : t("Common:SaveHereButton");
    const name = dialogName ? dialogName : t("Common:SaveButton");

    const isUnavailableTree = isDisableTree || !isAvailable;
    const isDisableMoving =
      currentFolderId?.toString() === resultingFolderId?.toString() ||
      !securityItem.MoveTo;

    const isDisabledOperations =
      operationsType === "move" ? isDisableMoving : !securityItem.CopyTo;

    const buttonIsDisabled =
      isDisabledOperations ||
      isLoadingData ||
      isUnavailableTree ||
      isDisableButton ||
      (selectionFiles && selectionFiles[0])?.parentId === +resultingFolderId;

    return displayType === "aside" ? (
      <SelectFolderDialogAsideView
        selectionFiles={selectionFiles}
        t={t}
        isPanelVisible={isPanelVisible}
        zIndex={zIndex}
        onClose={this.onCloseAside}
        withoutProvider={withoutProvider}
        withFileSelectDialog={withFileSelectDialog}
        certainFolders={true}
        folderId={resultingFolderId}
        resultingFolderTree={resultingFolderTree}
        onSelectFolder={this.onSelect}
        onButtonClick={this.onButtonClick}
        header={header}
        dialogName={
          withFileSelectDialog ? t("Translations:FolderSelection") : name
        }
        footer={footer}
        isLoadingData={isLoadingData}
        primaryButtonName={
          withFileSelectDialog ? t("Common:SelectAction") : primaryButtonName
        }
        isAvailable={isAvailable}
        isDisableTree={isDisableTree}
        isDisableButton={buttonIsDisabled}
      />
    ) : (
      <SelectionPanel
        selectionFiles={selectionFiles}
        t={t}
        isPanelVisible={isPanelVisible}
        onClose={this.onClose}
        withoutProvider={withoutProvider}
        folderId={resultingFolderId}
        resultingFolderTree={resultingFolderTree}
        onButtonClick={this.onButtonClick}
        header={header}
        dialogName={name}
        footer={footer}
        isLoadingData={isLoadingData}
        primaryButtonName={primaryButtonName}
        isAvailable={isAvailable}
        onSelectFolder={this.onSelect}
        folderTitle={folderTitle}
        expandedKeys={expandedKeys}
        isDisableTree={isDisableTree}
        folderSelection
        newFilter={this.newFilter}
        isDisableButton={buttonIsDisabled}
      />
    );
  }
}

SelectFolderDialog.propTypes = {
  onSelectFolder: PropTypes.func,
  onClose: PropTypes.func,
  isPanelVisible: PropTypes.bool.isRequired,
  filteredType: PropTypes.oneOf([
    "exceptSortedByTags",
    "exceptPrivacyTrashArchiveFolders",
    "roomsOnly",
    "userFolderOnly",
    "",
  ]),
  displayType: PropTypes.oneOf(["aside", "modal"]),
  id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  withoutProvider: PropTypes.bool,
  withoutImmediatelyClose: PropTypes.bool,
  isDisableTree: PropTypes.bool,
  operationsType: PropTypes.oneOf(["copy", "move"]),
};
SelectFolderDialog.defaultProps = {
  id: "",
  withoutProvider: false,
  withoutImmediatelyClose: false,
  isDisableTree: false,
  filteredType: "",
};

export default inject(
  (
    {
      treeFoldersStore,
      selectedFolderStore,
      selectFolderDialogStore,
      filesStore,
      filesActionsStore,
    },
    { selectedId }
  ) => {
    const { setExpandedPanelKeys, fetchTreeFolders } = treeFoldersStore;

    const { filter } = filesStore;
    const { setSelectedItems } = filesActionsStore;

    const { id } = selectedFolderStore;
    const {
      setResultingFolderId,
      setFolderTitle,
      setProviderKey,
      providerKey,
      folderTitle,
      resultingFolderId,
      setIsLoading,
      resultingFolderTree,
      setResultingFoldersTree,
      toDefault,
      setItemSecurity,
      securityItem,
    } = selectFolderDialogStore;

    const selectedFolderId = selectedId ? selectedId : id;

    return {
      storeFolderId: selectedFolderId,
      providerKey,
      folderTitle,
      resultingFolderId,
      setExpandedPanelKeys,
      setResultingFolderId,
      setFolderTitle,
      setProviderKey,
      filter,
      setSelectedItems,
      fetchTreeFolders,
      setIsLoading,
      resultingFolderTree,
      toDefault,
      setResultingFoldersTree,
      setItemSecurity,
      securityItem,
    };
  }
)(
  observer(
    withTranslation(["SelectFolder", "Common", "Translations"])(
      SelectFolderDialog
    )
  )
);
