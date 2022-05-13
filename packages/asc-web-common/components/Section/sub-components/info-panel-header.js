import IconButton from "@appserver/components/icon-button";
import Text from "@appserver/components/text";
import { Base } from "@appserver/components/themes";
import { tablet } from "@appserver/components/utils/device";
import { inject, observer } from "mobx-react";
import PropTypes from "prop-types";
import React from "react";
import styled from "styled-components";

const StyledInfoPanelHeader = styled.div`
  width: 100%;
  max-width: 100%;
  height: 52px;
  min-height: 52px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  align-self: center;
  border-bottom: ${(props) => `1px solid ${props.theme.infoPanel.borderColor}`};

  .header-text {
    margin-left: 20px;
  }
`;

const StyledInfoPanelToggleWrapper = styled.div`
  display: flex;
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

const SubInfoPanelHeader = ({ children, closeInfoPanel }) => {
  const content = children?.props?.children;

  return (
    <StyledInfoPanelHeader>
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

export default inject(({ infoPanelStore }) => {
  let closeInfoPanel = () => {};
  if (infoPanelStore) {
    closeInfoPanel = () => infoPanelStore.setIsVisible(false);
  }
  return { closeInfoPanel };
})(observer(SubInfoPanelHeader));
