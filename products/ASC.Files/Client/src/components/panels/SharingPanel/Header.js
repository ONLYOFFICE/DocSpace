import React from "react";

import IconButton from "@appserver/components/icon-button";
import Heading from "@appserver/components/heading";
import DropDown from "@appserver/components/drop-down";
import DropDownItem from "@appserver/components/drop-down-item";

import { StyledHeaderContent } from "./StyledSharingPanel";

const Header = React.forwardRef(
  (
    {
      forwardRef,
      headerText,
      addUsers,
      addGroups,
      uploadPanelVisible,
      onPlusClickProp,
      isLoading,
      isPersonal,
      isEncrypted,
      showActionPanel,
      onShowUsersPanel,
      onShowGroupsPanel,
      onClose,
      onCloseActionPanel,
    },
    ref
  ) => {
    return (
      <StyledHeaderContent className="sharing_panel-header-container">
        <div className="sharing_panel-header-info">
          {uploadPanelVisible && (
            <IconButton
              size="15"
              iconName="/static/images/arrow.path.react.svg"
              className="sharing_panel-arrow"
              onClick={onClose}
            />
          )}
          <Heading className="sharing_panel-header" size="medium" truncate>
            {headerText}
          </Heading>
        </div>

        {!isPersonal && (
          <div className="sharing_panel-icons-container">
            <div ref={ref} className="sharing_panel-drop-down-wrapper">
              <IconButton
                size="15"
                iconName="/static/images/actions.header.touch.react.svg"
                className="sharing_panel-plus-icon"
                {...onPlusClickProp}
                isDisabled={isLoading}
              />

              <DropDown
                forwardedRef={forwardRef}
                directionX="right"
                className="sharing_panel-drop-down"
                open={showActionPanel}
                manualY="30px"
                clickOutsideAction={onCloseActionPanel}
              >
                <DropDownItem label={addUsers} onClick={onShowUsersPanel} />

                {!isEncrypted && (
                  <DropDownItem label={addGroups} onClick={onShowGroupsPanel} />
                )}
              </DropDown>
            </div>

            {/*<IconButton
      size="16"
      iconName="images/key.react.svg"
      onClick={onKeyClick}
    />*/}
          </div>
        )}
      </StyledHeaderContent>
    );
  }
);

export default React.memo(Header);
