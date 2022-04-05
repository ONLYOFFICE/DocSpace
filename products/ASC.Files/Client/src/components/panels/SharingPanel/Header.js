import React from "react";

import styled from "styled-components";

import IconButton from "@appserver/components/icon-button";
import Heading from "@appserver/components/heading";
import DropDown from "@appserver/components/drop-down";
import DropDownItem from "@appserver/components/drop-down-item";

const StyledHeaderContent = styled.div`
  width: calc(100% - 16px);
  max-width: calc(100% - 16px);
  height: 52px;

  border-bottom: 1px solid #eceef1;

  padding: 0 16px;

  margin-bottom: 24px;
  margin-right: -16px;

  display: flex;
  align-items: center;
  justify-content: space-between;

  .sharing_panel-header-info {
    max-width: calc(100% - 33px);

    display: flex;
    align-items: center;
    justify-content: start;

    .sharing_panel-arrow {
      margin-right: 16px;
    }

    .sharing_panel-header {
    }
  }

  .sharing_panel-icons-container {
    display: flex;
    margin-left: 16px;
  }
`;

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
              size="16"
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
                size="17"
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
