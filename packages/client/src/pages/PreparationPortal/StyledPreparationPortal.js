import styled from "styled-components";
import { isMobile } from "react-device-detect";
import { Base } from "@docspace/components/themes";

const StyledBodyPreparationPortal = styled.div`
  margin-bottom: 24px;
  width: 100%;
  max-width: ${(props) => (props.errorMessage ? "560px" : "480px")};
  box-sizing: border-box;
  align-items: center;

  .preparation-portal_progress {
    display: flex;
    margin-bottom: 16px;
    position: relative;
    .preparation-portal_progress-bar {
      padding: 2px;
      box-sizing: border-box;
      border-radius: 6px;
      width: 100%;
      height: 24px;
      background-color: ${(props) =>
        props.theme.preparationPortalProgress.backgroundColor};
    }
    .preparation-portal_progress-line {
      border-radius: 5px;
      width: ${(props) => props.percent}%;
      height: 20px;
      transition-property: width;
      transition-duration: 0.9s;
    }
    .preparation-portal_percent {
      position: absolute;
      font-size: 14px;
      font-weight: 600;
      color: ${(props) =>
        props.percent > 50
          ? props.theme.preparationPortalProgress.colorPercentBig
          : props.theme.preparationPortalProgress.colorPercentSmall};
      top: 2px;
      left: calc(50% - 9px);
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
