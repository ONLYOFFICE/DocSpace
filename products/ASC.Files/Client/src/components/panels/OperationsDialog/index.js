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
class OperationsDialog extends React.PureComponent {
  constructor(props) {
    super(props);
    this.inputRef = React.createRef();
    this.state = {
      isLoading: false,
      isLoadingData: false,
      selectedInput: "",
      fullFolderPath: "",
      thirdParty: "",
      common: "",
    };
  }
  componentDidMount() {
    const { fetchTreeFolders, getCommonFolder, folderPath } = this.props;
    this.setState({ isLoading: true }, function () {
      fetchTreeFolders()
        .then(() => getCommonFolder())
        .finally(() => this.setState({ isLoading: false }));
    });

    if (folderPath.length !== 0) {
      this.getTitlesFolders(folderPath);
    }
  }

  componentDidUpdate(prevProps) {
    const { commonTreeFolder, onSelectFolder, folderPath } = this.props;

    if (
      commonTreeFolder.length !== prevProps.commonTreeFolder.length &&
      folderPath.length === 0
    ) {
      onSelectFolder([`${commonTreeFolder.id}`]);
    }
  }

  onSelect = (folder, treeNode) => {
    const { onSelectFolder, onClose } = this.props;
    this.setState({ isLoadingData: true }, function () {
      getFolderPath(folder)
        .then((res) =>
          this.setState(
            {
              selectedInput: this.inputRef.current.props.name,
            },
            function () {
              this.getTitlesFolders(res);
            }
          )
        )
        .then(() => onSelectFolder(folder))
        .then(() => onClose())
        .finally(() => this.setState({ isLoadingData: false }));
    });
  };
  getTitlesFolders = (folderPath) => {
    const { selectedInput } = this.state;

    path = "";
    if (folderPath.length > 1) {
      for (let item of folderPath) {
        if (!path) {
          path = path + `${item.title}`;
        } else path = path + " " + "/" + " " + `${item.title}`;
      }
      this.setState({
        //fullFolderPath: path,
        [selectedInput]: path,
      });
    } else {
      for (let item of folderPath) {
        path = `${item.title}`;
      }
      this.setState({
        //fullFolderPath: path,
        [selectedInput]: path,
      });
    }
  };

  render() {
    const {
      expandedKeys,
      filter,
      commonTreeFolder,

      t,
      name,
      onClickInput,
      isPanelVisible,
      folderList,
      isCommonWithoutProvider,
      onClose,
    } = this.props;
    const { isLoading, selectedInput, isLoadingData } = this.state;
    const zIndex = 310;

    return (
      <StyledComponent>
        <FileInputWithFolderPath
          name={name}
          scale
          className="input-with-folder-path"
          baseFolder={folderList ? "" : commonTreeFolder.title}
          isDisabled={isLoading}
          folderPath={this.state[selectedInput]}
          onClickInput={onClickInput}
          ref={this.inputRef}
          selectedInput={selectedInput}
        />

        {!isLoading && commonTreeFolder.length !== 0 && isPanelVisible && (
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
                    isCommonWithoutProvider={isCommonWithoutProvider}
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
      commonTreeFolder,
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
      commonTreeFolder,
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
