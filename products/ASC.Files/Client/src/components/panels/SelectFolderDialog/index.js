import React from "react";
import { inject, observer, Provider as MobxProvider } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import throttle from "lodash/throttle";
import { getCommonThirdPartyList } from "@appserver/common/api/settings";
import {
  getCommonFolderList,
  getFolderPath,
} from "@appserver/common/api/files";

import SelectFolderInput from "../SelectFolderInput";
import i18n from "./i18n";
import SelectFolderDialogAsideView from "./AsideView";
import SelectFolderDialogModalView from "./ModalView";
import stores from "../../../store/index";
import utils from "@appserver/components/utils";
import store from "studio/store";
import toastr from "studio/toastr";

import SelectionPanel from "../SelectionPanel/SelectionPanelBody";

const { auth: authStore } = store;

const { desktop } = utils.device;

class SelectFolderModalDialog extends React.Component {
  constructor(props) {
    super(props);
    const { id, displayType } = this.props;

    this.state = {
      isLoadingData: false,
      isInitialLoader: false,
      folderId: id ? id : "",
      displayType: displayType || this.getDisplayType(),
      canCreate: true,
      isAvailable: true,
    };
    this.throttledResize = throttle(this.setDisplayType, 300);
    this.noTreeSwitcher = false;
  }

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
      //this.loadersCompletes();
      return;
    }

    this.setState({
      resultingFolderTree: tree,
      isInitialLoader: false,
      ...((foldersType === "common" || isSetFolderImmediately) && {
        folderId: treeFromInput ? id : resultingId,
      }),
    });
  }

  componentDidUpdate(prevProps) {
    const {
      storeFolderId,
      canCreate,
      showButtons,
      selectionButtonPrimary,
      isReset,
    } = this.props;

    if (
      showButtons &&
      !selectionButtonPrimary &&
      storeFolderId !== prevProps.storeFolderId
    ) {
      this.setState({
        canCreate: canCreate,
        isLoadingData: false,
      });
    }

    if (isReset && isReset !== prevProps.isReset) {
      this.onResetInfo();
    }
  }

  loadersCompletes = () => {
    const {
      onSetLoadingData,

      onSetLoadingInput,
    } = this.props;

    onSetLoadingData && onSetLoadingData(false);
    onSetLoadingInput && onSetLoadingInput(false);

    this.setState({
      isLoadingData: false,
    });
  };

  componentWillUnmount() {
    const {
      setExpandedPanelKeys,
      resetTreeFolders,
      setSelectedFolder,
      dialogWithFiles,
    } = this.props;
    if (this.throttledResize) {
      this.throttledResize && this.throttledResize.cancel();
      window.removeEventListener("resize", this.throttledResize);
    }

    if (resetTreeFolders && !dialogWithFiles) {
      setExpandedPanelKeys(null);
      setSelectedFolder(null);
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

  onSelect = async (folder) => {
    const {
      setSelectedNode,
      setExpandedPanelKeys,
      setSelectedFolder,
    } = this.props;

    this.setState({
      folderId: folder[0],
    });

    SelectionPanel.setFolderObjectToTree(
      folder[0],
      setSelectedNode,
      setExpandedPanelKeys,
      setSelectedFolder
    );
  };

  onSave = (e) => {
    const { onClose, onSave, onSetNewFolderPath, onSelectFolder } = this.props;
    const { folderId } = this.state;

    onSave && onSave(e, folderId);
    onSetNewFolderPath && onSetNewFolderPath(folderId);
    onSelectFolder && onSelectFolder(folderId);

    onClose && onClose();
  };

  onResetInfo = async () => {
    const {
      id,
      setSelectedNode,
      setExpandedPanelKeys,
      setSelectedFolder,
    } = this.props;

    SelectionPanel.setFolderObjectToTree(
      id,
      setSelectedNode,
      setExpandedPanelKeys,
      setSelectedFolder
    );

    this.setState({
      folderId: id,
    });
  };

  render() {
    const {
      t,
      theme,
      isPanelVisible,
      zIndex,
      onClose,
      withoutProvider,
      isNeedArrowIcon,
      modalHeightContent,
      asideHeightContent,
      header,
      headerName,
      footer,
      buttonName,
    } = this.props;
    const {
      folderId,
      displayType,
      canCreate,
      isLoadingData,
      isAvailable,
      resultingFolderTree,
    } = this.state;

    const primaryButtonName = buttonName ? buttonName : t("Common:SaveButton");

    return displayType === "aside" ? (
      <SelectFolderDialogAsideView
        theme={theme}
        t={t}
        isPanelVisible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        withoutProvider={withoutProvider}
        isNeedArrowIcon={isNeedArrowIcon}
        asideHeightContent={asideHeightContent}
        certainFolders={true}
        folderId={folderId}
        resultingFolderTree={resultingFolderTree}
        onSelect={this.onSelect}
        onSave={this.onSave}
        header={header}
        headerName={headerName}
        footer={footer}
        canCreate={canCreate}
        isLoadingData={isLoadingData}
        primaryButtonName={primaryButtonName}
        noTreeSwitcher={this.noTreeSwitcher}
        isAvailable={isAvailable}
      />
    ) : (
      <SelectFolderDialogModalView
        t={t}
        theme={theme}
        isPanelVisible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        withoutProvider={withoutProvider}
        modalHeightContent={modalHeightContent}
        certainFolders={true}
        folderId={folderId}
        resultingFolderTree={resultingFolderTree}
        onSelect={this.onSelect}
        onSave={this.onSave}
        header={header}
        headerName={headerName}
        footer={footer}
        canCreate={canCreate}
        isLoadingData={isLoadingData}
        primaryButtonName={primaryButtonName}
        noTreeSwitcher={this.noTreeSwitcher}
        isAvailable={isAvailable}
      />
    );
  }
}

SelectFolderModalDialog.propTypes = {
  onSelectFolder: PropTypes.func,
  onClose: PropTypes.func.isRequired,
  isPanelVisible: PropTypes.bool.isRequired,
  foldersType: PropTypes.oneOf([
    "common",
    "third-party",
    "exceptSortedByTags",
    "exceptPrivacyTrashFolders",
  ]),
  displayType: PropTypes.oneOf(["aside", "modal"]),
  id: PropTypes.string,
  zIndex: PropTypes.number,
  withoutProvider: PropTypes.bool,
  isNeedArrowIcon: PropTypes.bool,
  dialogWithFiles: PropTypes.bool,
  showButtons: PropTypes.bool,
  selectionButtonPrimary: PropTypes.bool,
  modalHeightContent: PropTypes.string,
  asideHeightContent: PropTypes.string,
};
SelectFolderModalDialog.defaultProps = {
  isSetFolderImmediately: false,
  dialogWithFiles: false,
  isNeedArrowIcon: false,
  id: "",
  modalHeightContent: "291px",
  asideHeightContent: "100%",
  zIndex: 310,
  withoutProvider: false,
  folderPath: "",
  showButtons: false,
  selectionButtonPrimary: false,
};

const SelectFolderDialogWrapper = inject(
  ({
    treeFoldersStore,
    selectedFolderStore,
    selectedFilesStore,
    filesStore,
    auth,
  }) => {
    const {
      setSelectedNode,
      setExpandedPanelKeys,
      treeFolders,
    } = treeFoldersStore;
    const { canCreate } = filesStore;
    const { setSelectedFolder, id } = selectedFolderStore;
    const { setFolderId, setFile } = selectedFilesStore;
    return {
      theme: auth.settingsStore.theme,
      setSelectedFolder,
      setSelectedNode,
      canCreate,
      storeFolderId: id,
      setExpandedPanelKeys,
      setFolderId,
      setFile,
      treeFolders,
    };
  }
)(
  observer(
    withTranslation(["SelectFolder", "Common", "Translations"])(
      SelectFolderModalDialog
    )
  )
);

class SelectFolderDialog extends React.Component {
  static getCommonThirdPartyList = async () => {
    const commonThirdPartyArray = await getCommonThirdPartyList();

    commonThirdPartyArray.map((currentValue, index) => {
      commonThirdPartyArray[index].key = `0-${index}`;
    });

    return commonThirdPartyArray;
  };

  static getCommonFolders = async () => {
    const commonFolders = await getCommonFolderList();

    const convertedData = {
      id: commonFolders.current.id,
      key: 0 - 1,
      parentId: commonFolders.current.parentId,
      title: commonFolders.current.title,
      rootFolderType: +commonFolders.current.rootFolderType,
      rootFolderName: "@common",
      folders: commonFolders.folders.map((folder) => {
        return {
          id: folder.id,
          title: folder.title,
          access: folder.access,
          foldersCount: folder.foldersCount,
          rootFolderType: folder.rootFolderType,
          providerKey: folder.providerKey,
          newItems: folder.new,
        };
      }),
      pathParts: commonFolders.pathParts,
      foldersCount: commonFolders.current.foldersCount,
      newItems: commonFolders.new,
    };

    return [convertedData];
  };

  static getFolderPath = async (folderId) => {
    const foldersArray = await getFolderPath(folderId);
    const convertFoldersArray = SelectFolderInput.setFullFolderPath(
      foldersArray
    );

    return convertFoldersArray;
  };

  render() {
    return (
      <MobxProvider auth={authStore} {...stores}>
        <I18nextProvider i18n={i18n}>
          <SelectFolderDialogWrapper {...this.props} />
        </I18nextProvider>
      </MobxProvider>
    );
  }
}

export default SelectFolderDialog;
