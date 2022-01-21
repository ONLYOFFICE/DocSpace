import styled, { css } from "styled-components";
import Base from "../themes/base";
import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import FloatingButton from "@appserver/common/components/FloatingButton";

const StyledFloatingButton = styled(FloatingButton)`
  .circle__background {
    background: ${(props) => props.theme.mainButtonMobile.buttonColor};

    path {
      fill: ${(props) => props.theme.mainButtonMobile.iconFill} !important;
    }
  }

  .circle__mask + div {
    svg {
      margin-top: 4px;
    }
  }

  ${(props) =>
    props.percent === 0 &&
    css`
      .circle__mask {
        clip: none;
      }

      .circle__fill {
        animation: none;
        transform: none;
        background: none !important;
      }
    `}

  .circle__mask,
  .circle__mask + div {
    z-index: 250;
  }

  .circle__mask .circle__fill {
    background-color: ${(props) =>
      props.theme.mainButtonMobile.circleBackground};
  }
`;

StyledFloatingButton.defaultProps = { theme: Base };

const StyledDropDown = styled(DropDown)`
  bottom: ${(props) => props.theme.mainButtonMobile.dropDown.bottom};
  right: ${(props) => props.theme.mainButtonMobile.dropDown.right};
  z-index: ${(props) => props.theme.mainButtonMobile.dropDown.zIndex};
  height: ${(props) => (props.isMobile ? props.heightProp : "auto")};
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  padding: 0px;

  .section-scroll {
    padding-right: 0px !important;
  }

  .separator-wrapper {
    padding: 23px;
  }

  .is-separator {
    height: 1px;
    width: 100%;
    background-color: ${(props) =>
      props.theme.mainButtonMobile.dropDown.separatorBackground};
  }

  .drop-down-item-button {
    color: ${(props) => props.theme.mainButtonMobile.dropDown.buttonColor};

    svg {
      path {
        fill: ${(props) => props.theme.mainButtonMobile.dropDown.buttonColor};
      }
    }

    &:hover {
      background-color: ${(props) =>
        props.theme.mainButtonMobile.dropDown.hoverButtonColor};
    }
  }

  .action-mobile-button {
    width: 100%;
    background-color: ${(props) =>
      props.theme.mainButtonMobile.dropDown.backgroundActionMobile};
    border-radius: 3px;
    font-size: 13px;
    display: block;
  }
`;

const StyledButtonOptions = styled.div`
  padding: ${(props) => (props.isOpenButton ? "23px 0px" : "0px")};
  background-color: ${(props) =>
    props.theme.mainButtonMobile.buttonOptions.backgroundColor};
  color: ${(props) => props.theme.mainButtonMobile.buttonOptions.color};
`;

StyledButtonOptions.defaultProps = { theme: Base };

const StyledContainerAction = styled.div`
  padding: 22px 0px 13px;
`;

const StyledProgressContainer = styled.div`
  background-color: ${(props) =>
    props.isUploading
      ? props.theme.mainButtonMobile.buttonWrapper.uploadingBackground
      : props.theme.mainButtonMobile.buttonWrapper.background};
  cursor: default;
  padding: ${(props) => (props.isUploading ? "16px 23px 7px;" : "0px")};
`;

StyledProgressContainer.defaultProps = { theme: Base };

const StyledButtonWrapper = styled.div`
  padding: ${(props) => (props.isOpenButton ? "0px" : "16px 23px 34px")};
  display: ${(props) => (props.isOpenButton ? "none" : "block")};
  background-color: ${(props) =>
    props.isUploading
      ? props.theme.mainButtonMobile.buttonWrapper.uploadingBackground
      : props.theme.mainButtonMobile.buttonWrapper.background};
`;

StyledButtonWrapper.defaultProps = { theme: Base };

const StyledProgressBarContainer = styled.div`
  display: ${(props) => (props.isUploading ? "flex" : "none")};
  flex-wrap: wrap;
  width: 100%;
  padding-bottom: 24px;

  .progress-header {
    width: 50%;
    color: ${(props) => props.theme.mainButtonMobile.textColor};
    &:hover {
      cursor: pointer;
    }
  }

  .progress_count {
    color: ${(props) => props.theme.mainButtonMobile.textColor};
    width: 42%;
    text-align: right;
    margin-right: 6px;
  }
`;

StyledProgressBarContainer.defaultProps = { theme: Base };

const StyledMobileProgressBar = styled.div`
  width: 100%;
  background-color: ${(props) =>
    props.theme.mainButtonMobile.mobileProgressBarBackground};
  border-radius: 2px;
  margin-top: 16px;
`;

const StyledBar = styled.div`
  width: ${(props) => props.uploadPercent}%;
  height: 4px;
  opacity: 1;
  background: ${(props) =>
    props.error
      ? props.theme.mainButtonMobile.bar.errorBackground
      : props.theme.mainButtonMobile.bar.background};
`;

StyledBar.defaultProps = { theme: Base };

StyledDropDown.defaultProps = { theme: Base };

const StyledDropDownItem = styled(DropDownItem)`
  padding: 7px 23px;
`;

export {
  StyledFloatingButton,
  StyledDropDown,
  StyledDropDownItem,
  StyledContainerAction,
  StyledProgressBarContainer,
  StyledMobileProgressBar,
  StyledProgressContainer,
  StyledBar,
  StyledButtonWrapper,
  StyledButtonOptions,
};
