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
import store from "studio/store";

const { auth: authStore } = store;

const { desktop } = utils.device;

let pathName = "";
let folderList;

const editorExceptions = [
  FolderType.Recent,
  FolderType.TRASH,
  FolderType.Favorites,
];
class SelectFolderModalDialog extends React.Component {
  constructor(props) {
    super(props);
    const { isSetFolderImmediately, id, displayType } = this.props;
    const isNeedFolder = id ? true : isSetFolderImmediately;
    this.state = {
      isLoadingData: false,
      isAvailable: true,
      certainFolders: true,
      folderId: "",
      displayType: displayType || this.getDisplayType(),
      isSetFolderImmediately: isNeedFolder,
    };
    this.throttledResize = throttle(this.setDisplayType, 300);
    this.folderTitle = "";
  }

  componentDidMount() {
    const { onSetLoadingData, onSetLoadingInput, displayType } = this.props;

    authStore.init();

    !displayType && window.addEventListener("resize", this.throttledResize);
    this.setState({ isLoadingData: true }, function () {
      onSetLoadingData && onSetLoadingData(true);
      onSetLoadingInput && onSetLoadingInput(true);
      this.trySwitch();
    });
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
      case "editor":
        try {
          const foldersTree = await getFoldersTree();
          folderList = this.convertFolders(foldersTree, editorExceptions);
          this.setBaseSettings();
        } catch (err) {
          console.error("error", err);
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
    } = this.props;

    folderList.length === 0 && this.setState({ isAvailable: false });

    isSetFolderImmediately &&
      folderList.length !== 0 &&
      !selectedFolderId &&
      onSelectFolder &&
      onSelectFolder(
        `${selectedFolderId ? selectedFolderId : id ? id : folderList[0].id}`
      );

    isSetFolderImmediately &&
      folderList.length !== 0 &&
      this.setState({
        folderId: `${
          selectedFolderId ? selectedFolderId : id ? id : folderList[0].id
        }`,
      });

    try {
      this.folderTitle = await SelectFolderDialog.getFolderPath(
        id ? id : folderList[0].id
      );

      !id &&
        !selectedFolderId &&
        isSetFolderImmediately &&
        folderList.length !== 0 &&
        onSetBaseFolderPath &&
        onSetBaseFolderPath(this.folderTitle);
    } catch (err) {
      console.error(err);
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
    } = this.props;

    fileName && onSetFileName && onSetFileName(fileName);

    if (!id && !selectedFolderId) {
      this.loadersCompletes();
    }

    if (selectedFolderId) {
      this.setSelectedFolder(selectedFolderId);
    }

    if (id && !selectedFolderId) {
      if (!dialogWithFiles) this.setSelectedFolderToTee(id);
      else {
        this.setSelectedFolder(id);
      }
    }
  };

  setSelectedFolder = (selectedFolderId) => {
    const { onSetBaseFolderPath } = this.props;
    SelectFolderDialog.getFolderPath(selectedFolderId)
      .then((folderPath) => (this.folderTitle = folderPath))
      .then(() => onSetBaseFolderPath && onSetBaseFolderPath(this.folderTitle))

      .catch((error) => console.log("error", error))
      .finally(() => {
        this.loadersCompletes();
      });
  };

  setSelectedFolderToTee = (id) => {
    const {
      setSelectedNode,
      setSelectedFolder,
      onSetBaseFolderPath,
    } = this.props;

    setSelectedNode([id + ""]);
    SelectFolderDialog.getFolderPath(id)
      .then((folderPath) => (this.folderTitle = folderPath))
      .then(() => onSetBaseFolderPath && onSetBaseFolderPath(this.folderTitle))
      .then(() => getFolder(id))
      .then((data) => {
        const newPathParts = SelectFolderDialog.convertPathParts(
          data.pathParts
        );
        setSelectedFolder({
          folders: data.folders,
          ...data.current,
          pathParts: newPathParts,
          ...{ new: data.new },
        });
      })
      .catch((error) => console.log("error", error))
      .finally(() => {
        this.loadersCompletes();
      });
  };
  convertFolders = (folders, arrayOfExceptions) => {
    let newArray = [];

    for (let i = 0; i < folders.length; i++) {
      !arrayOfExceptions.includes(folders[i].rootFolderType) &&
        newArray.push(folders[i]);
    }
    return newArray;
  };
  componentWillUnmount() {
    if (this.throttledResize) {
      this.throttledResize && this.throttledResize.cancel();
      window.removeEventListener("resize", this.throttledResize);
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

  onSelect = (folder) => {
    const { onSelectFolder, onClose, showButtons, onSetFullPath } = this.props;

    this.setState({
      folderId: folder[0],
    });

    getFolderPath(folder)
      .then(
        (foldersArray) =>
          (pathName = SelectFolderInput.setFullFolderPath(foldersArray))
      )
      .then(() => onSetFullPath && onSetFullPath(pathName))
      .then(() => onSelectFolder && onSelectFolder(folder[0]))
      .finally(() => !showButtons && onClose && onClose());
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
    const { isAvailable, certainFolders, folderId, displayType } = this.state;

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
      />
    );
  }
}

SelectFolderModalDialog.propTypes = {
  onSelectFolder: PropTypes.func,
  onClose: PropTypes.func.isRequired,
  isPanelVisible: PropTypes.bool.isRequired,
  foldersType: PropTypes.oneOf(["common", "third-party", "editor"]),
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
  ({ treeFoldersStore, selectedFolderStore }) => {
    const { setSelectedNode } = treeFoldersStore;

    const { setSelectedFolder } = selectedFolderStore;
    return {
      setSelectedFolder,
      setSelectedNode,
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
