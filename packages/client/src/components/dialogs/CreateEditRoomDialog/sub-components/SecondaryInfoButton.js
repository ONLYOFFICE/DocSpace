import React from "react";
import styled from "styled-components";

import HelpButton from "@docspace/components/help-button";

const StyledHelpButton = styled(HelpButton)`
  border-radius: 50%;
  background-color: #a3a9ae;
  circle,
  rect {
    fill: #ffffff;
  }
`;

const SecondaryInfoButton = ({ content }) => {
  return (
    <StyledHelpButton
      displayType="auto"
      className="set_room_params-info-title-help"
      iconName="/static/images/info.react.svg"
      tooltipProps={{ globalEventOff: "click" }}
      tooltipContent={content}
      offsetRight={0}
      size={12}
    />
  );
};

export default SecondaryInfoButton;
