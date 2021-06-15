import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import { withTranslation } from "react-i18next";
import PropTypes from "prop-types";
import stores from "../../../store/index";
import { getCommonThirdPartyList } from "@appserver/common/api/settings";
import ModalDialog from "@appserver/components/modal-dialog";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import { StyledAsidePanel, StyledSelectFolderPanel } from "../StyledPanels";
import TreeFolders from "../../Article/Body/TreeFolders";
import {
  getCommonFolderList,
  getFolderPath,
} from "@appserver/common/api/files";
import IconButton from "@appserver/components/icon-button";
import SelectFolderModal from "../SelectFolderInput";
import i18n from "../SelectFolderInput/i18n";

let pathName = "";
let folderList;
class SelectFolderModalDialog extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoadingData: false,
      isAvailableFolders: true,
      certainFolders: true,
    };
  }
  componentDidMount() {
    const {
      folderPath,
      onSelectFolder,
      onSetLoadingData,
      onSetBaseFolderPath,
      foldersType,
    } = this.props;

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
                folderList.length === 0 &&
                this.setState({ isAvailableFolders: false })
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
  onSelect = (folder) => {
    const { onSelectFolder, onClose, onSetFullPath } = this.props;

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
      expandedKeys,
      filter,
      isCommonWithoutProvider,
      isNeedArrowIcon,
    } = this.props;
    const { isLoadingData, isAvailableFolders, certainFolders } = this.state;

    return (
      <StyledAsidePanel visible={isPanelVisible}>
        <ModalDialog visible={isPanelVisible} zIndex={zIndex} onClose={onClose}>
          <ModalDialog.Header>
            <StyledSelectFolderPanel isNeedArrowIcon={isNeedArrowIcon}>
              <div className="modal-dialog_header">
                {isNeedArrowIcon && (
                  <IconButton
                    size="16"
                    iconName="/static/images/arrow.path.react.svg"
                    onClick={onClose}
                    color="#A3A9AE"
                  />
                )}
                <div className="modal-dialog_header-title">
                  {t("ChooseFolder")}
                </div>
              </div>
            </StyledSelectFolderPanel>
          </ModalDialog.Header>

          <ModalDialog.Body>
            {!isLoadingData ? (
              isAvailableFolders ? (
                <TreeFolders
                  expandedPanelKeys={expandedKeys}
                  data={folderList}
                  filter={filter}
                  onSelect={this.onSelect}
                  withoutProvider={isCommonWithoutProvider}
                  certainFolders={certainFolders}
                />
              ) : (
                <Text as="span">{t("NotAvailableFolder")}</Text>
              )
            ) : (
              <div key="loader" className="panel-loader-wrapper">
                <Loader type="oval" size="16px" className="panel-loader" />
                <Text as="span">{`${t("Common:LoadingProcessing")} ${t(
                  "Common:LoadingDescription"
                )}`}</Text>
              </div>
            )}
          </ModalDialog.Body>
        </ModalDialog>
      </StyledAsidePanel>
    );
  }
}

SelectFolderModalDialog.propTypes = {
  onSelectFolder: PropTypes.func.isRequired,
  onClose: PropTypes.func.isRequired,
  isPanelVisible: PropTypes.bool.isRequired,
  foldersType: PropTypes.oneOf(["common", "third-party"]),
};
SelectFolderModalDialog.defaultProps = {
  isNeedArrowIcon: false,
};

const SelectFolderDialogWrapper = inject(
  ({ filesStore, treeFoldersStore, selectedFolderStore }) => {
    const { filter } = filesStore;
    const { expandedPanelKeys } = treeFoldersStore;
    return {
      expandedKeys: expandedPanelKeys
        ? expandedPanelKeys
        : selectedFolderStore.pathParts,
      filter,
    };
  }
)(
  observer(
    withTranslation(["SelectedFolder", "Common"])(SelectFolderModalDialog)
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
