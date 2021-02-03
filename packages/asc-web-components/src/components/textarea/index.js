import React from "react";
import PropTypes from "prop-types";
import { StyledTextarea, StyledScrollbar } from "./styled-textarea";


// eslint-disable-next-line react/prop-types, no-unused-vars

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
