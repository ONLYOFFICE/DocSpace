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
  display: flex;
  align-items: center;
  line-height: ${(props) => props.theme.passwordInput.lineHeight};
  flex-direction: row;
  flex-wrap: wrap;
  position: relative;

  .input-relative {
    svg {
      path {
        fill: ${(props) =>
          props.isDisabled
            ? props.theme.passwordInput.disableColor
            : props.theme.passwordInput.color} !important;
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

const NewPasswordButton = styled.div`
  margin: ${(props) => props.theme.passwordInput.newPassword.margin};
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  svg {
    overflow: ${(props) => props.theme.passwordInput.newPassword.svg.overflow};
    vertical-align: middle;
    margin-bottom: ${(props) =>
      props.theme.passwordInput.newPassword.svg.marginBottom};
    path {
      fill: ${(props) =>
        props.isDisabled
          ? props.theme.passwordInput.disableColor
          : props.theme.passwordInput.color};
    }
  }
  :hover {
    cursor: pointer;
  }
`;
NewPasswordButton.defaultProps = { theme: Base };

const CopyLink = styled.div`
  margin-top: ${(props) => props.theme.passwordInput.link.marginTop};
  .password-input_link {
    color: ${(props) =>
      props.isDisabled
        ? props.theme.passwordInput.disableColor
        : props.theme.passwordInput.color};
  }
  @media ${tablet} {
    width: ${(props) => props.theme.passwordInput.link.tablet.width};
    margin-left: ${(props) => props.theme.passwordInput.link.tablet.marginLeft};
    margin-top: ${(props) => props.theme.passwordInput.link.tablet.marginTop};
  }
`;
CopyLink.defaultProps = { theme: Base };

const Progress = styled.div`
  border: 1.5px solid
    ${(props) =>
      !props.isDisabled && props.progressColor
        ? props.progressColor
        : "transparent"};
  border-radius: ${(props) => props.theme.passwordInput.progress.borderRadius};
  margin-top: ${(props) => props.theme.passwordInput.progress.marginTop};
  width: ${(props) => (props.progressWidth ? props.progressWidth + "%" : "0%")};
`;
Progress.defaultProps = { theme: Base };

const TooltipStyle = styled.div`
  .__react_component_tooltip {
  }
`;

const StyledTooltipContainer = styled(Text)`
  //margin: 8px 16px 16px 16px;
`;

const StyledTooltipItem = styled(Text)`
  margin-left: 8px;
  height: 24px;
  color: ${(props) => (props.valid ? "#44bb00" : "#B40404")};
`;

export {
  Progress,
  CopyLink,
  NewPasswordButton,
  PasswordProgress,
  StyledInput,
  TooltipStyle,
  StyledTooltipContainer,
  StyledTooltipItem,
};
