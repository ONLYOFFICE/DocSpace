import React from "react";

import IconButton from "@appserver/components/icon-button";
import FolderTreeBody from "../../FolderTreeBody";
import { StyledAsidePanel, StyledSelectFolderPanel } from "../StyledPanels";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";

const DISPLAY_TYPE = "aside";
const SelectFolderDialogAsideView = ({
  theme,
  t,
  isPanelVisible,
  zIndex,
  onClose,
  withoutProvider,
  isNeedArrowIcon,
  asideHeightContent,
  isAvailable,
  certainFolders,
  folderId,
  isLoadingData,
  folderList,
  onSelect,
  footer,
  showButtons,
  onSave,
  headerName,
  header,
  canCreate,
  isLoading,
  primaryButtonName,
  noTreeSwitcher,
}) => {
  return (
    <StyledAsidePanel theme={theme} visible={isPanelVisible}>
      <ModalDialog
        theme={theme}
        visible={isPanelVisible}
        zIndex={zIndex}
        contentHeight="100%"
        contentPaddingBottom={footer && showButtons ? "100px" : "40px"}
        onClose={onClose}
        withoutBodyScroll
        displayType="aside"
      >
        <ModalDialog.Header theme={theme}>
          <StyledSelectFolderPanel theme={theme}>
            <div className="select-folder-dialog_header">
              {isNeedArrowIcon && (
                <IconButton
                  theme={theme}
                  className="select-folder-dialog_header-icon"
                  size="16"
                  iconName="/static/images/arrow.path.react.svg"
                  onClick={onClose}
                  // color={theme.filesPanels.selectFolder.color}
                />
              )}
              {headerName ? headerName : t("Translations:FolderSelection")}
            </div>
          </StyledSelectFolderPanel>
        </ModalDialog.Header>
        <ModalDialog.Body theme={theme}>
          <StyledSelectFolderPanel
            theme={theme}
            displayType={DISPLAY_TYPE}
            showButtons={showButtons}
            isFooter={!!footer}
            noTreeSwitcher={noTreeSwitcher}
          >
            <div className="select-folder-dialog_aside_body">
              <div>{header} </div>

              <FolderTreeBody
                theme={theme}
                isLoadingData={isLoadingData}
                folderList={folderList}
                onSelect={onSelect}
                withoutProvider={withoutProvider}
                certainFolders={certainFolders}
                isAvailable={isAvailable}
                selectedKeys={[folderId]}
                heightContent={asideHeightContent}
                displayType={DISPLAY_TYPE}
              />
            </div>
          </StyledSelectFolderPanel>
        </ModalDialog.Body>
        <ModalDialog.Footer theme={theme}>
          <StyledSelectFolderPanel theme={theme}>
            {footer}
            {showButtons && (
              <div className="select-folder-dialog-modal_buttons">
                <Button
                  theme={theme}
                  className="select-folder-dialog-buttons-save"
                  primary
                  size="normal"
                  label={primaryButtonName}
                  onClick={onSave}
                  isDisabled={isLoadingData || !isAvailable || !canCreate}
                />
                <Button
                  size="normal"
                  label={t("Common:CancelButton")}
                  onClick={onClose}
                  isDisabled={isLoadingData || isLoading}
                />
              </div>
            )}
          </StyledSelectFolderPanel>
        </ModalDialog.Footer>
      </ModalDialog>
    </StyledAsidePanel>
  );
};
export default SelectFolderDialogAsideView;
