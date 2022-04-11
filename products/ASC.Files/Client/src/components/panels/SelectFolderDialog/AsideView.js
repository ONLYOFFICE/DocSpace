import React from "react";

import IconButton from "@appserver/components/icon-button";
import FolderTreeBody from "../../FolderTreeBody";
import { StyledAsidePanel, StyledSelectFolderPanel } from "../StyledPanels";
import Button from "@appserver/components/button";
import ModalDialog from "@appserver/components/modal-dialog";
import { StyledAsideBody } from "../SelectionPanel/StyledSelectionPanel";
import Text from "@appserver/components/text";
import Loaders from "@appserver/common/components/Loaders";
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
  canCreate,
  primaryButtonName,
  noTreeSwitcher,
}) => {
  console.log("canCreate", canCreate);
  return (
    <StyledAsidePanel theme={theme} visible={isPanelVisible}>
      <ModalDialog
        theme={theme}
        visible={isPanelVisible}
        contentHeight="100%"
        contentPaddingBottom="0px"
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
              {dialogName ? dialogName : t("Translations:FolderSelection")}
            </div>
          </StyledSelectFolderPanel>
        </ModalDialog.Header>
        <ModalDialog.Body theme={theme}>
          <StyledAsideBody theme={theme}>
            <div className="selection-panel_aside-body">
              <div>
                {header}
                <Text
                  fontWeight="700"
                  fontSize="18px"
                  className="selection-panel_aside-folder-title"
                >
                  {"Documents"}
                </Text>
              </div>

              <div className="selection-panel_aside-tree">
                {folderId && resultingFolderTree ? (
                  <FolderTreeBody
                    theme={theme}
                    folderTree={resultingFolderTree}
                    onSelect={onSelectFolder}
                    withoutProvider={withoutProvider}
                    certainFolders
                    isAvailable={isAvailable}
                    selectedKeys={[`${folderId}`]}
                    displayType="aside"
                  />
                ) : (
                  <Loaders.NewTreeFolders />
                )}
              </div>

              <div className="selection-panel_aside-footer">
                <div>{footer}</div>
                <div className="selection-panel_aside-buttons">
                  <Button
                    theme={theme}
                    className="select-folder-dialog-buttons-save"
                    primary
                    size="normal"
                    label={primaryButtonName}
                    onClick={onButtonClick}
                    isDisabled={isLoadingData || !isAvailable || !canCreate}
                  />
                  <Button
                    size="normal"
                    label={t("Common:CancelButton")}
                    onClick={onClose}
                    isDisabled={isLoadingData}
                  />
                </div>
              </div>
            </div>
          </StyledAsideBody>
        </ModalDialog.Body>
      </ModalDialog>
    </StyledAsidePanel>
  );
};
export default SelectFolderDialogAsideView;
