import React from "react";
import IconButton from "@appserver/components/icon-button";
import FolderTreeBody from "../../FolderTreeBody";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import {
  StyledAsideBody,
  StyledAsideHeader,
} from "../SelectionPanel/StyledSelectionPanel";
import Text from "@appserver/components/text";
import Loaders from "@appserver/common/components/Loaders";
import styled, { css } from "styled-components";

const StyledModalDialog = styled(ModalDialog)`
  .modal-dialog-aside-body {
    padding-top: 12px;
  }
`;
const SelectFolderDialogAsideView = ({
  theme,
  t,
  isPanelVisible,
  onClose,
  withoutProvider,
  isNeedArrowIcon,
  isAvailable,
  folderId,
  isLoadingData,
  resultingFolderTree,
  onSelectFolder,
  footer,
  onButtonClick,
  dialogName,
  header,
  primaryButtonName,
  isDisableTree,
  isDisableButton,
  parentId,
  selectionFiles,
}) => {
  return (
    <StyledModalDialog
      theme={theme}
      visible={isPanelVisible}
      contentHeight="100%"
      contentPaddingBottom="0px"
      onClose={onClose}
      withoutBodyScroll
      displayType="aside"
    >
      <ModalDialog.Header theme={theme}>
        <StyledAsideHeader>
          {isNeedArrowIcon && (
            <IconButton
              theme={theme}
              className="selection-panel_aside-header-icon"
              size="16"
              iconName="/static/images/arrow.path.react.svg"
              onClick={onClose}
            />
          )}
          {dialogName ? dialogName : t("Translations:FolderSelection")}
        </StyledAsideHeader>
      </ModalDialog.Header>
      <ModalDialog.Body theme={theme}>
        <StyledAsideBody theme={theme} header={!!header} footer={!!footer}>
          <div className="selection-panel_aside-body">
            <div className="selection-panel_aside-header">
              <div>{header}</div>
              <Text fontWeight="700" fontSize="18px">
                {t("Common:Documents")}
              </Text>
            </div>

            <div className="selection-panel_aside-tree">
              {folderId && resultingFolderTree ? (
                <FolderTreeBody
                  selectionFiles={selectionFiles}
                  parentId={parentId}
                  theme={theme}
                  folderTree={resultingFolderTree}
                  onSelect={onSelectFolder}
                  withoutProvider={withoutProvider}
                  certainFolders
                  isAvailable={isAvailable}
                  selectedKeys={[`${folderId}`]}
                  isDisableTree={isDisableTree}
                  displayType="aside"
                />
              ) : (
                <div className="selection-panel_aside-loader">
                  <Loaders.NewTreeFolders />
                </div>
              )}
            </div>

            <div className="selection-panel_aside-footer">
              <div>{footer}</div>
              <div className="selection-panel_aside-buttons">
                <Button
                  theme={theme}
                  className="select-folder-dialog-buttons-save"
                  primary
                  size="normalTouchscreen"
                  label={primaryButtonName}
                  onClick={onButtonClick}
                  isDisabled={
                    isDisableButton ||
                    isDisableTree ||
                    isLoadingData ||
                    !isAvailable
                  }
                />
                <Button
                  size="normalTouchscreen"
                  label={t("Common:CancelButton")}
                  onClick={onClose}
                  isDisabled={isLoadingData}
                />
              </div>
            </div>
          </div>
        </StyledAsideBody>
      </ModalDialog.Body>
    </StyledModalDialog>
  );
};
export default SelectFolderDialogAsideView;
