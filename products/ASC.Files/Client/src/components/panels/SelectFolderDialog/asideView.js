import React from "react";

import IconButton from "@appserver/components/icon-button";
import Aside from "@appserver/components/aside";
import Heading from "@appserver/components/heading";
import Backdrop from "@appserver/components/backdrop";
import FolderTreeBody from "../FolderTreeBody";
import {
  StyledAsidePanel,
  StyledSelectFilePanel,
  StyledHeaderContent,
} from "../StyledPanels";

const DISPLAY_TYPE = "aside";
const SelectFolderDialogAsideView = ({
  t,
  isPanelVisible,
  zIndex,
  onClose,
  withoutProvider,
  isNeedArrowIcon,
  id,
  asideHeightContent,
  isAvailable,
  certainFolders,
  folderId,
  isLoadingData,
  folderList,
  onSelect,
}) => {
  return (
    <StyledAsidePanel visible={isPanelVisible}>
      <Backdrop
        onClick={onClose}
        visible={isPanelVisible}
        zIndex={zIndex}
        isAside={true}
      />
      <Aside visible={isPanelVisible} zIndex={zIndex}>
        <StyledSelectFilePanel displayType={DISPLAY_TYPE}>
          <StyledHeaderContent className="select-file-dialog_aside-header">
            <div className="select-file-dialog_header">
              {isNeedArrowIcon && (
                <IconButton
                  className="select-file-dialog_header-icon"
                  size="16"
                  iconName="/static/images/arrow.path.react.svg"
                  onClick={onClose}
                  color="#A3A9AE"
                />
              )}

              <Heading
                size="medium"
                className="select-file-dialog_aside-header_title"
              >
                {t("ChooseFolder")}
              </Heading>
            </div>
          </StyledHeaderContent>

          <div className="select-file-dialog_aside-body_wrapper">
            <div className="select-file-dialog_aside_body">
              <FolderTreeBody
                isLoadingData={isLoadingData}
                folderList={folderList}
                onSelect={onSelect}
                withoutProvider={withoutProvider}
                certainFolders={certainFolders}
                isAvailable={isAvailable}
                selectedKeys={[id ? id : folderId]}
                heightContent={asideHeightContent}
              />
            </div>
          </div>
        </StyledSelectFilePanel>
      </Aside>
    </StyledAsidePanel>
  );
};
export default SelectFolderDialogAsideView;
