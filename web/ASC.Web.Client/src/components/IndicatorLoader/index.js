import React from "react";
import styled from "styled-components";

const StyledWrapper = styled.div`
  #ipl-progress-indicator {
    position: fixed;
    z-index: 390;
    top: ${(props) => (props.isDesktop ? "0" : "48px")};
    left: -6px;
    width: 0%;
    height: 3px;
    background-color: #eb835f;
    -moz-border-radius: 1px;
    -webkit-border-radius: 1px;
    border-radius: 1px;
  }
`;

const IndicatorLoader = () => {
  return (
    <StyledWrapper>
      <div id="ipl-progress-indicator"></div>
    </StyledWrapper>
  );
};

export default IndicatorLoader;
