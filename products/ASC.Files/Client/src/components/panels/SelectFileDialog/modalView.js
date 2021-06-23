import React from "react";
import { Provider as MobxProvider } from "mobx-react";

import { I18nextProvider } from "react-i18next";

import PropTypes from "prop-types";
import stores from "../../../store/index";
import i18n from "../SelectFileInput/i18n";
import { StyledAsidePanel, StyledSelectFilePanel } from "../StyledPanels";
import ModalDialog from "@appserver/components/modal-dialog";
import SelectFolderDialog from "../SelectFolderDialog";
import FolderTreeBody from "../SelectFolderDialog/folderTreeBody";
import FileListBody from "./fileListBody";
import Button from "@appserver/components/button";
class SelectFileDialogModalViewBody extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      isLoading: false,
    };
    this.backupList;
    this.convertedData = [];
    this.folderList = "";
  }

  componentDidMount() {
    const { foldersType, onSetLoadingData, onSelectFolder } = this.props;
    switch (foldersType) {
      case "common":
        SelectFolderDialog.getCommonFolders()
          .then((commonFolder) => {
            this.folderList = commonFolder;
          })
          .then(() => onSelectFolder(`${this.folderList[0].id}`))
          .finally(() => {
            onSetLoadingData && onSetLoadingData(false);

            this.setState({
              isLoading: false,
            });
          });
        break;
      case "third-party":
        SelectFolderDialog.getCommonThirdPartyList()
          .then(
            (commonThirdPartyArray) => (this.folderList = commonThirdPartyArray)
          )
          .finally(() => {
            onSetLoadingData && onSetLoadingData(false);

            this.setState({
              isLoading: false,
            });
          });
        break;
    }
  }

  onSetLoadingData = (loading) => {
    this.setState({
      isLoadingData: loading,
    });
  };
  onSelect = (folder) => {
    const { onSelectFolder } = this.props;
    onSelectFolder && onSelectFolder(folder[0]);
  };
  render() {
    const {
      t,
      isPanelVisible,
      onClose,
      zIndex,
      isCommonWithoutProvider,
      expandedKeys,
      filter,
      onFileClick,
      filesList,
      isLoadingData,
      hasNextPage,
      isNextPageLoading,
      loadNextPage,
      selectedFolder,
    } = this.props;
    const { isAvailableFolders, isLoading } = this.state;
    console.log("filesList", filesList);
    return (
      <StyledAsidePanel visible={isPanelVisible}>
        <ModalDialog
          visible={isPanelVisible}
          zIndex={zIndex}
          onClose={onClose}
          className="select-file-modal-dialog"
          style={{ maxWidth: "660px" }}
          displayType="modal"
        >
          <ModalDialog.Header>{t("SelectFile")}</ModalDialog.Header>
          <ModalDialog.Body className="select-file_body-modal-dialog">
            <StyledSelectFilePanel>
              <div className="modal-dialog_body">
                <div className="modal-dialog_tree-body">
                  <FolderTreeBody
                    expandedKeys={expandedKeys}
                    folderList={this.folderList}
                    onSelect={this.onSelect}
                    isCommonWithoutProvider={isCommonWithoutProvider}
                    certainFolders
                    isAvailableFolders
                    filter={filter}
                  />
                </div>
                <div className="modal-dialog_files-body">
                  <FileListBody
                    isLoadingData={isLoadingData}
                    filesList={filesList}
                    onFileClick={onFileClick}
                    hasNextPage={hasNextPage}
                    isNextPageLoading={isNextPageLoading}
                    loadNextPage={loadNextPage}
                    selectedFolder={selectedFolder}
                  />
                </div>
              </div>
            </StyledSelectFilePanel>
          </ModalDialog.Body>
          <ModalDialog.Footer>
            <Button
              className="modal-dialog-button"
              primary
              size="big"
              label={t("Common:CloseButton")}
              tabIndex={1}
              onClick={onClose}
            />
          </ModalDialog.Footer>
        </ModalDialog>
      </StyledAsidePanel>
    );
  }
}

class SelectFileDialogModalView extends React.Component {
  render() {
    return (
      <MobxProvider {...stores}>
        <I18nextProvider i18n={i18n}>
          <SelectFileDialogModalViewBody {...this.props} />
        </I18nextProvider>
      </MobxProvider>
    );
  }
}

export default SelectFileDialogModalView;
