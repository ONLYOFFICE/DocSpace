import styled, { css } from "styled-components";
import Base from "../themes/base";
import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import FloatingButton from "@appserver/common/components/FloatingButton";

const StyledFloatingButton = styled(FloatingButton)`
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
    background-color: #fff;
  }
`;

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
    background-color: #fff;
  }

  .drop-down-item-button {
    color: #fff;

    svg {
      path {
        fill: #fff;
      }
    }

    &:hover {
      background-color: #3a6c9e;
    }
  }

  .action-mobile-button {
    width: 100%;
    background-color: #265a8f;
    border-radius: 3px;
    font-size: 13px;
    display: block;
  }
`;

const StyledButtonOptions = styled.div`
  padding: ${(props) => (props.isOpenButton ? "23px 0px" : "0px")};
  background-color: #265a8f;
  color: #fff;
`;

const StyledContainerAction = styled.div`
  padding: 22px 0px 13px;
`;

const StyledProgressContainer = styled.div`
  background-color: ${(props) => (props.isUploading ? "#ECEEF1" : "#fff")};
  cursor: default;
  padding: ${(props) => (props.isUploading ? "16px 23px 7px;" : "0px")};
`;

const StyledButtonWrapper = styled.div`
  padding: ${(props) => (props.isOpenButton ? "0px" : "16px 23px 34px")};
  display: ${(props) => (props.isOpenButton ? "none" : "block")};
  background-color: ${(props) => (props.isUploading ? "#ECEEF1" : "#fff")};
`;

const StyledProgressBarContainer = styled.div`
  display: ${(props) => (props.isUploading ? "flex" : "none")};
  flex-wrap: wrap;
  width: 100%;
  padding-bottom: 24px;

  .progress-header {
    width: 50%;
    &:hover {
      cursor: pointer;
    }
  }

  .progress_count {
    width: 42%;
    text-align: right;
    margin-right: 6px;
  }
`;

const StyledMobileProgressBar = styled.div`
  width: 100%;
  background-color: rgb(48%, 58%, 69%, 0.4);
  border-radius: 2px;
  margin-top: 16px;
`;

const StyledBar = styled.div`
  width: ${(props) => props.uploadPercent}%;
  height: 4px;
  opacity: 1;
  background: ${(props) =>
    props.error
      ? "#C96C27"
      : "linear-gradient(225deg, #2274aa 0%, #0f4071 100%)"}; ;
`;

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
