import styled, { css } from "styled-components";
import { isMobileOnly } from "react-device-detect";

const StyledWrapper = styled.div`
  #ipl-progress-indicator {
    position: fixed;
    z-index: 390;
    top: 0;
    left: -6px;
    width: 0%;
    height: 3px;
    -moz-border-radius: 1px;
    -webkit-border-radius: 1px;
    border-radius: 1px;

    ${isMobileOnly &&
    css`
      top: 48px;
    `}
  }
`;

export default StyledWrapper;
