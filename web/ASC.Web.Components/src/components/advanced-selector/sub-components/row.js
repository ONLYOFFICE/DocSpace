import React from "react";
import PropTypes from "prop-types";
import Checkbox from "../../checkbox";
import Link from "../../link";

class ADSelectorRow extends React.Component {
  render() {
    const {
      label,
      isChecked,
      style,
      onChange,
      onSelect,
      isMultiSelect,
      isSelected
    } = this.props;

    console.log("ADSelectorRow render", label, isChecked);

    return (
      <div className={`option ${isSelected && "selected"}`} style={style}>
        {isMultiSelect ? (
          <Checkbox
            label={label}
            isChecked={isChecked}
            className="option_checkbox"
            onChange={onChange}
          />
        ) : (
          <Link
            as="span"
            truncate={true}
            className="option_link"
            onClick={onSelect}
          >
            {label}
          </Link>
        )}
      </div>
    );
  }
}

ADSelectorRow.propTypes = {
  label: PropTypes.string,
  isSelected: PropTypes.bool,
  isChecked: PropTypes.bool,
  isMultiSelect: PropTypes.bool,
  style: PropTypes.object,
  onChange: PropTypes.func,
  onSelect: PropTypes.func
};

export default ADSelectorRow;
