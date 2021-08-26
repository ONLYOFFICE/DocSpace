import React from "react";
import { inject, observer, Provider as MobxProvider } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import throttle from "lodash/throttle";

import { getCommonThirdPartyList } from "@appserver/common/api/settings";
import {
  getCommonFolderList,
  getFolder,
  getFolderPath,
  getFoldersTree,
} from "@appserver/common/api/files";

import SelectFolderInput from "../SelectFolderInput";
import i18n from "./i18n";
import SelectFolderDialogAsideView from "./AsideView";
import SelectFolderDialogModalView from "./ModalView";
import stores from "../../../store/index";
import utils from "@appserver/components/utils";
import { FolderType } from "@appserver/common/constants";
import { isArrayEqual } from "@appserver/components/utils/array";
import store from "studio/store";
import toastr from "studio/toastr";

const { auth: authStore } = store;

const { desktop } = utils.device;

let pathName = "";
let folderList;

const exceptSortedByTagsFolders = [
  FolderType.Recent,
  FolderType.TRASH,
  FolderType.Favorites,
];

const exceptTrashFolder = [FolderType.TRASH];
class SelectFolderModalDialog extends React.Component {
  constructor(props) {
    super(props);
    const { isSetFolderImmediately, id, displayType } = this.props;
    const isNeedFolder = id ? true : isSetFolderImmediately;
    this.state = {
      isLoadingData: false,
      isLoading: false,
      isAvailable: true,
      certainFolders: true,
      folderId: "",
      displayType: displayType || this.getDisplayType(),
      isSetFolderImmediately: isNeedFolder,
      canCreate: true,
    };
    this.throttledResize = throttle(this.setDisplayType, 300);
    this.folderTitle = "";
  }

  componentDidMount() {
    const { onSetLoadingData, onSetLoadingInput, displayType } = this.props;

    authStore.init(true); // it will work if authStore is not initialized

    !displayType && window.addEventListener("resize", this.throttledResize);
    this.setState({ isLoadingData: true }, function () {
      onSetLoadingData && onSetLoadingData(true);
      onSetLoadingInput && onSetLoadingInput(true);
      this.trySwitch();
    });
  }

  componentDidUpdate(prevProps) {
    const { storeFolderId, canCreate, showButtons } = this.props;
    if (showButtons && storeFolderId !== prevProps.storeFolderId) {
      this.setState({
        canCreate: canCreate,
        isLoading: false,
      });
    }
  }
  trySwitch = async () => {
    const {
      folderPath,
      onSelectFolder,
      onSetBaseFolderPath,
      foldersType,
      id,
      selectedFolderId,
    } = this.props;
    switch (foldersType) {
      case "exceptSortedByTags":
        try {
          const foldersTree = await getFoldersTree();
          folderList = SelectFolderDialog.convertFolders(
            foldersTree,
            exceptSortedByTagsFolders
          );
          this.setBaseSettings();
        } catch (err) {
          console.error("error", err);
          this.loadersCompletes();
        }
        break;
      case "exceptTrashFolder":
        try {
          const foldersTree = await getFoldersTree();
          folderList = SelectFolderDialog.convertFolders(
            foldersTree,
            exceptTrashFolder
          );
          this.setBaseSettings();
        } catch (err) {
          console.error(err);
          this.loadersCompletes();
        }
        break;
      case "common":
        try {
          folderList = await SelectFolderDialog.getCommonFolders();
          folderPath.length === 0 &&
            !selectedFolderId &&
            onSelectFolder &&
            onSelectFolder(`${id ? id : folderList[0].id}`);

          this.setState({
            folderId: `${
              selectedFolderId ? selectedFolderId : id ? id : folderList[0].id
            }`,
          });

          !id &&
            !selectedFolderId &&
            onSetBaseFolderPath &&
            onSetBaseFolderPath(folderList[0].title);

          this.setFolderInfo();
        } catch (err) {
          console.error(err);
          this.loadersCompletes();
        }

        break;

      case "third-party":
        try {
          folderList = await SelectFolderDialog.getCommonThirdPartyList();

          this.setBaseSettings();
        } catch (err) {
          console.error(err);

          this.loadersCompletes();
        }
        break;
    }
  };

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
  setBaseSettings = async () => {
    const { isSetFolderImmediately } = this.state;
    const {
      onSelectFolder,
      onSetBaseFolderPath,
      id,
      selectedFolderId,
      showButtons,
    } = this.props;

    if (folderList.length === 0) {
      this.setState({ isAvailable: false });
      onSelectFolder(null);
      this.loadersCompletes();
      return;
    }

    !id && showButtons && this.setFolderToTree(folderList[0].id);

    isSetFolderImmediately &&
      !selectedFolderId &&
      onSelectFolder &&
      onSelectFolder(
        `${selectedFolderId ? selectedFolderId : id ? id : folderList[0].id}`
      );

    isSetFolderImmediately &&
      this.setState({
        folderId: `${
          selectedFolderId ? selectedFolderId : id ? id : folderList[0].id
        }`,
      });

    if (onSetBaseFolderPath) {
      try {
        this.folderTitle = await SelectFolderDialog.getFolderPath(
          id ? id : folderList[0].id
        );

        !id &&
          !selectedFolderId &&
          isSetFolderImmediately &&
          onSetBaseFolderPath(this.folderTitle);
      } catch (err) {
        console.error(err);
      }
    }

    this.setFolderInfo();
  };

