import styled from "styled-components";
import { isMobile } from "react-device-detect";
import { Base } from "@docspace/components/themes";

const StyledBodyPreparationPortal = styled.div`
  margin-bottom: 24px;
  // display: flex;
  width: 100%;
  max-width: ${(props) => (props.errorMessage ? "560px" : "480px")};
  padding: 0 24px;
  box-sizing: border-box;
  align-items: center;
  position: relative;

  .preparation-portal_progress {
    display: flex;
    margin-bottom: 16px;
    position: relative;
    .preparation-portal_progress-bar {
      border-radius: 2px;
   
      width: 100%;

      height: 24px;
      background-color: #f3f4f4;
    }
    .preparation-portal_progress-line {
      border-radius: inherit;
      width: ${(props) => props.percent}%;
      background: #439ccd;
      height: inherit;
      transition-property: width;
      transition-duration: 0.9s;
      background: #1f97ca;
    }
    .preparation-portal_percent {
      position: absolute;

      ${(props) => props.percent > 50 && "color: white"};
      top: 2px;
      left: 50%;
    }
  }

  .preparation-portal_text {
    text-align: center;

    color: ${(props) => props.theme.text.disableColor};
  }
`;

StyledBodyPreparationPortal.defaultProps = { theme: Base };

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

export { StyledBodyPreparationPortal, StyledPreparationPortal };
