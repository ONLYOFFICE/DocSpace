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
    const { onSelectFolder } = this.props;
    const { isLoading, selectedKeys } = this.state;

    if (isArrayEqual(folder, selectedKeys)) {
      return;
    }
    this.setState({ selectedKeys: folder });

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
      onSelectFile,
      filesList,
      isLoadingData,
      hasNextPage,
      isNextPageLoading,
      loadNextPage,
      selectedFolder,
      iconUrl,
    } = this.props;
    const { isLoading, selectedKeys } = this.state;
    console.log("filesList", filesList);
    return (
      <StyledAsidePanel visible={isPanelVisible}>
        <ModalDialog
          visible={isPanelVisible}
          zIndex={zIndex}
          onClose={onClose}
          className="select-file-modal-dialog"
          style={{ maxWidth: "890px" }}
          displayType="modal"
        >
          <ModalDialog.Header>{t("SelectFile")}</ModalDialog.Header>
          <ModalDialog.Body className="select-file_body-modal-dialog">
            <StyledSelectFilePanel>
              {!isLoading ? (
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
                      selectedKeys={selectedKeys}
                    />
                  </div>
                  <div className="modal-dialog_files-body">
                    {selectedFolder && (
                      <FileListBody
                        isLoadingData={isLoadingData}
                        filesList={filesList}
                        onSelectFile={onSelectFile}
                        hasNextPage={hasNextPage}
                        isNextPageLoading={isNextPageLoading}
                        loadNextPage={loadNextPage}
                        selectedFolder={selectedFolder}
                        iconUrl={iconUrl}
                      />
                    )}
                  </div>
                </div>
              ) : (
                <div
                  key="loader"
                  className="select-file-dialog_modal-loader panel-loader-wrapper "
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