  setFolderInfo = () => {
    const {
      id,
      onSetFileName,
      fileName,
      selectedFolderId,
      dialogWithFiles,
      onSetBaseFolderPath,
    } = this.props;

    fileName && onSetFileName && onSetFileName(fileName);

    if (!id && !selectedFolderId) {
      this.loadersCompletes();
      return;
    }

    if (selectedFolderId) {
      onSetBaseFolderPath
        ? this.setBaseFolderPath(selectedFolderId)
        : this.loadersCompletes();
    }

    if (id && !selectedFolderId) {
      if (!dialogWithFiles) this.setSelectedFolder(id);
      else {
        this.setBaseFolderPath(id);
      }
    }
  };

  setBaseFolderPath = () => {
    const { onSetBaseFolderPath, selectedFolderId } = this.props;

    SelectFolderDialog.getFolderPath(selectedFolderId)
      .then((folderPath) => (this.folderTitle = folderPath))
      .then(() => onSetBaseFolderPath(this.folderTitle))
      .catch((error) => console.log("error", error))
      .finally(() => {
        this.loadersCompletes();
      });
  };
  setSelectedFolder = async (id) => {
    const { onSetBaseFolderPath } = this.props;

    let folder,
      folderPath,
      requests = [];

    requests.push(getFolder(id));

    if (onSetBaseFolderPath) {
      requests.push(getFolderPath(id));
    }

    try {
      [folder, folderPath] = await Promise.all(requests);
    } catch (e) {
      console.error(e);
    }

    folder && this.setFolderObjectToTree(id, folder);

    if (onSetBaseFolderPath && folderPath) {
      this.folderTitle = SelectFolderInput.setFullFolderPath(folderPath);
      onSetBaseFolderPath(this.folderTitle);
    }

    this.loadersCompletes();
  };

  setFolderToTree = (id) => {
    getFolder(id)
      .then((data) => {
        this.setFolderObjectToTree(id, data);
      })
      .catch((error) => console.log("error", error));
  };

  setFolderObjectToTree = (id, data) => {
    const { setSelectedNode, setSelectedFolder } = this.props;

    setSelectedNode([id + ""]);

    const newPathParts = SelectFolderDialog.convertPathParts(data.pathParts);
    setSelectedFolder({
      folders: data.folders,
      ...data.current,
      pathParts: newPathParts,
      ...{ new: data.new },
    });
  };

