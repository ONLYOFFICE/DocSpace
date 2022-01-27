import styled, { css } from "styled-components";
import Base from "../themes/base";
import DropDown from "../drop-down";
import DropDownItem from "../drop-down-item";
import FloatingButton from "@appserver/common/components/FloatingButton";

import { isMobile } from "react-device-detect";
import { mobile } from "../utils/device";

const StyledFloatingButton = styled(FloatingButton)`
  position: relative;
  z-index: 1010;

  .circle__mask + div {
    display: flex;
    align-items: center;
    justify-content: center;
    div {
      padding-top: 0;
      display: flex;
      align-items: center;
      justify-content: center;
      /* svg {
        ${(props) => props.isOpen && `margin-bottom: 4px;`}
      } */
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

  .circle__mask .circle__fill {
    background-color: #fff;
  }
`;

const mobileDropDown = css`
  width: ${(props) => props.theme.mainButtonMobile.dropDown.mobile.width};

  right: ${(props) => props.theme.mainButtonMobile.dropDown.mobile.right};
  bottom: ${(props) => props.theme.mainButtonMobile.dropDown.mobile.bottom};
`;

const StyledDropDown = styled(DropDown)`
  position: ${(props) => props.theme.mainButtonMobile.dropDown.position};
  width: ${(props) => props.theme.mainButtonMobile.dropDown.width};

  right: ${(props) => props.theme.mainButtonMobile.dropDown.right};
  bottom: ${(props) => props.theme.mainButtonMobile.dropDown.bottom};

  z-index: ${(props) => props.theme.mainButtonMobile.dropDown.zIndex};
  height: ${(props) => (props.isMobile ? props.heightProp : "auto")};

  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;

  padding: 0px;

  @media ${mobile} {
    ${mobileDropDown}
  }

  ${isMobile && mobileDropDown}

  .section-scroll {
    padding-right: 0px !important;
  }

  .separator-wrapper {
    padding: 12px 24px;
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

const StyledDropDownItem = styled(DropDownItem)`
  padding: 6px 23px;
`;

const StyledButtonOptions = styled.div`
  display: ${(props) => !props.isOpenButton && "none"};
  padding: 16px 0;
  background-color: #265a8f;
  color: #fff;
`;

const StyledContainerAction = styled.div`
  padding: 16px 0px;
`;

const StyledButtonWrapper = styled.div`
  padding: 0 24px 34px;
  display: ${(props) => (props.isOpenButton ? "none" : "block")};
  background-color: ${(props) => (props.isUploading ? "#ECEEF1" : "#fff")};
`;

const StyledProgressContainer = styled.div`
  display: ${(props) => (props.isUploading ? "flex" : "none")};
  background-color: ${(props) => (props.isUploading ? "#ECEEF1" : "#fff")};
  cursor: default;
  padding: 0 24px 34px;
`;

const StyledProgressBarContainer = styled.div`
  display: ${(props) => (props.isUploading ? "flex" : "none")};

  flex-wrap: wrap;

  width: 100%;

  box-sizing: border-box;

  height: 60px;
  padding-top: 26px;

  .progress-header {
    width: 50%;
    line-height: 16px;
    &:hover {
      cursor: pointer;
    }
  }

  .progress_count {
    line-height: 16px;
    width: 42%;
    text-align: right;
    margin-right: 6px;
  }
`;

const StyledMobileProgressBar = styled.div`
  width: 100%;
  height: 4px;
  background-color: rgb(48%, 58%, 69%, 0.4);
  border-radius: 2px;
  margin-top: 14px;
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
