import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { I18nextProvider } from "react-i18next";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import PropTypes from "prop-types";

import store from "studio/store";
import {
  getCommonFolderList,
  getFolderPath,
} from "@appserver/common/api/files";
import { getCommonThirdPartyList } from "@appserver/common/api/settings";
import ModalDialog from "@appserver/components/modal-dialog";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";

import i18n from "./i18n";
import TreeFolders from "../../Article/Body/TreeFolders";
import stores from "../../../store/index";
import { StyledAsidePanel } from "../StyledPanels";
import FileInputWithFolderPath from "./fileInputWithFolderPath";

const { auth: authStore } = store;

let path = "";

const StyledComponent = styled.div`
  .input-with-folder-path {
    margin-top: 16px;
  }
  .input-with-folder-path,
  .text-input-with-folder-path {
    width: 100%;
    max-width: 820px;
  }
  .panel-loader-wrapper {
    margin-top: 8px;
    padding-left: 32px;
  }
  .panel-loader {
    display: inline;
    margin-right: 10px;
  }
`;
let commonTreeFolder;
let pathName = "";
let folderList;
class SelectedFolder extends React.PureComponent {
  static setFullFolderPath = (foldersArray) => {
    path = "";
    if (foldersArray.length > 1) {
      for (let item of foldersArray) {
        if (!path) {
          path = path + `${item.title}`;
        } else path = path + " " + "/" + " " + `${item.title}`;
      }
    } else {
      for (let item of foldersArray) {
        path = `${item.title}`;
      }
    }
    return path;
  };
  constructor(props) {
    super(props);
    this.inputRef = React.createRef();
    this.state = {
      isLoading: false,
      isLoadingData: false,
      baseFolderPath: "",
      fullFolderPath: "",
      fullFolderPathDefault: "",
      isAvailableFolders: true,
    };
    this._isMounted = false;
  }
  componentDidMount() {
    const {
      fetchTreeFolders,
      folderPath,
      onSelectFolder,
      onSetLoadingData,
      isThirdPartyFolders,
      isCommonFolders,
    } = this.props;
    this._isMounted = true;
    this._isMounted &&
      this.setState({ isLoading: true }, function () {
        onSetLoadingData && onSetLoadingData(true);

        if (isCommonFolders) {
          SelectedFolderModal.getCommonFolders()
            .then((commonFolder) => {
              folderList = commonFolder;
            })
            .then(
              () =>
                folderPath.length === 0 &&
                onSelectFolder &&
                onSelectFolder([`${folderList[0].id}`])
            )
            .finally(() => {
              onSetLoadingData && onSetLoadingData(false);
              this._isMounted &&
                this.setState({
                  isLoading: false,
                  baseFolderPath: folderList[0].title,
                });
            });
        }

        if (isThirdPartyFolders) {
          SelectedFolderModal.getCommonThirdPartyList()
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

              this._isMounted &&
                this.setState({
                  isLoading: false,
                });
            });
        }
      });

    if (folderPath.length !== 0) {
      this._isMounted &&
        this.setState({
          fullFolderPath: folderPath,
          fullFolderPathDefault: folderPath,
        });
    }
  }

  componentWillUnmount() {
    this._isMounted = false;
  }
  componentDidUpdate(prevProps) {
    const { isSetDefaultFolderPath, folderPath } = this.props;

    if (
      isSetDefaultFolderPath &&
      isSetDefaultFolderPath !== prevProps.isSetDefaultFolderPath
    ) {
      this.setState({
        fullFolderPath: this.state.fullFolderPathDefault,
      });
    }
    if (folderPath !== prevProps.folderPath) {
      this.setState({
        fullFolderPath: folderPath,
        fullFolderPathDefault: folderPath,
      });
    }
  }
  onSelect = (folder) => {
    const { onSelectFolder, onClose } = this.props;

    this.setState({ isLoadingData: true }, function () {
      getFolderPath(folder)
        .then(
          (foldersArray) =>
            (pathName = SelectedFolder.setFullFolderPath(foldersArray))
        )
        .then(() =>
          this.setState(
            {
              fullFolderPath: pathName,
            },
            function () {
              onSelectFolder && onSelectFolder(folder);
            }
          )
        )
        .then(() => onClose && onClose())
        .finally(() => this.setState({ isLoadingData: false }));
    });
  };

  render() {
    const {
      expandedKeys,
      filter,

      t,
      name,
      onClickInput,
      isPanelVisible,
      isCommonWithoutProvider,
      onClose,
      isError,
      withoutTopLevelFolder,
      isSavingProcess,
      isDisabled,
    } = this.props;
    const {
      isLoading,
      isLoadingData,
      baseFolderPath,
      fullFolderPath,
      isAvailableFolders,
    } = this.state;
    const zIndex = 310;
    //console.log("name", name);

    return (
      <StyledComponent>
        <FileInputWithFolderPath
          name={name}
          className="input-with-folder-path"
          baseFolderPath={baseFolderPath}
          folderPath={fullFolderPath}
          isDisabled={isLoading || isSavingProcess || isDisabled}
          isError={isError}
          onClickInput={onClickInput}
        />

        {!isLoading && isPanelVisible && (
          <StyledAsidePanel visible={isPanelVisible}>
            <ModalDialog
              visible={isPanelVisible}
              zIndex={zIndex}
              onClose={onClose}
            >
              <ModalDialog.Header>{t("ChooseFolder")}</ModalDialog.Header>

              <ModalDialog.Body>
                {!isLoadingData ? (
                  isAvailableFolders ? (
                    <TreeFolders
                      expandedPanelKeys={expandedKeys}
                      data={folderList}
                      filter={filter}
                      onSelect={this.onSelect}
                      needUpdate={false}
                      withoutProvider={isCommonWithoutProvider}
                      withoutTopLevelFolder={withoutTopLevelFolder}
                    />
                  ) : (
                    <Text as="span">{t("NotAvailableFolder")}</Text>
                  )
                ) : (
                  <div key="loader" className="panel-loader-wrapper">
                    <Loader type="oval" size="16px" className="panel-loader" />
                    <Text as="span">{t("LoadingLabel")}</Text>
                  </div>
                )}
              </ModalDialog.Body>
            </ModalDialog>
          </StyledAsidePanel>
        )}
      </StyledComponent>
    );
  }
}

SelectedFolder.propTypes = {
  onClickInput: PropTypes.func.isRequired,
  onSelectFolder: PropTypes.func.isRequired,
  onClose: PropTypes.func.isRequired,
};

SelectedFolder.defaultProps = {
  isThirdParty: false,
  isCommonWithoutProvider: false,
  withoutTopLevelFolder: false,
  folderList: "",
  folderPath: "",
};
const SelectedFolderWrapper = inject(
  ({ filesStore, treeFoldersStore, selectedFolderStore }) => {
    const { filter } = filesStore;
    const { fetchTreeFolders, expandedPanelKeys } = treeFoldersStore;
    return {
      expandedKeys: expandedPanelKeys
        ? expandedPanelKeys
        : selectedFolderStore.pathParts,
      filter,
      fetchTreeFolders,
    };
  }
)(observer(withTranslation("SelectedFolder")(SelectedFolder)));

class SelectedFolderModal extends React.Component {
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
    const convertFoldersArray = SelectedFolder.setFullFolderPath(foldersArray);

    return convertFoldersArray;
  };
  render() {
    return (
      <MobxProvider auth={authStore} {...stores}>
        <I18nextProvider i18n={i18n}>
          <SelectedFolderWrapper {...this.props} />
        </I18nextProvider>
      </MobxProvider>
    );
  }
}

export default SelectedFolderModal;
