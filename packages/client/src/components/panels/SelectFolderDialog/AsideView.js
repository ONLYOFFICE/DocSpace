import ArrowPathReactSvgUrl from "PUBLIC_DIR/images/arrow.path.react.svg?url";
import React from "react";
import IconButton from "@docspace/components/icon-button";
import FolderTreeBody from "../../FolderTreeBody";
import Button from "@docspace/components/button";
import ModalDialog from "@docspace/components/modal-dialog";
import {
  StyledAsideBody,
  StyledAsideHeader,
} from "../SelectionPanel/StyledSelectionPanel";
import Text from "@docspace/components/text";
import Loaders from "@docspace/common/components/Loaders";
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
  withFileSelectDialog,
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
  const isLoaded = folderId && resultingFolderTree;
  return (
    <StyledModalDialog
      theme={theme}
      visible={isPanelVisible}
      onClose={onClose}
      withoutBodyScroll
      withFooterBorder
      displayType="aside"
      isDoubleFooterLine
    >
      <ModalDialog.Header theme={theme}>
        <StyledAsideHeader>
          {withFileSelectDialog && (
            <IconButton
              theme={theme}
              className="selection-panel_aside-header-icon"
              size="16"
              iconName={ArrowPathReactSvgUrl}
              onClick={onClose}
            />
          )}
          {dialogName}
        </StyledAsideHeader>
      </ModalDialog.Header>
      <ModalDialog.Body theme={theme}>
        <StyledAsideBody theme={theme} header={!!header} footer={!!footer}>
          <div className="selection-panel_aside-body">
            <div className="selection-panel_aside-header">
              <div>{header}</div>
              {isLoaded ? (
                <Text fontWeight="700" fontSize="18px">
                  {t("Common:Rooms")}
                </Text>
              ) : (
                <Loaders.Rectangle
                  className="selection-panel_header-loader"
                  width="83px"
                  height="24px"
                />
              )}
            </div>

            <div className="selection-panel_aside-tree">
              {isLoaded ? (
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
          </div>
        </StyledAsideBody>
      </ModalDialog.Body>
      <ModalDialog.Footer>
        {footer}
        <div>
          <Button
            theme={theme}
            className="select-folder-dialog-buttons-save"
            primary
            scale
            size="normal"
            label={primaryButtonName}
            onClick={onButtonClick}
            isDisabled={isDisableButton}
          />
          <Button
            size="normal"
            scale
            label={t("Common:CancelButton")}
            onClick={onClose}
            isDisabled={isLoadingData}
          />
        </div>
      </ModalDialog.Footer>
    </StyledModalDialog>
  );
};
export default SelectFolderDialogAsideView;
