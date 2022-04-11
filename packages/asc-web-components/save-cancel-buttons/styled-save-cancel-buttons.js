import styled, { css } from "styled-components";
import Base from "../themes/base";
import { tablet } from "../utils/device";
import { isMobileOnly, isTablet } from "react-device-detect";

const displaySettings = css`
  position: absolute;
  display: block;
  flex-direction: column-reverse;
  align-items: flex-start;
  border-top: ${(props) =>
    props.hasScroll && !props.showReminder ? "1px solid #ECEEF1" : "none"};

  ${(props) =>
    !props.hasScroll &&
    !isMobileOnly &&
    css`
      padding-left: 24px;
    `}

  ${(props) =>
    props.hasScroll &&
    css`
      bottom: auto;
    `}

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
    padding-top: 16px;
    padding-bottom: 16px;
    font-size: 12px;
    font-weight: 600;
    width: calc(100% - 32px);
    bottom: 56px;
    background-color: ${(props) =>
      props.hasScroll
        ? props.theme.mainButtonMobile.buttonWrapper.background
        : "none"};
  }

  ${(props) =>
    props.showReminder &&
    props.hasScroll &&
    css`
      .unsaved-changes {
        border-top: 1px solid #eceef1;
        width: calc(100% - 16px);
        left: 0px;
        padding-left: 16px;
      }
    `}
`;

const tabletButtons = css`
  position: static;
  display: flex;
  max-width: none;
  flex-direction: row;
  justify-content: flex-start;
  align-items: center;
  padding: 0;
  border-top: none;

  .buttons-flex {
    width: auto;
  }

  .save-button,
  .cancel-button {
    width: auto;
    padding-left: 28px;
    padding-right: 28px;
  }

  .unsaved-changes {
    border-top: none;
    margin-left: 8px;
    margin-bottom: 0;
    position: static;
    padding: 0;
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
  left: ${(props) => props.theme.saveCancelButtons.left};
  padding: ${(props) =>
    props.displaySettings ? "16px" : props.theme.saveCancelButtons.padding};

  .save-button {
    margin-right: ${(props) => props.theme.saveCancelButtons.marginRight};
  }
  .unsaved-changes {
    color: ${(props) => props.theme.saveCancelButtons.unsavedColor};
  }

  ${(props) => props.displaySettings && displaySettings}

  @media (min-width: 600px), isTablet {
    ${(props) =>
      props.displaySettings &&
      `
      ${tabletButtons}
    `}
  }

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

  @media (min-width: 1024px) {
    ${(props) =>
      props.displaySettings &&
      !isTablet &&
      `
      .save-button, .cancel-button {
        font-size: 13px;
        line-height: 20px;
        padding-top: 5px;
        padding-bottom: 5px;
      }

    `}
  }
`;
StyledSaveCancelButtons.defaultProps = { theme: Base };
export default StyledSaveCancelButtons;