  componentWillUnmount() {
    const {
      setExpandedPanelKeys,
      setDefaultSelectedFolder,

      resetTreeFolders,

      dialogWithFiles,
    } = this.props;
    if (this.throttledResize) {
      this.throttledResize && this.throttledResize.cancel();
      window.removeEventListener("resize", this.throttledResize);
    }

    if (resetTreeFolders && !dialogWithFiles) {
      setExpandedPanelKeys(null);
      setDefaultSelectedFolder();
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
    const { onSelectFolder, onClose, showButtons, onSetFullPath } = this.props;
    const { folderId } = this.state;

    let requests = [];

    if (isArrayEqual([folder[0]], [folderId])) {
      return;
    }

    this.setState({
      folderId: folder[0],
    });

    let folderInfo, folderPath;

    if (showButtons) {
      this.setState({
        isLoading: true,
        canCreate: false,
      });
    }

    try {
      if (showButtons && onSetFullPath) {
        requests.push(getFolder(folder[0]), getFolderPath(folder));

        [folderInfo, folderPath] = await Promise.all(requests);
      } else {
        showButtons
          ? (folderInfo = await getFolder(folder[0]))
          : (folderPath = await getFolderPath(folder));
      }

      if (folderInfo) {
        this.setFolderObjectToTree(folder[0], folderInfo);
      }

      if (folderPath) {
        pathName = SelectFolderInput.setFullFolderPath(folderPath);
        onSetFullPath && onSetFullPath(pathName);
      }
    } catch (e) {
      console.error(e);
      toastr.error();

      if (showButtons) {
        this.setState({
          isLoading: false,
          canCreate: true,
        });

        onClose && onClose();
      }
    }

    onSelectFolder && onSelectFolder(folder[0]);
    !showButtons && onClose && onClose();

    this.loadersCompletes();
  };
  onSave = (e) => {
    const { onClose, onSave } = this.props;
    const { folderId } = this.state;

    onSave && onSave(e, folderId);
    onClose && onClose();
  };
  render() {
    const {
      t,
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
      showButtons,
    } = this.props;
    const {
      isAvailable,
      certainFolders,
      folderId,
      displayType,
      isLoadingData,
      canCreate,
      isLoading,
    } = this.state;

    return displayType === "aside" ? (
      <SelectFolderDialogAsideView
        t={t}
        isPanelVisible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        withoutProvider={withoutProvider}
        isNeedArrowIcon={isNeedArrowIcon}
        asideHeightContent={asideHeightContent}
        isAvailable={isAvailable}
        certainFolders={certainFolders}
        folderId={folderId}
        folderList={folderList}
        onSelect={this.onSelect}
        onSave={this.onSave}
        header={header}
        headerName={headerName}
        footer={footer}
        showButtons={showButtons}
        isLoadingData={isLoadingData}
        canCreate={canCreate}
        isLoading={isLoading}
      />
    ) : (
      <SelectFolderDialogModalView
        t={t}
        isPanelVisible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        withoutProvider={withoutProvider}
        modalHeightContent={modalHeightContent}
        isAvailable={isAvailable}
        certainFolders={certainFolders}
        folderId={folderId}
        folderList={folderList}
        onSelect={this.onSelect}
        onSave={this.onSave}
        header={header}
        headerName={headerName}
        footer={footer}
        showButtons={showButtons}
        canCreate={canCreate}
        isLoadingData={isLoadingData}
        isLoading={isLoading}
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
    "exceptTrashFolder",
  ]),
  displayType: PropTypes.oneOf(["aside", "modal"]),
  id: PropTypes.string,
  zIndex: PropTypes.number,
  withoutProvider: PropTypes.bool,
  isNeedArrowIcon: PropTypes.bool,
  dialogWithFiles: PropTypes.bool,
  showButtons: PropTypes.bool,
  modalHeightContent: PropTypes.string,
  asideHeightContent: PropTypes.string,
};
SelectFolderModalDialog.defaultProps = {
  isSetFolderImmediately: false,
  dialogWithFiles: false,
  isNeedArrowIcon: false,
  id: "",
  modalHeightContent: "325px",
  asideHeightContent: "100%",
  zIndex: 310,
  withoutProvider: false,
  folderPath: "",
};

const SelectFolderDialogWrapper = inject(
  ({
    treeFoldersStore,
    selectedFolderStore,
    selectedFilesStore,
    filesStore,
  }) => {
    const { setSelectedNode, setExpandedPanelKeys } = treeFoldersStore;
    const { canCreate } = filesStore;
    const {
      setSelectedFolder,
      id,
      toDefault: setDefaultSelectedFolder,
    } = selectedFolderStore;
    const { setFolderId, setFile } = selectedFilesStore;
    return {
      setSelectedFolder,
      setSelectedNode,
      canCreate,
      storeFolderId: id,
      setExpandedPanelKeys,
      setDefaultSelectedFolder,
      setFolderId,
      setFile,
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
  static convertPathParts = (pathParts) => {
    let newPathParts = [];
    for (let i = 0; i < pathParts.length - 1; i++) {
      if (typeof pathParts[i] === "number") {
        newPathParts.push(String(pathParts[i]));
      } else {
        newPathParts.push(pathParts[i]);
      }
    }
    return newPathParts;
  };
  static convertFolders = (folders, arrayOfExceptions) => {
    let newArray = [];

    for (let i = 0; i < folders.length; i++) {
      !arrayOfExceptions.includes(folders[i].rootFolderType) &&
        newArray.push(folders[i]);
    }
    return newArray;
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
