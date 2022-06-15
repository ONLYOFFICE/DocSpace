import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import { Base } from "@appserver/components/themes";
import { tablet } from "@appserver/components/utils/device";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import React from "react";
import styled from "styled-components";
import Submenu from "@appserver/components/submenu";

const StyledInfoPanelHeader = styled.div`
  width: 100%;
  max-width: 100%;
  height: ${(props) => (props.isPrivacyFolder ? "85px" : "52px")};
  min-height: ${(props) => (props.isPrivacyFolder ? "85px" : "52px")};
  display: flex;
  flex-direction: column;

  border-bottom: ${(props) =>
    props.isPrivacyFolder
      ? "none"
      : `1px solid ${props.theme.infoPanel.borderColor}`};

  .main {
    height: 52px;
    min-height: 52px;
    padding-bottom: 1px;
    display: flex;
    flex-direction: row;
    align-items: center;
    justify-content: space-between;

    .header-text {
      margin-left: 20px;
    }
  }

  .submenu {
    display: flex;
    width: 100%;
    justify-content: center;

    .sticky {
      display: flex;
      flex-direction: column;
      align-items: center;
    }
  }
`;

const StyledInfoPanelToggleWrapper = styled.div`
  display: flex;

  @media ${tablet} {
    display: none;
  }

  align-items: center;
  justify-content: center;
  padding-right: 20px;

  .info-panel-toggle-bg {
    height: 32px;
    width: 32px;
    display: flex;
    align-items: center;
    justify-content: center;
    border-radius: 50%;
    background-color: ${(props) =>
      props.theme.infoPanel.sectionHeaderToggleBgActive};

    path {
      fill: ${(props) => props.theme.infoPanel.sectionHeaderToggleIconActive};
    }
  }
`;
StyledInfoPanelToggleWrapper.defaultProps = { theme: Base };

const SubInfoPanelHeader = ({
  children,
  setIsVisible,
  roomState,
  setRoomState,
  isPrivacyFolder,
}) => {
  const content = children?.props?.children;

  const closeInfoPanel = () => setIsVisible(false);

  return (
    <StyledInfoPanelHeader isPrivacyFolder={isPrivacyFolder}>
      <div className="main">
        <Text className="header-text" fontSize="21px" fontWeight="700">
          {content}
        </Text>
        <StyledInfoPanelToggleWrapper
          isRootFolder={true}
          isInfoPanelVisible={true}
        >
          <div className="info-panel-toggle-bg">
            <IconButton
              className="info-panel-toggle"
              iconName="images/panel.react.svg"
              size="16"
              isFill={true}
              onClick={closeInfoPanel}
            />
          </div>
        </StyledInfoPanelToggleWrapper>
      </div>

      {isPrivacyFolder && (
        <div className="submenu">
          <Submenu
            style={{ width: "100%" }}
            data={[
              {
                content: null,
                onClick: () => setRoomState("members"),
                id: "members",
                name: "Members",
              },
              {
                content: null,
                onClick: () => setRoomState("history"),
                id: "History",
                name: "History",
              },
              {
                content: null,
                onClick: () => setRoomState("details"),
                id: "Details",
                name: "Details",
              },
            ]}
            startSelect={["members", "history", "details"].indexOf(roomState)}
          />
        </div>
      )}
    </StyledInfoPanelHeader>
  );
};

SubInfoPanelHeader.propTypes = {
  children: PropTypes.oneOfType([
    PropTypes.arrayOf(PropTypes.node),
    PropTypes.node,
    PropTypes.any,
  ]),
  toggleIsVisible: PropTypes.func,
};

StyledInfoPanelHeader.defaultProps = { theme: Base };
SubInfoPanelHeader.defaultProps = { theme: Base };

SubInfoPanelHeader.displayName = "SubInfoPanelHeader";

export default inject(({ auth, treeFoldersStore }) => {
  const { setIsVisible, roomState, setRoomState } = auth.infoPanelStore;
  const { isPrivacyFolder } = treeFoldersStore;
  return { setIsVisible, roomState, setRoomState, isPrivacyFolder };
})(observer(SubInfoPanelHeader));
