import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import throttle from "lodash/throttle";

import { getCommonThirdPartyList } from "@appserver/common/api/settings";
import {
  getCommonFolderList,
  getFolderPath,
} from "@appserver/common/api/files";

import SelectFolderModal from "../SelectFolderInput";
import i18n from "../SelectFolderInput/i18n";
import SelectFolderDialogAsideView from "./asideView";
import SelectFolderDialogModalView from "./modalView";
import stores from "../../../store/index";
import utils from "@appserver/components/utils";

const { desktop } = utils.device;

let pathName = "";
let folderList;
class SelectFolderModalDialog extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoadingData: false,
      isAvailable: true,
      certainFolders: true,
      folderId: "",
      displayType: this.getDisplayType(),
    };
    this.throttledResize = throttle(this.setDisplayType, 300);
  }

  componentDidMount() {
    const {
      folderPath,
      onSelectFolder,
      onSetLoadingData,
      onSetBaseFolderPath,
      foldersType,
      isSetFolderImmediately,
    } = this.props;

    window.addEventListener("resize", this.throttledResize);

    this.setState({ isLoadingData: true }, function () {
      onSetLoadingData && onSetLoadingData(true);
      switch (foldersType) {
        case "common":
          SelectFolderDialog.getCommonFolders()
            .then((commonFolder) => {
              folderList = commonFolder;
            })
            .then(
              () =>
                folderPath.length === 0 &&
                onSelectFolder &&
                onSelectFolder(`${folderList[0].id}`)
            )
            .then(() =>
              this.setState({
                folderId: `${folderList[0].id}`,
              })
            )
            .then(() => id && setExpandedKeys([`${folderList[0].id}`]))
            .then(
              () =>
                onSetBaseFolderPath && onSetBaseFolderPath(folderList[0].title)
            )
            .finally(() => {
              onSetLoadingData && onSetLoadingData(false);

              this.setState({
                isLoadingData: false,
              });
            });
          break;
        case "third-party":
          SelectFolderDialog.getCommonThirdPartyList()
            .then(
              (commonThirdPartyArray) => (folderList = commonThirdPartyArray)
            )
            .then(
              () =>
                folderList.length === 0 && this.setState({ isAvailable: false })
            )
            .then(
              () =>
                isSetFolderImmediately &&
                folderList.length !== 0 &&
                onSelectFolder &&
                onSelectFolder(`${folderList[0].id}`)
            )
            .then(
              () =>
                isSetFolderImmediately &&
                folderList.length !== 0 &&
                this.setState({
                  folderId: `${folderList[0].id}`,
                })
            )
            .then(
              () =>
                isSetFolderImmediately &&
                folderList.length !== 0 &&
                onSetBaseFolderPath &&
                onSetBaseFolderPath(folderList[0].title)
            )
            .finally(() => {
              onSetLoadingData && onSetLoadingData(false);

              this.setState({
                isLoadingData: false,
              });
            });
          break;
      }
    });
  }
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
    const { onSelectFolder, onClose, onSetFullPath } = this.props;

    this.setState({
      folderId: folder[0],
    });

    getFolderPath(folder)
      .then(
        (foldersArray) =>
          (pathName = SelectFolderModal.setFullFolderPath(foldersArray))
      )
      .then(() => onSetFullPath && onSetFullPath(pathName))
      .then(() => onSelectFolder && onSelectFolder(folder[0]))
      .finally(() => onClose && onClose());
  };
  render() {
    const {
      t,
      isPanelVisible,
      zIndex,
      onClose,
      withoutProvider,
      isNeedArrowIcon,
      id,
      modalHeightContent,
      asideHeightContent,
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
        id={id}
        asideHeightContent={asideHeightContent}
        isAvailable={isAvailable}
        certainFolders={certainFolders}
        folderId={folderId}
        folderList={folderList}
        onSelect={this.onSelect}
      />
    ) : (
      <SelectFolderDialogModalView
        t={t}
        isPanelVisible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        withoutProvider={withoutProvider}
        id={id}
        modalHeightContent={modalHeightContent}
        isAvailable={isAvailable}
        certainFolders={certainFolders}
        folderId={folderId}
        folderList={folderList}
        onSelect={this.onSelect}
      />
    );
  }
}

SelectFolderModalDialog.propTypes = {
  onSelectFolder: PropTypes.func.isRequired,
  onClose: PropTypes.func.isRequired,
  isPanelVisible: PropTypes.bool.isRequired,
  foldersType: PropTypes.oneOf(["common", "third-party"]),
  id: PropTypes.string,
  zIndex: PropTypes.number,
  withoutProvider: PropTypes.bool,
  isNeedArrowIcon: PropTypes.bool,
  modalHeightContent: PropTypes.string,
  asideHeightContent: PropTypes.string,
};
SelectFolderModalDialog.defaultProps = {
  isSetFolderImmediately: false,
  isNeedArrowIcon: false,
  id: "",
  modalHeightContent: "325px",
  asideHeightContent: "calc(100% - 86px)",
  zIndex: 310,
  withoutProvider: false,
};

const SelectFolderDialogWrapper = withTranslation(["SelectFolder", "Common"])(
  SelectFolderModalDialog
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
    const convertFoldersArray = SelectFolderModal.setFullFolderPath(
      foldersArray
    );

    return convertFoldersArray;
  };

  render() {
    return (
      <MobxProvider {...stores}>
        <I18nextProvider i18n={i18n}>
          <SelectFolderDialogWrapper {...this.props} />
        </I18nextProvider>
      </MobxProvider>
    );
  }
}

export default SelectFolderDialog;
