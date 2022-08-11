import React from "react";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import throttle from "lodash/throttle";
import SelectFolderDialogAsideView from "./AsideView";
import utils from "@appserver/components/utils";
import toastr from "studio/toastr";
import SelectionPanel from "../SelectionPanel/SelectionPanelBody";
import { FilterType } from "@appserver/common/constants";

const { desktop } = utils.device;

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
      treeFolders,
      foldersType,
      onSetBaseFolderPath,
      onSelectFolder,
      foldersList,
      displayType,
      isNeedArrowIcon = false,
      folderTree,
      setFolderId,
      withInput,
      id,
      storeFolderId,
      withoutBasicSelection = false,
    } = this.props;

    !displayType && window.addEventListener("resize", this.throttledResize);

    const initialFolderId = withInput ? id : storeFolderId;

    let resultingFolderTree, resultingId;

    if (!withInput && !isNeedArrowIcon) {
      try {
        [
          resultingFolderTree,
          resultingId,
        ] = await SelectionPanel.getBasicFolderInfo(
          treeFolders,
          foldersType,
          initialFolderId,
          onSetBaseFolderPath,
          onSelectFolder,
          foldersList
        );
      } catch (e) {
        toastr.error(e);

        return;
      }
    }

    const tree =
      isNeedArrowIcon || withInput ? folderTree : resultingFolderTree;

    if (tree.length === 0) {
      this.setState({ isAvailable: false });
      onSelectFolder(null);
      return;
    }
    const resId = isNeedArrowIcon || withInput ? id : resultingId;

    !withoutBasicSelection && onSelectFolder && onSelectFolder(resId);
    // isNeedArrowIcon && onSetBaseFolderPath(resId);

    setFolderId(resId);

    this.setState({
      resultingFolderTree: tree,
    });
  }

  componentDidUpdate(prevProps) {
    const { isReset } = this.props;

    if (isReset && isReset !== prevProps.isReset) {
      this.onResetInfo();
    }
  }

  componentWillUnmount() {
    const { setFolderTitle, setProviderKey, setFolderId } = this.props;
    //console.log("componentWillUnmount");

    if (this.throttledResize) {
      this.throttledResize && this.throttledResize.cancel();
      window.removeEventListener("resize", this.throttledResize);
    }

    setFolderTitle("");
    setProviderKey(null);
    setFolderId(null);
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
    const { setFolderId, folderId } = this.props;

    if (+folderId === +folder[0]) return;

    setFolderId(folder[0]);
  };

  onClose = () => {
    const {
      setExpandedPanelKeys,
      onClose,
      treeFolders,
      withInput,
      isNeedArrowIcon,
    } = this.props;

    if (!treeFolders.length && !withInput && !isNeedArrowIcon) {
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
      withoutImmediatelyClose,
      onSubmit,

      providerKey,
      folderTitle,
      folderId,
      setSelectedItems,
    } = this.props;

    setSelectedItems();

    onSubmit && onSubmit(folderId, folderTitle, providerKey);
    onSave && onSave(e, folderId);
    onSetNewFolderPath && onSetNewFolderPath(folderId);
    onSelectFolder && onSelectFolder(folderId);

    !withoutImmediatelyClose && this.onClose();
  };

  onResetInfo = async () => {
    const { id, setFolderId } = this.props;
    setFolderId(id);
  };

  render() {
    const {
      t,
      theme,
      isPanelVisible,
      zIndex,
      withoutProvider,
      isNeedArrowIcon, //for aside view when selected file
      header,
      dialogName,
      footer,
      buttonName,
      isDisableTree,
      folderId,
      folderTitle,
      expandedKeys,
      isDisableButton,
      isRecycleBin,
      currentFolderId,
      selectionFiles,
      selectionButtonPrimary,
    } = this.props;
    const {
      displayType,
      isLoadingData,
      isAvailable,
      resultingFolderTree,
    } = this.state;

    const primaryButtonName = buttonName
      ? buttonName
      : t("Common:SaveHereButton");
    const name = dialogName ? dialogName : t("Common:SaveButton");

    //console.log("Render Folder Component?", this.state);

    return displayType === "aside" ? (
      <SelectFolderDialogAsideView
        selectionFiles={selectionFiles}
        theme={theme}
        t={t}
        isPanelVisible={isPanelVisible}
        zIndex={zIndex}
        onClose={this.onCloseAside}
        withoutProvider={withoutProvider}
        isNeedArrowIcon={isNeedArrowIcon}
        certainFolders={true}
        folderId={folderId}
        resultingFolderTree={resultingFolderTree}
        onSelectFolder={this.onSelect}
        onButtonClick={this.onButtonClick}
        header={header}
        dialogName={isNeedArrowIcon ? t("Translations:FolderSelection") : name}
        footer={footer}
        isLoadingData={isLoadingData}
        primaryButtonName={
          isNeedArrowIcon ? t("Common:SelectAction") : primaryButtonName
        }
        isAvailable={isAvailable}
        isDisableTree={isDisableTree}
        isDisableButton={
          isDisableButton || (isRecycleBin && currentFolderId === folderId)
        }
      />
    ) : (
      <SelectionPanel
        selectionFiles={selectionFiles}
        t={t}
        theme={theme}
        isPanelVisible={isPanelVisible}
        onClose={this.onClose}
        withoutProvider={withoutProvider}
        folderId={folderId}
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
        isDisableButton={
          isDisableButton || (isRecycleBin && currentFolderId === folderId)
        }
      />
    );
  }
}

SelectFolderDialog.propTypes = {
  onSelectFolder: PropTypes.func,
  onClose: PropTypes.func,
  isPanelVisible: PropTypes.bool.isRequired,
  foldersType: PropTypes.oneOf([
    "common",
    "third-party",
    "exceptSortedByTags",
    "exceptPrivacyTrashFolders",
    "rooms",
    "",
  ]),
  displayType: PropTypes.oneOf(["aside", "modal"]),
  id: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  withoutProvider: PropTypes.bool,
  withoutImmediatelyClose: PropTypes.bool,
  isDisableTree: PropTypes.bool,
};
SelectFolderDialog.defaultProps = {
  id: "",
  withoutProvider: false,
  withoutImmediatelyClose: false,
  isDisableTree: false,
};

export default inject(
  (
    {
      treeFoldersStore,
      selectedFolderStore,
      selectFolderDialogStore,
      filesStore,
      auth,
      filesActionsStore,
    },
    { selectedId }
  ) => {
    const { treeFolders, setExpandedPanelKeys } = treeFoldersStore;

    const { filter } = filesStore;
    const { setSelectedItems } = filesActionsStore;

    const { id } = selectedFolderStore;
    const {
      setFolderId,
      setFolderTitle,
      setProviderKey,
      providerKey,
      folderTitle,
      folderId,
    } = selectFolderDialogStore;

    const { settingsStore } = auth;
    const { theme } = settingsStore;
    const selectedFolderId = selectedId ? selectedId : id;

    return {
      theme: theme,
      storeFolderId: selectedFolderId,
      providerKey,
      folderTitle,
      folderId,
      setExpandedPanelKeys,
      setFolderId,
      setFolderTitle,
      setProviderKey,
      treeFolders,
      filter,
      setSelectedItems,
    };
  }
)(
  observer(
    withTranslation(["SelectFolder", "Common", "Translations"])(
      SelectFolderDialog
    )
  )
);
