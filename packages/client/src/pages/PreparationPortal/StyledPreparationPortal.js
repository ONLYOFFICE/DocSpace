import styled from "styled-components";
import { isMobile } from "react-device-detect";

const StyledPreparationPortal = styled.div`
  width: 100%;
  ${isMobile &&
  `
    margin-top: 48px;
  `}

  #header {
    font-size: 23px;
  }
  #text {
    color: #a3a9ae;
    font-size: 13px;
    line-height: 20px;
    max-width: 480px;
  }
`;

export { StyledPreparationPortal };
