import React, { useEffect } from "react";

import ModalDialog from "@appserver/components/modal-dialog";
import {
  StyledAsidePanel,
  StyledSelectFolderPanel,
  StyledSelectFilePanel,
} from "../StyledPanels";
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
  onClickSave,
  header,
  footer,
  headerName,
}) => {
  console.log("header", header);
  return (
    <StyledAsidePanel visible={isPanelVisible}>
      <ModalDialog
        visible={isPanelVisible}
        zIndex={zIndex}
        onClose={onClose}
        {...(!header && !footer && { contentHeight: "400px" })}
      >
        <ModalDialog.Header>
          {headerName ? headerName : t("Translations:SelectFolder")}
        </ModalDialog.Header>

        <ModalDialog.Body>
          <StyledSelectFolderPanel isNeedArrowIcon={isNeedArrowIcon}>
            <div>{header} </div>
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
          <StyledSelectFilePanel>
            {footer}

            {/* <div className="select-file-dialog-modal_buttons">
              <Button
                className="select-file-dialog-buttons-save"
                primary
                size="medium"
                label={t("Common:SaveButton")}
                onClick={onClickSave}
                //isDisabled={selectedFile.length === 0}
              />
              <Button
                className="modal-dialog-button"
                primary
                size="medium"
                label={t("Common:CloseButton")}
                onClick={onClose}
              />
            </div> */}
          </StyledSelectFilePanel>
        </ModalDialog.Footer>
      </ModalDialog>
    </StyledAsidePanel>
  );
};
export default SelectFolderDialogModalView;
