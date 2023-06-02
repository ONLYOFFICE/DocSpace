import React, { useRef, useEffect } from "react";
import PropTypes from "prop-types";
import InputWrapper from "./styled-code-input";

const CodeInput = (props) => {
  const { onSubmit, onChange, isDisabled } = props;

  const inputsRef = useRef([]);
  const characters = 6;
  const allowed = "^[A-Za-z0-9]*$";

  useEffect(() => {
    inputsRef.current[0].focus();
  }, []);

  const onEnter = () => {
    const code = inputsRef.current.map((input) => input.value).join("");
    if (code.length === characters) {
      onSubmit && onSubmit(code);
    }
  };

  const handleOnChange = (e) => {
    if (e.target.value.match(allowed)) {
      onChange();
      if (e.target.nextElementSibling !== null) {
        if (e.target.nextElementSibling.nodeName === "HR") {
          e.target.nextElementSibling.nextElementSibling.focus();
        }
        e.target.nextElementSibling.focus();
      }
    } else {
      e.target.value = "";
    }
    onEnter();
  };

  const handleOnKeyDown = (e) => {
    const { key } = e;
    const target = e.target;

    if (key === "Backspace") {
      onChange();
      if (target.value === "" && target.previousElementSibling !== null) {
        if (target.previousElementSibling !== null) {
          if (e.target.previousElementSibling.nodeName === "HR") {
            e.target.previousElementSibling.previousElementSibling.focus();
          }
          target.previousElementSibling.focus();
          e.preventDefault();
        }
      } else {
        target.value = "";
      }
    }
  };

  const handleOnFocus = (e) => {
    e.target.select();
  };

  const handleOnPaste = (e) => {
    const value = e.clipboardData.getData("Text");
    if (value.match(allowed)) {
      for (let i = 0; i < characters && i < value.length; i++) {
        inputsRef.current[i].value = value.charAt(i);
        if (inputsRef.current[i].nextElementSibling !== null) {
          inputsRef.current[i].nextElementSibling.focus();
        }
      }
    }
    onEnter();
    e.preventDefault();
  };

  const elements = [];
  for (let i = 0; i < characters; i++) {
    if (i === 3) elements.push(<hr key="InputCode-line" />);

    elements.push(
      <input
        key={`InputCode-${i}`}
        onChange={handleOnChange}
        onKeyDown={handleOnKeyDown}
        onFocus={handleOnFocus}
        onPaste={handleOnPaste}
        ref={(el) => (inputsRef.current[i] = el)}
        maxLength={1}
        disabled={isDisabled}
      />
    );
  }

  return <InputWrapper {...props}>{elements}</InputWrapper>;
};

CodeInput.propTypes = {
  /** Sets a callback function that is triggered when the enter is pressed */
  onSubmit: PropTypes.func.isRequired,
  /** Sets a callback function that is triggered on the onChange event */
  onChange: PropTypes.func,
  /** Sets the code input to present a disabled state */
  isDisabled: PropTypes.bool,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

CodeInput.defaultProps = {
  isDisabled: false,
};

export default CodeInput;
