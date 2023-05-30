import styled from "styled-components";
import Base from "@docspace/components/themes/base";

const StyledPreparationPortalProgress = styled.div`
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
`;

StyledPreparationPortalProgress.defaultProps = { theme: Base };

export default StyledPreparationPortalProgress;
