import React from "react";
import { StyledAsideBody } from "../SelectionPanel/StyledSelectionPanel";
import Text from "@docspace/components/text";
import SelectFolderInput from "../SelectFolderInput";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
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
  filteredType,
  onCloseSelectFolderDialog,
  onClickInput,
  isFolderPanelVisible,
  maxInputWidth,
  newFilter,
  embedded,
}) => {
  return (
    <ModalDialog
      visible={isPanelVisible}
      onClose={onClose}
      displayType="aside"
      withoutBodyScroll
      withFooterBorder
      embedded={embedded}
    >
      {!embedded && <ModalDialog.Header>{dialogName}</ModalDialog.Header>}
      <ModalDialog.Body className="select-file_body-modal-dialog">
        <StyledAsideBody embedded={embedded} theme={theme}>
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
                filteredType={filteredType}
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
                withFileSelectDialog
                maxInputWidth={maxInputWidth ? maxInputWidth : "446px"}
                embedded={embedded}
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
          </div>
        </StyledAsideBody>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        <Button
          theme={theme}
          primary
          scale
          size="normal"
          label={primaryButtonName}
          onClick={onButtonClick}
          isDisabled={!fileId}
        />
        {onClose && (
          <Button
            theme={theme}
            scale
            size="normal"
            label={t("Common:CancelButton")}
            onClick={onClose}
          />
        )}
      </ModalDialog.Footer>
    </ModalDialog>
  );
};
export default SelectFileDialogAsideView;
