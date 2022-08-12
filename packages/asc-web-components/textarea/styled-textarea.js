import React from "react";
import styled from "styled-components";
import TextareaAutosize from "react-autosize-textarea";

import Scrollbar from "../scrollbar/index";
import commonInputStyle from "../text-input/common-input-styles";
import Base from "../themes/base";

const ClearScrollbar = ({ isDisabled, heightScale, hasError, ...props }) => (
  <Scrollbar {...props} />
);
const StyledScrollbar = styled(ClearScrollbar)`
  ${commonInputStyle};

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
  ({ isDisabled, heightScale, hasError, color, ...props }, ref) => (
    <TextareaAutosize {...props} ref={ref} />
  )
);

const StyledTextarea = styled(ClearTextareaAutosize).attrs((props) => ({
  autoFocus: props.autoFocus,
}))`
  ${commonInputStyle};

  width: ${(props) => props.theme.textArea.width};
  height: ${(props) => props.theme.textArea.height};
  border: ${(props) => props.theme.textArea.border};
  outline: ${(props) => props.theme.textArea.outline};
  resize: ${(props) => props.theme.textArea.resize};
  overflow: ${(props) => props.theme.textArea.overflow};
  padding: ${(props) => props.theme.textArea.padding};
  font-size: ${(props) => props.fontSize + "px"};
  font-family: ${(props) => props.theme.fontFamily};
  line-height: ${(props) => props.theme.textArea.lineHeight};

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

export { StyledTextarea, StyledScrollbar };
