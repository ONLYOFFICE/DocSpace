import React from "react";
import { StyledAsideBody } from "../SelectionPanel/StyledSelectionPanel";
import Text from "@appserver/components/text";
import SelectFolderInput from "../SelectFolderInput";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import FilesListWrapper from "../SelectionPanel/FilesListWrapper";

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
  dialogName,
  primaryButtonName,
  theme,
  onButtonClick,
  folderId,
  onSelectFolder,
  resultingFolderTree,
  footer,
  fileId,
  foldersType,
  onCloseSelectFolderDialog,
  onClickInput,
  isFolderPanelVisible,
  maxInputWidth,
  newFilter,
}) => {
  return (
    <ModalDialog
      visible={isPanelVisible}
      onClose={onClose}
      contentHeight="100%"
      contentPaddingBottom="0px"
      displayType="aside"
      withoutBodyScroll
    >
      <ModalDialog.Header>{dialogName}</ModalDialog.Header>
      <ModalDialog.Body className="select-file_body-modal-dialog">
        <StyledAsideBody theme={theme}>
          <div className="selection-panel_aside-body">
            <div className="selection-panel_folder-info">
              <Text
                className="selection-panel_folder-selection-title"
                fontWeight={600}
              >
                {t("Translations:FolderSelection")}
              </Text>

              <SelectFolderInput
                theme={theme}
                onClickInput={onClickInput}
                onClose={onCloseSelectFolderDialog}
                onSelectFolder={onSelectFolder}
                isPanelVisible={isFolderPanelVisible}
                foldersType={foldersType}
                withoutProvider={withoutProvider}
                id={folderId}
                onSelectFile={onSelectFile}
                displayType="aside"
                hasNextPage={hasNextPage}
                isNextPageLoading={isNextPageLoading}
                loadNextPage={loadNextPage}
                files={files}
                folderTree={resultingFolderTree}
                isFolderTreeLoading={!!!resultingFolderTree}
                isNeedArrowIcon
                maxInputWidth={maxInputWidth ? maxInputWidth : "446px"}
              />

              <Text color="#A3A9AE" className="selection-panel_aside-title">
                {filesListTitle}
              </Text>
            </div>
            <div className="selection-panel_files">
              <FilesListWrapper
                theme={theme}
                onSelectFile={onSelectFile}
                folderId={folderId}
                displayType="aside"
                folderSelection={false}
                fileId={fileId}
                newFilter={newFilter}
              />
            </div>
            <div className="selection-panel_aside-footer">
              {footer}
              <div className="selection-panel_aside-buttons">
                <Button
                  theme={theme}
                  primary
                  size="normalTouchscreen"
                  label={primaryButtonName}
                  onClick={onButtonClick}
                  isDisabled={!fileId}
                />
                <Button
                  theme={theme}
                  size="normalTouchscreen"
                  label={t("Common:CancelButton")}
                  onClick={onClose}
                />
              </div>
            </div>
          </div>
        </StyledAsideBody>
      </ModalDialog.Body>
    </ModalDialog>
  );
};
export default SelectFileDialogAsideView;
