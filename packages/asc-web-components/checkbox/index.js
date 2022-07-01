import React from "react";
import PropTypes from "prop-types";

import Text from "../text";
import { StyledLabel, HiddenInput } from "./styled-checkbox";
import CheckboxIndeterminateIcon from "./svg/checkbox.indeterminate.react.svg";
import CheckboxCheckedIcon from "./svg/checkbox.checked.react.svg";
import CheckboxIcon from "./svg/checkbox.react.svg";

// eslint-disable-next-line react/prop-types
const RenderCheckboxIcon = ({ isChecked, isIndeterminate }) => {
  // let newProps = {
  //   size: "medium",
  //   className: "checkbox",
  // };

  // return <>{React.createElement(Icons[iconName], { ...newProps })}</>;

  return (
    <>
      {isIndeterminate ? (
        <CheckboxIndeterminateIcon className="checkbox not-selectable" />
      ) : isChecked ? (
        <CheckboxCheckedIcon className="checkbox not-selectable" />
      ) : (
        <CheckboxIcon className="checkbox not-selectable" />
      )}
    </>
  );
};

class Checkbox extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      checked: props.isChecked,
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
    if (this.props.isDisabled) return e.preventDefault();
    e.stopPropagation();
    this.setState({ checked: e.target.checked });
    this.props.onChange && this.props.onChange(e);
  }

  onClick(e) {
    return e.preventDefault();
  }

  render() {
    //console.log("Checkbox render");
    const {
      isDisabled,
      isIndeterminate,
      id,
      className,
      label,
      style,
      value,
      title,
      truncate,
      name,
      helpButton,
      onChange,
      isChecked,
      ...rest
    } = this.props;

    return (
      <>
        <StyledLabel
          id={id}
          style={style}
          isDisabled={isDisabled}
          isIndeterminate={isIndeterminate}
          className={className}
          title={title}
        >
          <HiddenInput
            name={name}
            type="checkbox"
            checked={this.state.checked}
            isDisabled={isDisabled}
            ref={this.ref}
            value={value}
            onChange={this.onInputChange}
            {...rest}
          />
          <RenderCheckboxIcon {...this.props} />
          <div className="wrapper">
            {this.props.label && (
              <Text
                as="span"
                title={title}
                isDisabled={isDisabled}
                truncate={truncate}
                className="checkbox-text"
              >
                {label}
              </Text>
            )}
            {helpButton && (
              <span className="help-button" onClick={this.onClick}>
                {helpButton}
              </span>
            )}
          </div>
        </StyledLabel>
      </>
    );
  }
}

Checkbox.propTypes = {
  /** Used as HTML id property */
  id: PropTypes.string,
  /** Used as HTML `name` property */
  name: PropTypes.string,
  /** Value of the input */
  value: PropTypes.string,
  /** Label of the input */
  label: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),
  /** The checked property sets the checked state of a checkbox */
  isChecked: PropTypes.bool,
  /** If true, this state is shown as a rectangle in the checkbox */
  isIndeterminate: PropTypes.bool,
  /** Disables the Checkbox input */
  isDisabled: PropTypes.bool,
  /** Will be triggered whenever an CheckboxInput is clicked */
  onChange: PropTypes.func,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Title */
  title: PropTypes.string,
  /** Disables word wrapping */
  truncate: PropTypes.bool,
  /** Help button render */
  helpButton: PropTypes.any,
};

Checkbox.defaultProps = {
  isChecked: false,
  truncate: false,
};

export default React.memo(Checkbox);
