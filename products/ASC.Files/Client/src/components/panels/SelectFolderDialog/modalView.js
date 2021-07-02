import React from "react";

import ModalDialog from "@appserver/components/modal-dialog";
import { StyledAsidePanel, StyledSelectFolderPanel } from "../StyledPanels";
import FolderTreeBody from "../FolderTreeBody";

const SelectFolderDialogModalView = ({
  t,
  isPanelVisible,
  zIndex,
  onClose,
  withoutProvider,
  isNeedArrowIcon,
  id,
  modalHeightContent,
  isAvailable,
  certainFolders,
  folderId,
  isLoadingData,
  folderList,
  onSelect,
}) => {
  return (
    <StyledAsidePanel visible={isPanelVisible}>
      <ModalDialog
        visible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        contentHeight="400px"
      >
        <ModalDialog.Header>{t("ChooseFolder")}</ModalDialog.Header>

        <ModalDialog.Body>
          <StyledSelectFolderPanel isNeedArrowIcon={isNeedArrowIcon}>
            <FolderTreeBody
              isLoadingData={isLoadingData}
              folderList={folderList}
              onSelect={onSelect}
              withoutProvider={withoutProvider}
              certainFolders={certainFolders}
              isAvailable={isAvailable}
              selectedKeys={[id ? id : folderId]}
              heightContent={modalHeightContent}
            />
          </StyledSelectFolderPanel>
        </ModalDialog.Body>
      </ModalDialog>
    </StyledAsidePanel>
  );
};
export default SelectFolderDialogModalView;
