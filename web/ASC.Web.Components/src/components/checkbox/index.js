import React from "react";
import PropTypes from "prop-types";
import styled, { css } from "styled-components";
import { Icons } from "../icons";
import { Text } from "../text";

const disableColor = "#A3A9AE";
const hoverColor = disableColor;

const Label = styled.label`
  display: flex;
  align-items: center;
  position: relative;
  margin: 0;
  user-select: none;
  -o-user-select: none;
  -moz-user-select: none;
  -webkit-user-select: none;

  .checkbox {
    margin-right: 8px;
  }

  ${props =>
    props.isDisabled
      ? css`
          cursor: not-allowed;
        `
      : css`
          cursor: pointer;

          &:hover {
            svg {
              rect:first-child {
                stroke: ${hoverColor};
              }
            }
          }
        `}
`;

const HiddenInput = styled.input`
  opacity: 0.0001;
  position: absolute;
  right: 0;
  z-index: -1;
`;

const CheckboxIcon = ({ isChecked, isDisabled, isIndeterminate }) => {
  const iconName = isIndeterminate
    ? "CheckboxIndeterminateIcon"
    : isChecked
    ? "CheckboxCheckedIcon"
    : "CheckboxIcon";

  let newProps = {
    size: "medium",
    className: "checkbox"
  };

  if (isDisabled) {
    newProps.isfill = true;
    newProps.color = "#F8F9F9";

    if (isIndeterminate || isChecked) {
      newProps.isStroke = true;
      newProps.stroke = "#ECEEF1";
    }
  }

  return <>{React.createElement(Icons[iconName], { ...newProps })}</>;
};

class Checkbox extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      checked: props.isChecked
    };

    this.onInputChange = this.onInputChange.bind(this);
  }

  componentDidMount() {
    this.ref.current.indeterminate = this.props.isIndeterminate;
  }

  componentDidUpdate(prevProps) {
    if (this.props.isIndeterminate !== prevProps.isIndeterminate) {
      this.ref.current.indeterminate = this.props.isIndeterminate;
    }
    if (this.props.isChecked !== prevProps.isChecked) {
      this.setState({ checked: this.props.isChecked });
    }
  }

  onInputChange(e) {
    this.setState({ checked: e.target.checked });
    this.props.onChange && this.props.onChange(e);
  }

  render() {
    //console.log("Checkbox render");

    const colorProps = this.props.isDisabled ? {color: disableColor} : {};

    return (
      <Label htmlFor={this.props.id} isDisabled={this.props.isDisabled}>
        <HiddenInput
          type="checkbox"
          checked={this.state.checked}
          disabled={this.props.isDisabled}
          ref={this.ref}
          {...this.props}
          onChange={this.onInputChange}
        />
        <CheckboxIcon {...this.props} />
        {this.props.label && (
          <Text.Body
            as="span"
            {...colorProps}
          >
            {this.props.label}
          </Text.Body>
        )}
      </Label>
    );
  }
}

Checkbox.propTypes = {
  id: PropTypes.string,
  name: PropTypes.string,
  value: PropTypes.string,
  label: PropTypes.string,

  isChecked: PropTypes.bool,
  isIndeterminate: PropTypes.bool,
  isDisabled: PropTypes.bool,

  onChange: PropTypes.func
};

Checkbox.defaultProps = {
  isChecked: false
};

export default Checkbox;
