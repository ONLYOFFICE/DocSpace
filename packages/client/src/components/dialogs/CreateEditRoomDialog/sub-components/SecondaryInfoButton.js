import React from "react";
import styled from "styled-components";

import HelpButton from "@docspace/components/help-button";
import { Base } from "@docspace/components/themes";

const StyledHelpButton = styled(HelpButton)`
  border-radius: 50%;
  background-color: ${({ theme }) =>
    theme.createEditRoomDialog.secondaryInfoButtton.background};

  path,
  rect {
    fill: ${({ theme }) =>
      theme.createEditRoomDialog.secondaryInfoButtton.icon};
  }
`;

StyledHelpButton.defaultProps = { theme: Base };

const SecondaryInfoButton = ({ content }) => {
  return (
    <StyledHelpButton
      displayType="auto"
      className="set_room_params-info-title-help"
      iconName="/static/images/info.no-border.svg"
      tooltipProps={{ globalEventOff: "click" }}
      tooltipContent={content}
      offsetRight={0}
      size={12}
    />
  );
};

export default SecondaryInfoButton;
