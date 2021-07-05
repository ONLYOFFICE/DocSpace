import React from "react";
import { Provider as MobxProvider } from "mobx-react";

import { I18nextProvider } from "react-i18next";

import PropTypes from "prop-types";
import stores from "../../../store/index";
import i18n from "../SelectFileInput/i18n";
import { StyledAsidePanel, StyledSelectFilePanel } from "../StyledPanels";
import ModalDialog from "@appserver/components/modal-dialog";
import SelectFolderDialog from "../SelectFolderDialog";
import FolderTreeBody from "../FolderTreeBody";
import FilesListBody from "./filesListBody";
import Button from "@appserver/components/button";
import Loader from "@appserver/components/loader";
import Text from "@appserver/components/text";
import { isArrayEqual } from "@appserver/components/utils/array";
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
        this.setState({ isLoading: true }, function () {
          SelectFolderDialog.getCommonFolders()
            .then((commonFolder) => {
              this.folderList = commonFolder;
            })
            .then(
              () => onSelectFolder && onSelectFolder(`${this.folderList[0].id}`)
            )
            .finally(() => {
              onSetLoadingData && onSetLoadingData(false);

              this.setState({
                isLoading: false,
              });
            });
        });

        break;
      case "third-party":
        this.setState({ isLoading: true }, function () {
          SelectFolderDialog.getCommonThirdPartyList()
            .then(
              (commonThirdPartyArray) =>
                (this.folderList = commonThirdPartyArray)
            )
            .then(
              () => onSelectFolder && onSelectFolder(`${this.folderList[0].id}`)
            )
            .finally(() => {
              onSetLoadingData && onSetLoadingData(false);

              this.setState({
                isLoading: false,
              });
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
    const { onSelectFolder, selectedKeys } = this.props;

    if (isArrayEqual(folder, selectedKeys)) {
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
      selectedKeys,
      folderId,
      header,
      modalHeightContent,
      loadingText,
      selectedFile,
      onClickSave,
    } = this.props;
    const { isLoading } = this.state;
    console.log("this.folderList", this.folderList);
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
            {header ? header : t("SelectFile")}
          </ModalDialog.Header>
          <ModalDialog.Body className="select-file_body-modal-dialog">
            <StyledSelectFilePanel>
              {!isLoading ? (
                <div className="modal-dialog_body">
                  <div className="modal-dialog_tree-body">
                    <FolderTreeBody
                      expandedKeys={expandedKeys}
                      folderList={this.folderList}
                      onSelect={this.onSelect}
                      withoutProvider={withoutProvider}
                      certainFolders
                      isAvailableFolders
                      filter={filter}
                      selectedKeys={[folderId ? folderId : selectedKeys]}
                      heightContent={modalHeightContent}
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
            <StyledSelectFilePanel>
              <div className="select-file-dialog-modal_buttons">
                <Button
                  className="select-file-dialog-buttons-save"
                  primary
                  size="medium"
                  label={t("Common:SaveButton")}
                  onClick={onClickSave}
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

SelectFileDialogModalViewBody.propTypes = {
  modalHeightContent: PropTypes.string,
};
SelectFileDialogModalViewBody.defaultProps = {
  modalHeightContent: "280px",
};
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
