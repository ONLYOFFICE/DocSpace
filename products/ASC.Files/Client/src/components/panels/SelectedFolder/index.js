import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { getFolderPath, thirdParty } from "@appserver/common/api/files";
import TreeFolders from "../../Article/Body/TreeFolders";
import stores from "../../../store/index";
import store from "studio/store";
import ModalDialog from "@appserver/components/modal-dialog";
import { StyledAsidePanel } from "../StyledPanels";
import { useTranslation, withTranslation } from "react-i18next";
import FileInputWithFolderPath from "./fileInputWithFolderPath";
import styled from "styled-components";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import { getCommonThirdPartyList } from "@appserver/common/api/settings";

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
  }
  getFields = (obj) => {
    return Object.keys(obj).reduce((acc, rec) => {
      return [...acc, obj[rec]];
    }, []);
  };
  componentDidMount() {
    const {
      fetchTreeFolders,
      getCommonFolder,
      folderPath,
      onSelectFolder,
      onSetLoadingData,
      isThirdPartyFolders,
      isCommonFolders,
    } = this.props;

    this.setState({ isLoading: true }, function () {
      onSetLoadingData && onSetLoadingData(true);

      if (isCommonFolders) {
        //debugger;
        fetchTreeFolders()
          .then(() => getCommonFolder())
          .then((commonFolder) => (folderList = [commonFolder])) //только общие сразу вызвать
          .then(
            () =>
              folderPath.length === 0 && onSelectFolder([`${folderList[0].id}`])
          )
          .finally(() => {
            onSetLoadingData && onSetLoadingData(false);
            this.setState({
              isLoading: false,
              baseFolderPath: folderList[0].title,
            });
          });
      }

      if (isThirdPartyFolders) {
        SelectedFolderModal.getCommonThirdPartyList()
          .then((commonThirdPartyArray) => (folderList = commonThirdPartyArray))
          .then(
            () =>
              folderList.length === 0 &&
              this.setState({ isAvailableFolders: false })
          )
          .then(
            () =>
              folderPath.length !== 0 &&
              this.setState({
                baseFolderPath: folderList,
              })
          )
          .finally(() => {
            onSetLoadingData && onSetLoadingData(false);

            this.setState({
              isLoading: false,
            });
          });
      }
    });

    if (folderPath.length !== 0) {
      pathName = this.setFullFolderPath(folderPath);

      this.setState({
        fullFolderPath: pathName,
        fullFolderPathDefault: pathName,
      });
    }
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
      pathName = this.setFullFolderPath(folderPath);

      this.setState({
        baseFolderPath: pathName,
      });
    }
  }
  onSelect = (folder) => {
    const { onSelectFolder, onClose } = this.props;

    this.setState({ isLoadingData: true }, function () {
      getFolderPath(folder)
        .then(
          (foldersArray) => (pathName = this.setFullFolderPath(foldersArray))
        )
        .then(() =>
          this.setState(
            {
              fullFolderPath: pathName,
            },
            function () {
              onSelectFolder(folder);
            }
          )
        )
        .then(() => onClose())
        .finally(() => this.setState({ isLoadingData: false }));
    });
  };

  setFullFolderPath = (foldersArray) => {
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

  render() {
    const {
      expandedKeys,
      filter,

      t,
      name,
      onClickInput,
      isPanelVisible,
      //folderList,
      isCommonWithoutProvider,
      onClose,
      isError,
      withoutTopLevelFolder,
      isSavingProcess,
      isDisabled,
      //commonThirdPartyList,
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

    // console.log("folderList", folderList);

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

SelectedFolder.defaultProps = {
  isCommonWithoutProvider: false,
  withoutTopLevelFolder: false,
  folderList: "",
  folderPath: "",
  isThirdParty: false,
};
const SelectedFolderWrapper = inject(
  ({ auth, filesStore, treeFoldersStore, selectedFolderStore }) => {
    const { filter } = filesStore;
    const { setPanelVisible, panelVisible } = auth;
    const {
      getFolderPath,
      getCommonThirdPartyList,
      commonThirdPartyList,
    } = auth.settingsStore;
    const {
      fetchTreeFolders,
      expandedPanelKeys,

      getCommonFolder,
    } = treeFoldersStore;
    return {
      getFolderPath,
      setPanelVisible,
      expandedKeys: expandedPanelKeys
        ? expandedPanelKeys
        : selectedFolderStore.pathParts,
      filter,
      getCommonThirdPartyList,
      fetchTreeFolders,
      commonThirdPartyList,
      getCommonFolder,
      panelVisible,
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
