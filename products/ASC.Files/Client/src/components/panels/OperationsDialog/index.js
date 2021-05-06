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

const { auth: authStore } = store;

let path = "";
class OperationsDialog extends React.PureComponent {
  constructor(props) {
    super(props);
    this.inputRef = React.createRef();
    this.state = {
      isLoading: false,
      selectedInput: "",
      fullFolderPath: "",
      thirdParty: "",
      common: "",
    };
  }
  componentDidMount() {
    const { fetchTreeFolders, getCommonFolder } = this.props;
    this.setState({ isLoading: true }, function () {
      fetchTreeFolders()
        .then(() => getCommonFolder())
        .finally(() => this.setState({ isLoading: false }));
    });
  }

  componentDidUpdate(prevProps) {
    const { commonTreeFolder, onSelectFolder } = this.props;

    if (commonTreeFolder.length !== prevProps.commonTreeFolder.length) {
      onSelectFolder(commonTreeFolder.id);
    }
  }

  // onClose = () => {
  //   const { setPanelVisible } = this.props;
  //   setPanelVisible(false);
  // };
  onSelect = (folder, treeNode) => {
    const { onSelectFolder, onClose } = this.props;

    getFolderPath(folder).then((res) =>
      this.setState(
        {
          selectedInput: this.inputRef.current.props.name,
        },
        function () {
          this.getTitlesFolders(res);
        }
      )
    );

    onSelectFolder(folder);

    onClose();
  };
  getTitlesFolders = (folderPath) => {
    const { selectedInput } = this.state;
    //debugger;
    //console.log("selectedInput", selectedInput);

    //debugger;
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
  // onClickInput = (e) => {
  //   const { setPanelVisible } = this.props;
  //   setPanelVisible(true);
  // };

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
    const { isLoading, selectedInput } = this.state;
    const zIndex = 310;
    // console.log("selectedInput ", selectedInput);

    return (
      <>
        <FileInputWithFolderPath
          name={name}
          id={name}
          scale
          className="folder_path"
          baseFolder={commonTreeFolder.title}
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
                <TreeFolders
                  expandedPanelKeys={expandedKeys}
                  data={folderList ? folderList : [commonTreeFolder]}
                  filter={filter}
                  onSelect={this.onSelect}
                  needUpdate={false}
                  isCommonWithoutProvider={isCommonWithoutProvider}
                />
              </ModalDialog.Body>
            </ModalDialog>
          </StyledAsidePanel>
        )}
      </>
    );
  }
}

OperationsDialog.defaultProps = {
  isCommonWithoutProvider: false,
  folderList: "",
};
const OperationsDialogWrapper = inject(
  ({ auth, filesStore, treeFoldersStore, selectedFolderStore }) => {
    const { filter } = filesStore;
    const { setPanelVisible, panelVisible } = auth;
    const { getFolderPath, folderPath } = auth.settingsStore;
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

      folderPath,
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
