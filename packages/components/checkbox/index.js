import React from "react";
import PropTypes from "prop-types";

import Text from "../text";
import { StyledLabel, HiddenInput } from "./styled-checkbox";
import CheckboxIndeterminateIcon from "PUBLIC_DIR/images/checkbox.indeterminate.react.svg";
import CheckboxCheckedIcon from "PUBLIC_DIR/images/checkbox.checked.react.svg";
import CheckboxIcon from "PUBLIC_DIR/images/checkbox.react.svg";

// eslint-disable-next-line react/prop-types
const RenderCheckboxIcon = ({ isChecked, isIndeterminate, tabIndex }) => {
  // let newProps = {
  //   size: "medium",
  //   className: "checkbox",
  // };

  // return <>{React.createElement(Icons[iconName], { ...newProps })}</>;

  return (
    <>
      {isIndeterminate ? (
        <CheckboxIndeterminateIcon
          tabIndex={tabIndex}
          className="checkbox not-selectable"
        />
      ) : isChecked ? (
        <CheckboxCheckedIcon
          tabIndex={tabIndex}
          className="checkbox not-selectable"
        />
      ) : (
        <CheckboxIcon tabIndex={tabIndex} className="checkbox not-selectable" />
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
      tabIndex,
      hasError,
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
          hasError={hasError}
        >
          <HiddenInput
            name={name}
            type="checkbox"
            checked={this.state.checked}
            isDisabled={isDisabled}
            ref={this.ref}
            value={value}
            onChange={this.onInputChange}
            tabIndex={-1}
            {...rest}
          />
          <RenderCheckboxIcon tabIndex={tabIndex} {...this.props} />
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
  /** Sets the checked state of the checkbox */
  isChecked: PropTypes.bool,
  /** The state is displayed as a rectangle in the checkbox when set to true */
  isIndeterminate: PropTypes.bool,
  /** Disables the Checkbox input */
  isDisabled: PropTypes.bool,
  /** Is triggered whenever the CheckboxInput is clicked */
  onChange: PropTypes.func,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Title */
  title: PropTypes.string,
  /** Disables word wrapping */
  truncate: PropTypes.bool,
  /** Renders the help button */
  helpButton: PropTypes.any,
  /** Checkbox tab index */
  tabIndex: PropTypes.number,
  /** Notifies if the error occurs */
  hasError: PropTypes.bool,
};

Checkbox.defaultProps = {
  isChecked: false,
  truncate: false,
  tabIndex: -1,
  hasError: false,
};

export default React.memo(Checkbox);
