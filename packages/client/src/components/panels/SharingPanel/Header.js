import React from "react";

import IconButton from "@docspace/components/icon-button";
import Heading from "@docspace/components/heading";
import DropDown from "@docspace/components/drop-down";
import DropDownItem from "@docspace/components/drop-down-item";

import { StyledHeaderContent } from "./StyledSharingPanel";

const Header = ({
  t,
  isPersonal,
  isEncrypted,
  uploadPanelVisible,
  onShowUsersPanel,
  onShowGroupsPanel,
  onClose,
  label,
}) => {
  const [showActionPanel, setShowActionPanel] = React.useState(false);

  const ref = React.useRef(null);

  const onPlusClick = React.useCallback(() => {
    setShowActionPanel((val) => !val);
  }, []);

  const onCloseActionPanel = React.useCallback(
    (e) => {
      if (ref.current.contains(e.target)) return;
      setShowActionPanel((val) => !val);
    },
    [ref.current]
  );

  const onShowUsersPanelAction = React.useCallback(() => {
    setShowActionPanel(false);
    onShowUsersPanel && onShowUsersPanel();
  }, [onShowUsersPanel]);

  const onShowGroupsPanelAction = React.useCallback(() => {
    setShowActionPanel(false);
    onShowGroupsPanel && onShowGroupsPanel();
  }, [onShowGroupsPanel]);

  return (
    <StyledHeaderContent
      isPersonal={isPersonal}
      className="sharing_panel-header-container"
    >
      <div className="sharing_panel-header-info">
        {uploadPanelVisible && (
          <IconButton
            size="15px"
            iconName="/static/images/arrow.path.react.svg"
            className="sharing_panel-arrow"
            onClick={onClose}
          />
        )}
        <Heading
          className="sharing_panel-header"
          size="medium"
          truncate={true}
          style={{ fontWeight: 700 }}
        >
          {uploadPanelVisible && label && isPersonal
            ? label
            : t("SharingSettingsTitle")}
        </Heading>
      </div>

      {!isPersonal && (
        <div className="sharing_panel-icons-container">
          <div ref={ref} className="sharing_panel-drop-down-wrapper">
            <IconButton
              size="15"
              iconName="/static/images/actions.header.touch.react.svg"
              className="sharing_panel-plus-icon"
              onClick={onPlusClick}
            />

            <DropDown
              forwardedRef={ref}
              directionX="right"
              className="sharing_panel-drop-down"
              open={showActionPanel}
              manualY="30px"
              clickOutsideAction={onCloseActionPanel}
            >
              <DropDownItem
                label={t("Common:AddUsers")}
                onClick={onShowUsersPanelAction}
              />

              {!isEncrypted && (
                <DropDownItem
                  label={t("AddGroupsForSharingButton")}
                  onClick={onShowGroupsPanelAction}
                />
              )}
            </DropDown>
          </div>
        </div>
      )}
    </StyledHeaderContent>
  );
};

export default React.memo(Header);
