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
  height: 54px;
  min-height: 54px;
  box-sizing: border-box;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: ${(props) => `1px solid ${props.theme.infoPanel.borderColor}`};

  .header-text {
    margin-left: 20px;
  }
`;

const SubInfoPanelHeader = ({ children, onHeaderCrossClick }) => {
  const content = children?.props?.children;

  return (
    <StyledInfoPanelHeader>
      <Text className="header-text" fontSize="21px" fontWeight="700">
        {content}
      </Text>
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
  let onHeaderCrossClick = () => {};
  if (infoPanelStore) {
    onHeaderCrossClick = infoPanelStore.onHeaderCrossClick;
  }
  return { onHeaderCrossClick };
})(observer(SubInfoPanelHeader));
