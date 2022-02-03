import React, { useRef, useEffect } from "react";
import PropTypes from "prop-types";
import InputWrapper from "./styled-code-input";

const CodeInput = (props) => {
  const { onSubmit, isDisabled } = props;

  const inputsRef = useRef([]);
  const characters = 6;
  const allowed = "^[0-9]*$";

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
      if (e.target.nextElementSibling !== null) {
        e.target.nextElementSibling.focus();
      }
    } else {
      e.target.value = "";
    }
  };

  const handleOnKeyDown = (e) => {
    const { key } = e;
    const target = e.target;

    //if (key === "Enter") onEnter();

    if (key === "Backspace") {
      if (target.value === "" && target.previousElementSibling !== null) {
        if (target.previousElementSibling !== null) {
          target.previousElementSibling.focus();
          e.preventDefault();
        }
      } else {
        target.value = "";
      }
    }
    onEnter();
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
  onSubmit: PropTypes.func.isRequired,
  isDisabled: PropTypes.bool,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

CodeInput.defaultProps = {
  isDisabled: false,
};

export default CodeInput;
