import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import { inject, observer } from "mobx-react";
import { getFolderPath } from "@appserver/common/api/files";
import TreeFolders from "../../Article/Body/TreeFolders";
import stores from "../../../store/index";
import store from "studio/store";
import ModalDialog from "@appserver/components/modal-dialog";
import { StyledAsidePanel } from "../StyledPanels";
import { useTranslation, withTranslation } from "react-i18next";
import FileInputWithFolderPath from "@appserver/components/file-input-with-folder-path";
import styled from "styled-components";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
const { auth: authStore } = store;

let path = "";

const StyledComponent = styled.div`
  .input-with-folder-path {
    margin-top: 16px;
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
class OperationsDialog extends React.PureComponent {
  constructor(props) {
    super(props);
    this.inputRef = React.createRef();
    this.state = {
      isLoading: false,
      isLoadingData: false,

      thirdPartyDefault: "",
      thirdParty: "",

      commonDefault: "",
      common: "",

      baseFolder: "",
    };
  }
  componentDidMount() {
    const {
      fetchTreeFolders,
      getCommonFolder,
      folderPath,
      onSelectFolder,
      name,
      folderList,
      isCommonWithoutProvider,
      onSetLoadingData,
    } = this.props;

    this.setState({ isLoading: true }, function () {
      onSetLoadingData && onSetLoadingData(true);

      fetchTreeFolders()
        .then(() => getCommonFolder())
        .then((commonFolder) => (commonTreeFolder = commonFolder))
        .then(
          () =>
            folderPath.length === 0 &&
            isCommonWithoutProvider &&
            onSelectFolder([`${commonTreeFolder.id}`])
        )
        .finally(() => {
          onSetLoadingData && onSetLoadingData(false);
          this.setState({
            isLoading: false,
            baseFolder: folderList ? "" : commonTreeFolder.title,
          });
        });
    });

    if (folderPath.length !== 0) {
      pathName = this.getTitlesFolders(folderPath);

      this.setState({
        //fullFolderPath: path,
        [name]: pathName,
        [`${name}Default`]: pathName,
      });
    }

    console.log("DID MOunt", folderPath);
  }

  componentDidUpdate(prevProps) {
    const { isSetDefaultFolderPath, name, folderPath } = this.props;

    //debugger;
    if (
      isSetDefaultFolderPath &&
      isSetDefaultFolderPath !== prevProps.isSetDefaultFolderPath
    ) {
      this.setState({
        [name]: this.state[`${name}Default`],
      });
    }
    if (folderPath !== prevProps.folderPath) {
      pathName = this.getTitlesFolders(folderPath);

      this.setState({
        baseFolder: pathName,
      });
    }
  }
  onSelect = (folder) => {
    const { onSelectFolder, onClose, name } = this.props;

    this.setState({ isLoadingData: true }, function () {
      getFolderPath(folder)
        .then((folderPath) => (pathName = this.getTitlesFolders(folderPath)))
        .then(() =>
          this.setState(
            {
              //fullFolderPath: path,
              [name]: pathName,
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
  getTitlesFolders = (folderPath) => {
    //debugger;
    path = "";
    if (folderPath.length > 1) {
      for (let item of folderPath) {
        if (!path) {
          path = path + `${item.title}`;
        } else path = path + " " + "/" + " " + `${item.title}`;
      }
    } else {
      for (let item of folderPath) {
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
      folderList,
      isCommonWithoutProvider,
      onClose,
      isError,
    } = this.props;
    const { isLoading, isLoadingData, commonDefault, baseFolder } = this.state;
    const zIndex = 310;
    //console.log("name", name);

    console.log("commonDefault", commonDefault);

    return (
      <StyledComponent>
        <FileInputWithFolderPath
          name={name}
          scale
          className="input-with-folder-path"
          baseFolder={baseFolder}
          isDisabled={isLoading}
          folderPath={this.state[name]}
          onClickInput={onClickInput}
          hasError={isError}
        />

        {!isLoading && commonTreeFolder && isPanelVisible && (
          <StyledAsidePanel visible={isPanelVisible}>
            <ModalDialog
              visible={isPanelVisible}
              displayType="aside"
              zIndex={zIndex}
              onClose={onClose}
            >
              <ModalDialog.Header>{t("ChooseFolder")}</ModalDialog.Header>

              <ModalDialog.Body>
                {!isLoadingData ? (
                  <TreeFolders
                    expandedPanelKeys={expandedKeys}
                    data={folderList ? folderList : [commonTreeFolder]}
                    filter={filter}
                    onSelect={this.onSelect}
                    needUpdate={false}
                    withoutProvider={isCommonWithoutProvider}
                  />
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

OperationsDialog.defaultProps = {
  isCommonWithoutProvider: false,
  folderList: "",
  folderPath: "",
};
const OperationsDialogWrapper = inject(
  ({ auth, filesStore, treeFoldersStore, selectedFolderStore }) => {
    const { filter } = filesStore;
    const { setPanelVisible, panelVisible } = auth;
    const { getFolderPath } = auth.settingsStore;
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

      fetchTreeFolders,

      getCommonFolder,
      panelVisible,
    };
  }
)(observer(withTranslation("OperationsPanel")(OperationsDialog)));

class OperationsModal extends React.Component {
  render() {
    return (
      <MobxProvider auth={authStore} {...stores}>
        <OperationsDialogWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default OperationsModal;
