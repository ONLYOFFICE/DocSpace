import React from "react";

import ModalDialog from "@appserver/components/modal-dialog";
import { StyledAsidePanel, StyledSelectFolderPanel } from "../StyledPanels";
import FolderTreeBody from "../../FolderTreeBody";
import Button from "@appserver/components/button";

const SelectFolderDialogModalView = ({
  t,
  theme,
  isPanelVisible,
  zIndex,
  onClose,
  withoutProvider,
  isNeedArrowIcon,
  modalHeightContent,
  isAvailable,
  certainFolders,
  folderId,
  isLoadingData,
  folderList,
  onSelect,
  header,
  footer,
  headerName,
  showButtons,
  onSave,
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
        onClose={onClose}
        displayType="modal"
        {...(!header && !footer && !showButtons && { contentHeight: "416px" })}
      >
        <ModalDialog.Header theme={theme}>
          {headerName ? headerName : t("Translations:FolderSelection")}
        </ModalDialog.Header>

        <ModalDialog.Body theme={theme}>
          <StyledSelectFolderPanel
            theme={theme}
            isNeedArrowIcon={isNeedArrowIcon}
            noTreeSwitcher={noTreeSwitcher}
          >
            <div className="select-folder-modal-dialog-header">{header} </div>
            <FolderTreeBody
              theme={theme}
              isLoadingData={isLoadingData}
              folderList={folderList}
              onSelect={onSelect}
              withoutProvider={withoutProvider}
              certainFolders={certainFolders}
              isAvailable={isAvailable}
              selectedKeys={[folderId]}
              heightContent={modalHeightContent}
            />
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
                  size="small"
                  label={primaryButtonName}
                  onClick={onSave}
                  isDisabled={isLoadingData || !isAvailable || !canCreate}
                />
                <Button
                  size="small"
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
export default SelectFolderDialogModalView;
