import styled from "styled-components";
import React from "react";

import commonInputStyle from "../text-input/common-input-styles";
import Base from "../themes/base";

const StyledIconBlock = styled.div`
  display: ${(props) => props.theme.inputBlock.display};
  align-items: ${(props) => props.theme.inputBlock.alignItems};
  cursor: ${(props) =>
    props.isDisabled || !props.isClickable ? "default" : "pointer"};

  height: ${(props) => props.theme.inputBlock.height};
  padding-right: ${(props) => props.theme.inputBlock.paddingRight};
  padding-left: ${(props) => props.theme.inputBlock.paddingLeft};
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
`;
StyledIconBlock.defaultProps = { theme: Base };

const StyledChildrenBlock = styled.div`
  display: ${(props) => props.theme.inputBlock.display};
  align-items: ${(props) => props.theme.inputBlock.alignItems};
  padding: ${(props) => props.theme.inputBlock.padding};
`;
StyledChildrenBlock.defaultProps = { theme: Base };

/* eslint-disable react/prop-types, no-unused-vars */
const CustomInputGroup = ({
  isIconFill,
  hasError,
  hasWarning,
  isDisabled,
  scale,
  hoverColor,
  ...props
}) => <div {...props}></div>;
/* eslint-enable react/prop-types, no-unused-vars */
const StyledInputGroup = styled(CustomInputGroup)`
  display: ${(props) => props.theme.inputBlock.display};

  input:-webkit-autofill,
  input:-webkit-autofill:hover,
  input:-webkit-autofill:focus,
  input:-webkit-autofill:active {
    -webkit-background-clip: text;
    -webkit-text-fill-color: #ffffff;
    transition: background-color 5000s ease-in-out 0s;
    box-shadow: inset 0 0 20px 20px #23232329;
  }

  .prepend {
    display: ${(props) => props.theme.inputBlock.display};
    align-items: ${(props) => props.theme.inputBlock.alignItems};
  }

  .append {
    align-items: ${(props) => props.theme.inputBlock.alignItems};
    margin: ${(props) => props.theme.inputBlock.margin};
  }

  ${commonInputStyle}

  :focus-within {
    border-color: ${(props) =>
      (props.hasError && props.theme.input.focusErrorBorderColor) ||
      props.theme.inputBlock.borderColor};
  }

  svg {
    path {
      fill: ${(props) =>
        props.color
          ? props.color
          : props.theme.inputBlock.iconColor} !important;
    }
  }

  &:hover {
    svg {
      path {
        fill: ${(props) =>
          props.hoverColor
            ? props.hoverColor
            : props.theme.inputBlock.hoverIconColor} !important;
      }
    }
  }
`;
StyledInputGroup.defaultProps = { theme: Base };

export { StyledInputGroup, StyledChildrenBlock, StyledIconBlock };
