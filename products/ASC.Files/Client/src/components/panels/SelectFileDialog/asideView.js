import React from "react";
import { StyledAsidePanel, StyledSelectFilePanel } from "../StyledPanels";
import Text from "@appserver/components/text";
import ModalDialog from "@appserver/components/modal-dialog";
import SelectFolderInput from "../SelectFolderInput";
import FileListBody from "./fileListBody";
const SelectFileDialogAsideView = ({
  t,
  isPanelVisible,
  zIndex,
  onClose,
  isVisible,
  isCommonWithoutProvider,
  foldersType,
  isLoadingData,
  onFileClick,
  onClickInput,
  onCloseSelectFolderDialog,
  onSelectFolder,
  onSetLoadingData,
  filesList,
}) => (
  <StyledAsidePanel visible={isPanelVisible}>
    <ModalDialog
      visible={isPanelVisible}
      zIndex={zIndex}
      onClose={onClose}
      //displayType="aside"
    >
      <ModalDialog.Header>{t("SelectFile")}</ModalDialog.Header>
      <ModalDialog.Body>
        <StyledSelectFilePanel>
          <Text fontWeight="600" fontSize="14px">
            {t("ChooseByUser")}
          </Text>
          <SelectFolderInput
            onClickInput={onClickInput}
            onClose={onCloseSelectFolderDialog}
            onSelectFolder={onSelectFolder}
            onSetLoadingData={onSetLoadingData}
            isPanelVisible={isVisible}
            foldersType={foldersType}
            isNeedArrowIcon
            isCommonWithoutProvider={isCommonWithoutProvider}
          />
          <div className="modal-dialog_body-files-list">
            <FileListBody
              isLoadingData={isLoadingData}
              filesList={filesList}
              onFileClick={onFileClick}
            />
          </div>
        </StyledSelectFilePanel>
      </ModalDialog.Body>
    </ModalDialog>
  </StyledAsidePanel>
);
export default SelectFileDialogAsideView;
