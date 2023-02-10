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
  ({ isDisabled, heightScale, hasError, color, ...props }, ref) => (
    <TextareaAutosize {...props} ref={ref} />
  )
);

const StyledTextarea = styled(ClearTextareaAutosize).attrs((props) => ({
  autoFocus: props.autoFocus,
}))`
  ${commonInputStyle};

  min-width: 600px;
  max-width: 1200px;

  width: 100%;
  height: fit-content;
  border: none;
  outline: none;
  resize: none;
  overflow: hidden;
  padding: 5px 8px 2px 42px;
  font-size: ${(props) => props.fontSize + "px"};
  font-family: Open Sans, sans-serif, Arial;
  line-height: 1.5;

  :focus-within {
    border-color: ${(props) => props.theme.textArea.focusBorderColor};
  }

  :focus {
    outline: ${(props) => props.theme.textArea.focusOutline};
  }

  ::-webkit-input-placeholder {
    color: ${(props) => props.theme.textInput.placeholderColor};
    font-family: Open Sans, sans-serif, Arial;
    user-select: none;
  }

  :-moz-placeholder {
    color: ${(props) => props.theme.textInput.placeholderColor};
    font-family: Open Sans, sans-serif, Arial;
    user-select: none;
  }

  ::-moz-placeholder {
    color: ${(props) => props.theme.textInput.placeholderColor};
    font-family: Open Sans, sans-serif, Arial;
    user-select: none;
  }

  :-ms-input-placeholder {
    color: ${(props) => props.theme.textInput.placeholderColor};
    font-family: Open Sans, sans-serif, Arial;
    user-select: none;
  }

  ::placeholder {
    color: ${(props) => props.theme.textInput.placeholderColor};
    font-family: Open Sans, sans-serif, Arial;
    user-select: none;
  }
`;

StyledTextarea.defaultProps = {
  theme: Base,
};

const StyledCopyIcon = styled.img`
  position: absolute;
  right: 8px;
  top: 8px;
  width: 16px;
  height: 16px;
  z-index: 2;

  :hover {
    cursor: pointer;
  }
`;

StyledCopyIcon.defaultProps = {
  theme: Base,
};

const Wrapper = styled.div`
  position: relative;
`;

const Numeration = styled.pre`
  display: block;
  position: absolute;
  font-size: ${(props) => props.fontSize + "px"};
  font-family: Open Sans, sans-serif, Arial;
  line-height: 1.5;
  z-index: 2;
  margin: 0;
  top: 6px;
  left: 18px;
  text-align: right;
  color: #a3a9ae;
  -webkit-user-select: none; /* Safari */
  -moz-user-select: none; /* Firefox */
  -ms-user-select: none; /* IE10+/Edge */
  user-select: none; /* Standard */
`;

Numeration.defaultProps = {
  theme: Base,
};

export { StyledTextarea, StyledScrollbar, StyledCopyIcon, Wrapper, Numeration };
