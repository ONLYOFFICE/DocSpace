import styled from "styled-components";

import Base from "../themes/base";
import { tablet } from "../utils/device";
import { isMobile, isTablet } from "react-device-detect";

const StyledSaveCancelButtons = styled.div`
  display: flex;
  position: absolute;
  justify-content: space-between;
  box-sizing: border-box;
  align-items: center;
  bottom: ${(props) => props.theme.saveCancelButtons.bottom};
  width: ${(props) =>
    props.displaySettings
      ? "calc(100% - 40px)"
      : props.theme.saveCancelButtons.width};
  left: ${(props) =>
    props.displaySettings ? "auto" : props.theme.saveCancelButtons.left};
  padding: ${(props) =>
    props.displaySettings
      ? "0px 0px 16px 0px"
      : props.theme.saveCancelButtons.padding};

  .save-button {
    margin-right: ${(props) => props.theme.saveCancelButtons.marginRight};
  }
  .unsaved-changes {
    color: ${(props) => props.theme.saveCancelButtons.unsavedColor};
  }

  ${(props) =>
    props.displaySettings &&
    `
    flex-direction: column-reverse;
    align-items: flex-start;
  
    .buttons-flex {
      display: flex;
      width: 100%;
    }

    .save-button, .cancel-button {
      width: 100%;
      height: auto;
      line-height: 16px;
      padding-top: 11px;
      padding-bottom: 11px;
    }

    .unsaved-changes {
      margin-bottom: 12px;
    }
  `}

  ${(props) =>
    props.displaySettings &&
    ((!isMobile && props.sectionWidth > 375 && props.sectionWidth < 1440) ||
      isTablet) &&
    `
    position: static;
    max-width: none;
    flex-direction: row;
    justify-content: flex-start;
    align-items: center;

    .buttons-flex {
      width: auto;
    }
 
    .save-button, .cancel-button {
      max-width: max-content;
      padding-left: 28px;
      padding-right: 28px;
    }

    .unsaved-changes {
      margin-left: 8px;
      margin-bottom: 0;
    }
   
  `}

${(props) =>
    props.displaySettings &&
    !isTablet &&
    props.sectionWidth >= 1440 &&
    `
    .save-button, .cancel-button {
      font-size: 13px;
      line-height: 20px;
      padding-top: 5px;
      padding-bottom: 5px;
    }
  `}

  @media ${tablet} {
    ${(props) =>
      !props.displaySettings &&
      `
    justify-content: flex-end;
    position: fixed;

    .unsaved-changes {
      display: none;
    } 
  `}
  }
`;
StyledSaveCancelButtons.defaultProps = { theme: Base };
export default StyledSaveCancelButtons;
