import React, { useState } from "react";
import { StyledAsidePanel, StyledSelectFilePanel } from "../StyledPanels";
import Text from "@appserver/components/text";
import SelectFolderInput from "../SelectFolderInput";
import FilesListBody from "./FilesListBody";
import Button from "@appserver/components/button";
import Loaders from "@appserver/common/components/Loaders";
import EmptyContainer from "../../EmptyContainer/EmptyContainer";
import ModalDialog from "@appserver/components/modal-dialog";
const DISPLAY_TYPE = "aside";
const SelectFileDialogAsideView = ({
  t,
  theme,
  isPanelVisible,
  zIndex,
  onClose,
  isVisible,
  withoutProvider,
  foldersType,
  onSelectFile,
  onClickInput,
  onCloseSelectFolderDialog,
  onSelectFolder,
  filesList,
  hasNextPage,
  isNextPageLoading,
  loadNextPage,
  selectedFolder,
  titleFilesList,
  loadingText,
  selectedFile,
  onClickSave,
  onSetFileName,
  fileName,
  displayType,
  isTranslationsReady,
  passedId,
  headerName,
  isAvailableFolderList,
  primaryButtonName,
}) => {
  const [isLoadingData, setIsLoadingData] = useState(false);
  const onSetLoadingData = (loading) => {
    setIsLoadingData(loading);
  };
  const isHeaderChildren = !!titleFilesList;

  const onMouseEvent = (event) => {
    event.stopPropagation();
  };

  return (
    <StyledAsidePanel
      visible={isPanelVisible}
      onMouseUp={onMouseEvent}
      onMouseDown={onMouseEvent}
    >
      <ModalDialog
        visible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        contentHeight="100%"
        displayType={DISPLAY_TYPE}
        withoutBodyScroll
      >
        <ModalDialog.Header>
          {headerName ? headerName : t("SelectFile")}
        </ModalDialog.Header>
        <ModalDialog.Body className="select-file_body-modal-dialog">
          <StyledSelectFilePanel
            theme={theme}
            isHeaderChildren={isHeaderChildren}
            displayType={DISPLAY_TYPE}
          >
            <div className="select-file-dialog_aside-body_wrapper">
              <div className="select-file-dialog_aside-children"></div>
              <div className="select-file-dialog_aside_body">
                <SelectFolderInput
                  theme={theme}
                  onClickInput={onClickInput}
                  onClose={onCloseSelectFolderDialog}
                  onSelectFolder={onSelectFolder}
                  onSetLoadingData={onSetLoadingData}
                  isPanelVisible={isVisible}
                  foldersType={foldersType}
                  isNeedArrowIcon
                  withoutProvider={withoutProvider}
                  isSetFolderImmediately
                  selectedFolderId={selectedFolder}
                  id={passedId}
                  onSetFileName={onSetFileName}
                  fileName={fileName}
                  displayType={displayType}
                  dialogWithFiles
                  showButtons
                  selectionButtonPrimary
                />
                {titleFilesList && (
                  <Text className="modal-dialog-filter-title">
                    {titleFilesList}
                  </Text>
                )}
                <div className="select-file-dialog_aside_body-files_list">
                  {selectedFolder && !isLoadingData ? (
                    <FilesListBody
                      theme={theme}
                      filesList={filesList}
                      onSelectFile={onSelectFile}
                      hasNextPage={hasNextPage}
                      isNextPageLoading={isNextPageLoading}
                      loadNextPage={loadNextPage}
                      selectedFolder={selectedFolder}
                      displayType={DISPLAY_TYPE}
                      loadingText={loadingText}
                      selectedFile={selectedFile}
                    />
                  ) : isAvailableFolderList ? (
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
                </div>
              </div>
            </div>
          </StyledSelectFilePanel>
        </ModalDialog.Body>
        <ModalDialog.Footer theme={theme}>
          <StyledSelectFilePanel
            theme={theme}
            isHeaderChildren={isHeaderChildren}
          >
            <div className="select-file-dialog-aside_buttons">
              <Button
                theme={theme}
                className="select-file-dialog-buttons-save"
                primary
                size="normal"
                label={primaryButtonName}
                onClick={onClickSave}
                isDisabled={selectedFile.length === 0}
              />
              <Button
                size="normal"
                theme={theme}
                label={t("Common:CancelButton")}
                onClick={onClose}
              />
            </div>
          </StyledSelectFilePanel>
        </ModalDialog.Footer>
      </ModalDialog>
    </StyledAsidePanel>
  );
};
export default SelectFileDialogAsideView;
