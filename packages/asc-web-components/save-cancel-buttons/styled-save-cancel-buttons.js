import styled, { css } from "styled-components";
import Base from "../themes/base";
import { tablet } from "../utils/device";
import { isMobile, isTablet } from "react-device-detect";

const displaySettings = css`
  position: relative;
  display: block;
  flex-direction: column-reverse;
  align-items: flex-start;
  border-top: ${(props) =>
    props.border && !props.showReminder ? "1px solid #ECEEF1" : "none"};

  .buttons-flex {
    display: flex;
    width: 100%;
  }

  .save-button,
  .cancel-button {
    width: 100%;
    height: auto;
    line-height: 16px;
    padding-top: 11px;
    padding-bottom: 11px;
  }

  .unsaved-changes {
    position: absolute;
    padding-top: 16px
    font-size: 12px;
    font-weight: 600;
  }

  ${(props) =>
    props.showReminder &&
    props.border &&
    css`
      .unsaved-changes {
        background-color: white;
        border-top: 1px solid #eceef1;
        width: 100%;
        bottom: 56px;
        padding-top: 16px;
      }
    `}

    @media (orientation: landscape) {
      display: flex;

      .unsaved-changes {
        font-size: 12px;
        position: static;
        padding-bottom: 16px;
      }
  }
`;

const staticButtons = `
  position: static;
  max-width: none;
  flex-direction: row;
  justify-content: flex-start;
  align-items: center;
  padding-bottom: 24px;

  .buttons-flex {
    width: auto;
  }

  .save-button,
  .cancel-button {
    max-width: max-content;
    padding-left: 28px;
    padding-right: 28px;
  }

  .unsaved-changes {
    margin-left: 8px;
    margin-bottom: 0;
  }
`;

const StyledSaveCancelButtons = styled.div`
  display: flex;
  position: absolute;
  justify-content: space-between;
  box-sizing: border-box;
  align-items: center;
  bottom: ${(props) => props.theme.saveCancelButtons.bottom};
  width: ${(props) => props.theme.saveCancelButtons.width};
  left: ${(props) =>
    props.displaySettings ? "auto" : props.theme.saveCancelButtons.left};
  padding: ${(props) =>
    props.displaySettings
      ? "16px 0px 0px 0px"
      : props.theme.saveCancelButtons.padding};

  .save-button {
    margin-right: ${(props) => props.theme.saveCancelButtons.marginRight};
  }
  .unsaved-changes {
    color: ${(props) => props.theme.saveCancelButtons.unsavedColor};
  }

  ${(props) => props.displaySettings && displaySettings}

  ${(props) =>
    props.displaySettings &&
    ((!isMobile && props.sectionWidth > 375 && props.sectionWidth <= 1024) ||
      isTablet) &&
    `
      ${staticButtons}
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

  ${(props) =>
    props.displaySettings &&
    !isTablet &&
    props.sectionWidth > 1024 &&
    `
      ${staticButtons}
      .save-button, .cancel-button {
        font-size: 13px;
        line-height: 20px;
        padding-top: 5px;
        padding-bottom: 5px;
      }
    `}
`;
StyledSaveCancelButtons.defaultProps = { theme: Base };
export default StyledSaveCancelButtons;
