import React from 'react';
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import { Icons } from '../icons';
import Text from '../text';

const disableColor = '#A3A9AE';
const hoverColor = disableColor;
const internalCircleDisabledColor = '#D0D5DA';
const externalCircleDisabledColor = '#eceef1';

// eslint-disable-next-line react/prop-types, no-unused-vars
const ClearLabel = ({ spacing, isDisabled, orientation, ...props }) => <label {...props} />
const Label = styled(ClearLabel)`
  display: flex;
  align-items: center;
  position: relative;
  margin: 0;
  user-select: none;
  cursor: ${props => !props.isDisabled && 'pointer'};

  .radiobutton {
    margin-right: 4px;  
  }

  ${props =>
    props.isDisabled
      ? css`
          cursor: default;
          path:first-child {
            stroke: ${externalCircleDisabledColor};
              }
          path:nth-child(even) {
            fill: ${internalCircleDisabledColor};
          }
        `
      : css`
          cursor: pointer;

          &:hover {
            svg {
              path:first-child {
                stroke: ${hoverColor};
              }
            }
          }
        `}

  &:not(:first-child) {
    ${props =>
    (props.orientation === 'horizontal' && css`margin-left: ${props.spacing};`)};
  }

  &:not(:last-child) {
    ${props =>
    (props.orientation === 'vertical' && css`margin-bottom: ${props.spacing};`)};
  }
`;

const Input = styled.input`
  position: absolute;
  z-index: -1;
  opacity: 0.0001;
`;

// eslint-disable-next-line react/prop-types
const RadiobuttonIcon = ({ isChecked, isDisabled }) => {
  const iconName = isChecked
    ? "RadiobuttonCheckedIcon"
    : "RadiobuttonIcon";

  let newProps = {
    size: "medium",
    className: "radiobutton"
  };

  if (isDisabled) {
    newProps.isfill = true;
    newProps.color = "#F8F9F9";
  }

  return <>{React.createElement(Icons[iconName], { ...newProps })}</>;
};

class RadioButton extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      isChecked: this.props.isChecked

    };
  }

  componentDidUpdate(prevProps) {
    if (this.props.isChecked !== prevProps.isChecked) {
      this.setState({ isChecked: this.props.isChecked });
    }
  }

  render() {

    const colorProps = this.props.isDisabled ? { color: disableColor } : {};

    return (
      <Label
        orientation={this.props.orientation}
        spacing={this.props.spacing}
        isDisabled={this.props.isDisabled}
        id={this.props.id}
        className={this.props.className}
        style={this.props.style}>
        <Input type='radio'
          name={this.props.name}
          value={this.props.value}
          checked={this.props.isChecked}
          onChange={this.props.onChange ? this.props.onChange : (e) => {
            this.setState({ isChecked: true })
            this.props.onClick && this.props.onClick(e);
          }}
          disabled={this.props.isDisabled} />
        <RadiobuttonIcon {...this.props} />
        <Text
          as='span'
          fontSize={this.props.fontSize}
          fontWeight={this.props.fontWeight}
          {...colorProps}
        >
          {this.props.label || this.props.value}
        </Text>
      </Label>
    );
  }
}

RadioButton.propTypes = {
  isChecked: PropTypes.bool,
  isDisabled: PropTypes.bool,
  label: PropTypes.string,
  fontSize: PropTypes.string,
  fontWeight: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  name: PropTypes.string.isRequired,
  onChange: PropTypes.func,
  onClick: PropTypes.func,
  value: PropTypes.string.isRequired,
  spacing: PropTypes.string,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  orientation: PropTypes.oneOf(['horizontal', 'vertical'])
}

RadioButton.defaultProps = {
  isChecked: false,
  isDisabled: false,
}

export default RadioButton;
