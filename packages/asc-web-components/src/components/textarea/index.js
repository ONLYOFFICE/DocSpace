import React from "react";
import styled from "styled-components";
import Scrollbar from "../scrollbar/index";
import PropTypes from "prop-types";
import commonInputStyle from "../text-input/common-input-styles";
import TextareaAutosize from "react-autosize-textarea";

// eslint-disable-next-line react/prop-types, no-unused-vars
const ClearScrollbar = ({ isDisabled, heightScale, hasError, ...props }) => (
  <Scrollbar {...props} />
);
const StyledScrollbar = styled(ClearScrollbar)`
  ${commonInputStyle};
  :focus-within {
    border-color: ${(props) => (props.hasError ? "#c30" : "#2DA7DB")};
  }
  :focus {
    outline: none;
  }
  width: 100% !important;
  height: ${(props) =>
    props.heightScale
      ? "67vh"
      : props.heighttextarea
      ? props.heighttextarea + 2 + "px"
      : "91px"} !important;
  background-color: ${(props) => props.isDisabled && "#F8F9F9"};
`;

// eslint-disable-next-line react/prop-types, no-unused-vars
const ClearTextareaAutosize = ({
  isDisabled,
  heightScale,
  hasError,
  color,
  ...props
}) => <TextareaAutosize {...props} />;
const StyledTextarea = styled(ClearTextareaAutosize)`
  ${commonInputStyle};
  width: 100%;
  height: 90%;
  border: none;
  outline: none;
  resize: none;
  overflow: hidden;
  padding: 5px 8px 2px 8px;
  font-size: ${(props) => props.fontSize + "px"};
  font-family: "Open Sans", sans-serif;
  line-height: 1.5;

  :focus-within {
    border-color: #2da7db;
  }

  :focus {
    outline: none;
  }

  ::-webkit-input-placeholder {
    color: "#A3A9AE";
    font-family: "Open Sans", sans-serif;
    user-select: none;
  }

  :-moz-placeholder {
    color: "#A3A9AE";
    font-family: "Open Sans", sans-serif;
    user-select: none;
  }

  ::-moz-placeholder {
    color: "#A3A9AE";
    font-family: "Open Sans", sans-serif;
    user-select: none;
  }

  :-ms-input-placeholder {
    color: "#A3A9AE";
    font-family: "Open Sans", sans-serif;
    user-select: none;
  }

  ::placeholder {
    color: "#A3A9AE";
    font-family: "Open Sans", sans-serif;
    user-select: none;
  }
`;

class Textarea extends React.PureComponent {
  render() {
    // console.log('Textarea render');
    const {
      className,
      id,
      isDisabled,
      isReadOnly,
      hasError,
      heightScale,
      maxLength,
      name,
      onChange,
      placeholder,
      style,
      tabIndex,
      value,
      fontSize,
      heightTextArea,
      color,
    } = this.props;
    return (
      <StyledScrollbar
        className={className}
        style={style}
        stype="preMediumBlack"
        isDisabled={isDisabled}
        hasError={hasError}
        heightScale={heightScale}
        heighttextarea={heightTextArea}
      >
        <StyledTextarea
          id={id}
          placeholder={placeholder}
          onChange={(e) => onChange && onChange(e)}
          maxLength={maxLength}
          name={name}
          tabIndex={tabIndex}
          isDisabled={isDisabled}
          disabled={isDisabled}
          readOnly={isReadOnly}
          value={value}
          fontSize={fontSize}
          color={color}
        />
      </StyledScrollbar>
    );
  }
}

Textarea.propTypes = {
  className: PropTypes.string,
  id: PropTypes.string,
  isDisabled: PropTypes.bool,
  isReadOnly: PropTypes.bool,
  hasError: PropTypes.bool,
  heightScale: PropTypes.bool,
  maxLength: PropTypes.number,
  name: PropTypes.string,
  onChange: PropTypes.func,
  placeholder: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  tabIndex: PropTypes.number,
  value: PropTypes.string,
  fontSize: PropTypes.number,
  heightTextArea: PropTypes.number,
  color: PropTypes.string,
};

Textarea.defaultProps = {
  className: "",
  isDisabled: false,
  isReadOnly: false,
  hasError: false,
  heightScale: false,
  placeholder: "",
  tabIndex: -1,
  value: "",
  fontSize: 13,
  color: "#333333",
};

export default Textarea;
