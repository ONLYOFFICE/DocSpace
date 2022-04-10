import React, { useState } from "react";
import { StyledAsideBody } from "../SelectionPanel/StyledSelectionPanel";
import Text from "@appserver/components/text";
import SelectFolderInput from "../SelectFolderInput";
import FilesListBody from "../SelectionPanel/FilesListBody";
import Button from "@appserver/components/button";
import Loaders from "@appserver/common/components/Loaders";
import EmptyContainer from "../../EmptyContainer/EmptyContainer";
import ModalDialog from "@appserver/components/modal-dialog";
const DISPLAY_TYPE = "aside";
const SelectFileDialogAsideView = ({
  t,
  isPanelVisible,
  onClose,
  withoutProvider,
  onSelectFile,
  files,
  hasNextPage,
  isNextPageLoading,
  loadNextPage,
  filesListTitle,
  selectedFile,
  dialogName,
  primaryButtonName,
  theme,
  isLoading,
  onButtonClick,
  folderId,
  onSelectFolder,
  resultingFolderTree,
  isAvailable,
  footer,
  header,
  folderSelection = false,
  folderTitle,
  fileId,
  foldersType,
  onCloseSelectFolderDialog,
  onClickInput,
  isFolderPanelVisible,
}) => {
  const onMouseEvent = (event) => {
    event.stopPropagation();
  };

  return (
    <ModalDialog
      visible={isPanelVisible}
      onClose={onClose}
      contentHeight="100%"
      contentPaddingBottom="0px"
      displayType={DISPLAY_TYPE}
      withoutBodyScroll
    >
      <ModalDialog.Header>{dialogName}</ModalDialog.Header>
      <ModalDialog.Body className="select-file_body-modal-dialog">
        <StyledAsideBody theme={theme} displayType={DISPLAY_TYPE}>
          <div className="selection-panel_aside-body">
            <div className="selection-panel_folder-info">
              <SelectFolderInput
                theme={theme}
                onClickInput={onClickInput}
                onClose={onCloseSelectFolderDialog}
                onSelectFolder={onSelectFolder}
                isPanelVisible={isFolderPanelVisible}
                foldersType={foldersType}
                withoutProvider={withoutProvider}
                isSetFolderImmediately
                id={folderId}
                onSelectFile={onSelectFile}
                displayType={DISPLAY_TYPE}
                hasNextPage={hasNextPage}
                isNextPageLoading={isNextPageLoading}
                loadNextPage={loadNextPage}
                files={files}
                resultingFolderTree={resultingFolderTree}
              />

              <Text className="modal-dialog-filter-title">
                {filesListTitle}
              </Text>
            </div>
            <div className="selection-panel_files">
              <FilesListBody
                theme={theme}
                files={files}
                onSelectFile={onSelectFile}
                hasNextPage={hasNextPage}
                isNextPageLoading={isNextPageLoading}
                loadNextPage={loadNextPage}
                folderId={folderId}
                displayType={"aside"}
                folderSelection={false}
                fileId={fileId}
              />
            </div>
            <div className="selection-panel_aside-footer">
              {footer}
              <div className="selection-panel_aside-buttons">
                <Button
                  theme={theme}
                  primary
                  size="small"
                  label={primaryButtonName}
                  onClick={onButtonClick}
                  //isDisabled={selectedFile.length === 0}
                />
                <Button
                  theme={theme}
                  size="small"
                  label={t("Common:CancelButton")}
                  onClick={onClose}
                />
              </div>
            </div>
            {/* <div className="select-file-dialog_aside_body-files_list">
                {folderId ? (
                 
                ) : isAvailable ? (
                  <div key="loader" className="panel-loader-wrapper">
                    <Loaders.Rows
                      theme={theme}
                      style={{
                        marginBottom: "24px",
                        marginTop: "20px",
                      }}
                      count={12}
                    />
                  </div>
                ) : (
                  <div className="select-file-dialog_empty-container">
                    <EmptyContainer
                      theme={theme}
                      headerText={t("Home:EmptyFolderHeader")}
                      imageSrc="/static/images/empty_screen.png"
                    />
                  </div>
                )}
              </div> */}
          </div>
        </StyledAsideBody>
      </ModalDialog.Body>
    </ModalDialog>
  );
};
export default SelectFileDialogAsideView;
