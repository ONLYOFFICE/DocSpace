import styled from "styled-components";

import Base from "../themes/base";
import { tablet } from "../utils/device";
import { isMobile } from "react-device-detect";

const StyledSaveCancelButtons = styled.div`
  display: flex;
  flex-direction: ${(props) => (props.displaySettings ? "column" : "row")};
  position: ${(props) => (props.displaySettings ? "inherit" : "absolute")};
  justify-content: space-between;
  box-sizing: border-box;
  align-items: center;

  bottom: ${(props) => props.theme.saveCancelButtons.bottom};
  width: ${(props) => props.theme.saveCancelButtons.width};

  left: ${(props) => props.theme.saveCancelButtons.left};
  padding: ${(props) =>
    props.displaySettings ? "0" : props.theme.saveCancelButtons.padding};

  .save-button {
    margin-right: ${(props) => props.theme.saveCancelButtons.marginRight};
  }

  .unsaved-changes {
    color: ${(props) => props.theme.saveCancelButtons.unsavedColor};
  }

  ${(props) =>
    props.displaySettings &&
    `
    max-width:350px;

    .buttons-flex {
      display: flex;
      width: 100%;
      margin-bottom:8px;
    }

    .save-button, .cancel-button {
      width: 100%;
      min-width:100px;
      line-height: 16px;
    }
  `}

  @media ${tablet} {
    justify-content: ${(props) =>
      props.displaySettings ? "normal" : "flex-end"};
    position: ${(props) => (props.displaySettings ? "inherit" : "fixed")};

    .unsaved-changes {
      display: ${(props) => (props.displaySettings ? "block" : "none")};
    }
  }

  ${(props) =>
    props.displaySettings &&
    !isMobile &&
    props.sectionWidth >= 375 &&
    `
    align-items: flex-start;

    .save-button, .cancel-button {
      width: max-content;
    }  
  `}

  ${(props) =>
    props.displaySettings &&
    props.showReminder &&
    `
    margin-bottom:14px; 
  `}

  ${(props) =>
    props.displaySettings &&
    !props.showReminder &&
    `
    margin-bottom:32px; 
  `}
`;
StyledSaveCancelButtons.defaultProps = { theme: Base };
export default StyledSaveCancelButtons;
