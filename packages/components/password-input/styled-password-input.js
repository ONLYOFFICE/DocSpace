import React from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import Text from "../text";
import { tablet, mobile } from "../utils/device";
import Base from "../themes/base";
// eslint-disable-next-line no-unused-vars

const SimpleInput = ({ onValidateInput, onCopyToClipboard, ...props }) => (
  <div {...props}></div>
);

SimpleInput.propTypes = {
  onValidateInput: PropTypes.func,
  onCopyToClipboard: PropTypes.func,
};

const StyledInput = styled(SimpleInput)`
  display: ${(props) => (props.$isFullWidth ? "block" : "flex")};
  align-items: center;
  line-height: ${(props) => props.theme.passwordInput.lineHeight};
  flex-direction: row;
  flex-wrap: wrap;
  position: relative;

  input {
    flex: inherit;
    width: calc(100% - 40px);
  }

  .input-relative {
    svg {
      path {
        fill: ${(props) =>
          props.isDisabled
            ? props.theme.passwordInput.disableColor
            : props.theme.passwordInput.iconColor} !important;
      }
    }

    &:hover {
      svg {
        path {
          fill: ${(props) =>
            props.isDisabled
              ? props.theme.passwordInput.disableColor
              : props.theme.passwordInput.hoverIconColor} !important;
        }
      }
    }
  }

  @media ${tablet} {
    flex-wrap: wrap;
  }

  .input-block-icon {
    height: 42px;
  }

  .append {
    padding-right: 8px;
    position: absolute;
    right: -16px;
    top: 50%;
    transform: translate(-50%, -50%);
  }

  .prepend-children {
    padding: 0;
  }

  .break {
    flex-basis: 100%;
    height: 0;
  }

  .text-tooltip {
    line-height: ${(props) => props.theme.passwordInput.text.lineHeight};
    margin-top: ${(props) => props.theme.passwordInput.text.marginTop};
  }

  .password-field-wrapper {
    display: flex;
    width: auto;

    @media ${mobile} {
      width: 100%;
    }
  }
`;
StyledInput.defaultProps = { theme: Base };

const PasswordProgress = styled.div`
  ${(props) =>
    props.inputWidth ? `width: ${props.inputWidth};` : `flex: auto;`}
  .input-relative {
    position: relative;
    svg {
      overflow: hidden;
      vertical-align: middle;
    }
  }

  *,
  *::before,
  *::after {
    box-sizing: border-box;
  }
`;
PasswordProgress.defaultProps = { theme: Base };

const TooltipStyle = styled.div`
  width: 294px;

  @media (max-width: 768px) {
    width: 320px;
  }

  .__react_component_tooltip {
  }
`;

const StyledTooltipContainer = styled(Text)`
  // margin: 8px 16px 16px 16px;
  color: ${(props) => props.theme.passwordInput.tooltipTextColor} !important;

  .generate-btn-container {
    margin-top: 10px;
  }

  .generate-btn {
    color: ${(props) => props.theme.passwordInput.tooltipTextColor};
  }
`;

StyledTooltipContainer.defaultProps = { theme: Base };

const StyledTooltipItem = styled(Text)`
  //height: 24px;
  color: ${(props) => (props.valid ? "#44bb00" : "#B40404")};
`;

export {
  PasswordProgress,
  StyledInput,
  TooltipStyle,
  StyledTooltipContainer,
  StyledTooltipItem,
};
