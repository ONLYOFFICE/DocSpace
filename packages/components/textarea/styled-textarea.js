import React from "react";
import styled from "styled-components";
import TextareaAutosize from "react-autosize-textarea";

import Scrollbar from "../scrollbar/index";
import commonInputStyle from "../text-input/common-input-styles";
import Base from "../themes/base";
import { CopyIcon } from "./svg";

const ClearScrollbar = ({ isDisabled, heightScale, hasError, ...props }) => (
  <Scrollbar {...props} />
);
const StyledScrollbar = styled(ClearScrollbar)`
  ${commonInputStyle};

  :focus-within {
    border-color: ${(props) =>
      props.hasError
        ? props.theme.textArea.focusErrorBorderColor
        : props.theme.textArea.focusBorderColor};
  }

  :focus {
    outline: ${(props) => props.theme.textArea.focusOutline};
  }

  width: ${(props) => props.theme.textArea.scrollWidth} !important;
  height: ${(props) =>
    props.heightScale
      ? "67vh"
      : props.heighttextarea
      ? props.heighttextarea + 2 + "px"
      : "91px"} !important;

  background-color: ${(props) =>
    props.isDisabled && props.theme.textArea.disabledColor};
`;

StyledScrollbar.defaultProps = {
  theme: Base,
};

// eslint-disable-next-line react/prop-types, no-unused-vars
const ClearTextareaAutosize = React.forwardRef(
  (
    {
      isDisabled,
      heightScale,
      hasError,
      color,
      paddingLeftProp,
      isJSONField,
      ...props
    },
    ref
  ) => <TextareaAutosize {...props} ref={ref} />
);

const StyledTextarea = styled(ClearTextareaAutosize).attrs(
  ({ autoFocus, ...props }) => ({
    autoFocus: props.autoFocus,
  })
)`
  ${commonInputStyle};
  white-space: ${(props) => (props.isJSONField ? "pre" : "normal")};

  display: table;
  width: -webkit-fill-available;
  height: fit-content;
  border: none;
  outline: none;
  resize: none;
  overflow: ${(props) => (props.isJSONField ? "visible !important" : "hidden")};
  padding: ${(props) => (props.showCopyIcon ? "5px 28px 2px" : "5px 8px 2px")};
  padding-left: ${(props) => props.paddingLeftProp};
  font-size: ${(props) => props.fontSize + "px"};
  font-family: ${(props) => props.theme.fontFamily};
  line-height: 1.5;

  :focus-within {
    border-color: ${(props) => props.theme.textArea.focusBorderColor};
  }

  :focus {
    outline: ${(props) => props.theme.textArea.focusOutline};
  }

  ::-webkit-input-placeholder {
    color: ${(props) => props.theme.textInput.placeholderColor};
    font-family: ${(props) => props.theme.fontFamily};
    user-select: none;
  }

  :-moz-placeholder {
    color: ${(props) => props.theme.textInput.placeholderColor};
    font-family: ${(props) => props.theme.fontFamily};
    user-select: none;
  }

  ::-moz-placeholder {
    color: ${(props) => props.theme.textInput.placeholderColor};
    font-family: ${(props) => props.theme.fontFamily};
    user-select: none;
  }

  :-ms-input-placeholder {
    color: ${(props) => props.theme.textInput.placeholderColor};
    font-family: ${(props) => props.theme.fontFamily};
    user-select: none;
  }

  ::placeholder {
    color: ${(props) => props.theme.textInput.placeholderColor};
    font-family: ${(props) => props.theme.fontFamily};
    user-select: none;
  }
`;

StyledTextarea.defaultProps = {
  theme: Base,
};

const StyledCopyIcon = styled(CopyIcon)`
  position: absolute;
  right: ${(props) =>
    props.isJSONField && props.heightScale ? "16px" : "8px"};
  top: 8px;
  width: 16px;
  height: 16px;
  z-index: 2;
  filter: ${(props) => props.theme.textArea.copyIconFilter};

  :hover {
    cursor: pointer;
  }
`;

StyledCopyIcon.defaultProps = {
  theme: Base,
};

const Wrapper = styled.div`
  position: relative;

  min-width: ${(props) => (props.isJSONField ? "600px" : "none")};
  max-width: ${(props) => (props.isLimitedWidth ? "1200px" : "none")};
`;

const Numeration = styled.pre`
  display: block;
  position: absolute;
  font-size: ${(props) => props.fontSize + "px"};
  font-family: ${(props) => props.theme.fontFamily};
  line-height: 1.5;
  z-index: 2;
  margin: 0;
  top: 6px;
  left: 18px;
  text-align: right;
  color: ${(props) => props.theme.textArea.numerationColor};

  -webkit-user-select: none; /* Safari */
  -moz-user-select: none; /* Firefox */
  -ms-user-select: none; /* IE10+/Edge */
  user-select: none; /* Standard */
`;

Numeration.defaultProps = {
  theme: Base,
};

export { StyledTextarea, StyledScrollbar, StyledCopyIcon, Wrapper, Numeration };
