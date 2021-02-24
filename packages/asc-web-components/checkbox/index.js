import React from "react";
import PropTypes from "prop-types";

import Text from "../text";
import { StyledLabel, HiddenInput } from "./styled-checkbox";
import CheckboxIndeterminateIcon from "../../../public/images/checkbox.indeterminate.react.svg";
import CheckboxCheckedIcon from "../../../public/images/checkbox.checked.react.svg";
import CheckboxIcon from "../../../public/images/checkbox.react.svg";

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
        <CheckboxIndeterminateIcon className="checkbox" />
      ) : isChecked ? (
        <CheckboxCheckedIcon className="checkbox" />
      ) : (
        <CheckboxIcon className="checkbox" />
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
    this.setState({ checked: e.target.checked });
    this.props.onChange && this.props.onChange(e);
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
    } = this.props;

    return (
      <StyledLabel
        id={id}
        style={style}
        isDisabled={isDisabled}
        isIndeterminate={isIndeterminate}
        className={className}
      >
        <HiddenInput
          type="checkbox"
          checked={this.state.checked}
          isDisabled={isDisabled}
          ref={this.ref}
          value={value}
          onChange={this.onInputChange}
        />
        <RenderCheckboxIcon {...this.props} />
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
      </StyledLabel>
    );
  }
}

Checkbox.propTypes = {
  id: PropTypes.string,
  name: PropTypes.string,
  value: PropTypes.string,
  label: PropTypes.oneOfType([PropTypes.string, PropTypes.object]),

  isChecked: PropTypes.bool,
  isIndeterminate: PropTypes.bool,
  isDisabled: PropTypes.bool,

  onChange: PropTypes.func,
  className: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  title: PropTypes.string,
  truncate: PropTypes.bool,
};

Checkbox.defaultProps = {
  isChecked: false,
  truncate: false,
};

export default Checkbox;
