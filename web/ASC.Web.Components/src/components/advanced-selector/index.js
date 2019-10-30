import React from "react";
import PropTypes from "prop-types";
import DropDown from "../drop-down";
import Aside from "../layout/sub-components/aside";
import ADSelector from "./sub-components/selector";

const displayTypes = ["dropdown", "aside"];
const sizes = ["compact", "full"];

class AdvancedSelector extends React.Component {
  render() {
    const { displayType, isOpen } = this.props;
    //console.log("AdvancedSelector render()");

    return (
        displayType === "dropdown" 
        ? <DropDown opened={isOpen} className="dropdown-container">
            <ADSelector {...this.props} />
          </DropDown>
        : <Aside visible={isOpen} scale={false} className="aside-container">
            <ADSelector {...this.props} />
          </Aside>
    );
  }
}

AdvancedSelector.propTypes = {
  options: PropTypes.array,
  selectedOptions: PropTypes.array,
  groups: PropTypes.array,
  selectedGroups: PropTypes.array,

  value: PropTypes.string,
  placeholder: PropTypes.string,
  selectAllLabel: PropTypes.string,
  buttonLabel: PropTypes.string,

  size: PropTypes.oneOf(sizes),
  displayType: PropTypes.oneOf(displayTypes),

  maxHeight: PropTypes.number,

  isMultiSelect: PropTypes.bool,
  isDisabled: PropTypes.bool,
  selectedAll: PropTypes.bool,
  isOpen: PropTypes.bool,
  allowCreation: PropTypes.bool,
  allowAnyClickClose: PropTypes.bool,
  hasNextPage: PropTypes.bool,
  isNextPageLoading: PropTypes.bool,

  onSearchChanged: PropTypes.func,
  onSelect: PropTypes.func,
  onGroupChange: PropTypes.func,
  onCancel: PropTypes.func,
  onAddNewClick: PropTypes.func,
  loadNextPage: PropTypes.func,
};

AdvancedSelector.defaultProps = {
  isMultiSelect: false,
  size: "compact",
  buttonLabel: "Add members",
  selectAllLabel: "Select all",
  allowAnyClickClose: true,
  displayType: "dropdown",
  options: []
};

export default AdvancedSelector;
