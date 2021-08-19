import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import stores from "../../../store/index";
import { StyledAsidePanel, StyledSelectFilePanel } from "../StyledPanels";
import ModalDialog from "@appserver/components/modal-dialog";
import SelectFolderDialog from "../SelectFolderDialog";
import FolderTreeBody from "../../FolderTreeBody";
import FilesListBody from "./FilesListBody";
import Button from "@appserver/components/button";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import { isArrayEqual } from "@appserver/components/utils/array";
import { FolderType } from "@appserver/common/constants";
import { getFoldersTree } from "@appserver/common/api/files";

const exceptSortedByTagsFolders = [
  FolderType.Recent,
  FolderType.TRASH,
  FolderType.Favorites,
];

const exceptTrashFolder = [FolderType.TRASH];
class SelectFileDialogModalView extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: true,
      isAvailable: true,
    };
    this.folderList = "";
  }

  componentDidMount() {
    const { onSetLoadingData } = this.props;
    this.setState({ isLoadingData: true }, function () {
      onSetLoadingData && onSetLoadingData(true);

      this.trySwitch();
    });
  }
  trySwitch = async () => {
    const {
      foldersType,
      onSelectFolder,
      selectedFolder,
      passedId,
    } = this.props;
    switch (foldersType) {
      case "exceptSortedByTags":
        try {
          const foldersTree = await getFoldersTree();
          this.folderList = SelectFolderDialog.convertFolders(
            foldersTree,
            exceptSortedByTagsFolders
          );
          this.onSetSelectedFolder();
        } catch (err) {
          console.error(err);
        }

        this.loadersCompletes();
        break;
      case "exceptTrashFolder":
        try {
          const foldersTree = await getFoldersTree();
          this.folderList = SelectFolderDialog.convertFolders(
            foldersTree,
            exceptTrashFolder
          );
          this.onSetSelectedFolder();
        } catch (err) {
          console.error(err);
        }

        this.loadersCompletes();
        break;
      case "common":
        try {
          this.folderList = await SelectFolderDialog.getCommonFolders();

          !selectedFolder &&
            onSelectFolder &&
            onSelectFolder(
              `${
                selectedFolder
                  ? selectedFolder
                  : passedId
                  ? passedId
                  : this.folderList[0].id
              }`
            );
        } catch (err) {
          console.error(err);
        }

        this.loadersCompletes();
        break;
      case "third-party":
        try {
          this.folderList = await SelectFolderDialog.getCommonThirdPartyList();
          this.folderList.length !== 0
            ? this.onSetSelectedFolder()
            : this.setState({ isAvailable: false });
        } catch (err) {
          console.error(err);
        }

        this.loadersCompletes();
        break;
    }
  };

  loadersCompletes = () => {
    const { onSetLoadingData } = this.props;
    onSetLoadingData && onSetLoadingData(false);

    this.setState({
      isLoading: false,
    });
  };

  onSetSelectedFolder = () => {
    const { onSelectFolder, selectedFolder, passedId } = this.props;

    onSelectFolder &&
      onSelectFolder(
        `${
          selectedFolder
            ? selectedFolder
            : passedId
            ? passedId
            : this.folderList[0].id
        }`
      );
  };
  onSelect = (folder) => {
    const { onSelectFolder, selectedFolder } = this.props;

    if (isArrayEqual([folder[0]], [selectedFolder])) {
      return;
    }

    onSelectFolder && onSelectFolder(folder[0]);
  };
  render() {
    const {
      t,
      isPanelVisible,
      onClose,
      zIndex,
      withoutProvider,
      expandedKeys,
      filter,
      onSelectFile,
      filesList,
      hasNextPage,
      isNextPageLoading,
      loadNextPage,
      selectedFolder,
      header,
      loadingText,
      selectedFile,
      onClickSave,
      headerName,
    } = this.props;

    const { isLoading, isAvailable } = this.state;

    const isHeaderChildren = !!header;

    return (
      <StyledAsidePanel visible={isPanelVisible}>
        <ModalDialog
          visible={isPanelVisible}
          zIndex={zIndex}
          onClose={onClose}
          className="select-file-modal-dialog"
          style={{ maxWidth: "890px" }}
          displayType="modal"
          bodyPadding="0"
        >
          <ModalDialog.Header>
            {headerName ? headerName : t("SelectFile")}
          </ModalDialog.Header>
          <ModalDialog.Body className="select-file_body-modal-dialog">
            <StyledSelectFilePanel isHeaderChildren={isHeaderChildren}>
              {!isLoading ? (
                <div className="modal-dialog_body">
                  <div className="modal-dialog_children">{header}</div>
                  <div className="modal-dialog_tree-body">
                    <FolderTreeBody
                      expandedKeys={expandedKeys}
                      folderList={this.folderList}
                      onSelect={this.onSelect}
                      withoutProvider={withoutProvider}
                      certainFolders
                      isAvailable={isAvailable}
                      filter={filter}
                      selectedKeys={[selectedFolder]}
                      isHeaderChildren={isHeaderChildren}
                    />
                  </div>
                  <div className="modal-dialog_files-body">
                    {selectedFolder && (
                      <FilesListBody
                        filesList={filesList}
                        onSelectFile={onSelectFile}
                        hasNextPage={hasNextPage}
                        isNextPageLoading={isNextPageLoading}
                        loadNextPage={loadNextPage}
                        selectedFolder={selectedFolder}
                        loadingText={loadingText}
                        selectedFile={selectedFile}
                        listHeight={isHeaderChildren ? 280 : 310}
                      />
                    )}
                  </div>
                </div>
              ) : (
                <div
                  key="loader"
                  className="select-file-dialog_modal-loader panel-loader-wrapper"
                >
                  <Loader type="oval" size="16px" className="panel-loader" />
                  <Text as="span">{`${t("Common:LoadingProcessing")} ${t(
                    "Common:LoadingDescription"
                  )}`}</Text>
                </div>
              )}
            </StyledSelectFilePanel>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <StyledSelectFilePanel isHeaderChildren={isHeaderChildren}>
              <div className="select-file-dialog-modal_buttons">
                <Button
                  className="select-file-modal-dialog-buttons-save"
                  primary
                  size="medium"
                  label={t("Common:SaveButton")}
                  onClick={onClickSave}
                  isDisabled={selectedFile.length === 0}
                />
                <Button
                  className="modal-dialog-button"
                  primary
                  size="medium"
                  label={t("Common:CloseButton")}
                  onClick={onClose}
                />
              </div>
            </StyledSelectFilePanel>
          </ModalDialog.Footer>
        </ModalDialog>
      </StyledAsidePanel>
    );
  }
}

export default SelectFileDialogModalView;
