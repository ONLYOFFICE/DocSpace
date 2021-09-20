import React, { useEffect } from "react";

import ModalDialog from "@appserver/components/modal-dialog";
import { StyledAsidePanel, StyledSelectFolderPanel } from "../StyledPanels";
import FolderTreeBody from "../../FolderTreeBody";
import Button from "@appserver/components/button";

const SelectFolderDialogModalView = ({
  t,
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
}) => {
  return (
    <StyledAsidePanel visible={isPanelVisible}>
      <ModalDialog
        visible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        displayType="modal"
        {...(!header && !footer && !showButtons && { contentHeight: "416px" })}
      >
        <ModalDialog.Header>
          {headerName ? headerName : t("Translations:SelectFolder")}
        </ModalDialog.Header>

        <ModalDialog.Body>
          <StyledSelectFolderPanel isNeedArrowIcon={isNeedArrowIcon}>
            <div className="select-folder-modal-dialog-header">{header} </div>
            <FolderTreeBody
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
        <ModalDialog.Footer>
          <StyledSelectFolderPanel>
            {footer}
            {showButtons && (
              <div className="select-folder-dialog-modal_buttons">
                <Button
                  className="select-folder-dialog-buttons-save"
                  primary
                  size="medium"
                  label={t("Common:SaveButton")}
                  onClick={onSave}
                  isDisabled={isLoadingData || !isAvailable || !canCreate}
                />
                <Button
                  primary
                  size="medium"
                  label={t("Common:CloseButton")}
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
