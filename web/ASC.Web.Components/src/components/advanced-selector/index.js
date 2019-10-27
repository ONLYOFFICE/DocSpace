import React from "react";
import PropTypes from "prop-types";
import DropDown from "../drop-down";
import Aside from "../layout/sub-components/aside";
import ADSelectorBody from "./sub-components/body";

const displayTypes = ["dropdown", "aside"];

class AdvancedSelector extends React.Component {
  render() {
    const { displayType, isOpen } = this.props;
    //console.log("AdvancedSelector render()");

    return (
        displayType === "dropdown" 
        ? <DropDown opened={isOpen} className="dropdown-container">
            <ADSelectorBody {...this.props} />
          </DropDown>
        : <Aside visible={isOpen} scale={false} className="aside-container">
            <ADSelectorBody {...this.props} />
          </Aside>
    );
  }
}

AdvancedSelector.propTypes = {
  value: PropTypes.string,
  placeholder: PropTypes.string,
  isMultiSelect: PropTypes.bool,
  size: PropTypes.oneOf(["compact", "full"]),
  maxHeight: PropTypes.number,
  isDisabled: PropTypes.bool,
  onSearchChanged: PropTypes.func,
  options: PropTypes.array,
  selectedOptions: PropTypes.array,
  groups: PropTypes.array,
  selectedGroups: PropTypes.array,
  selectedAll: PropTypes.bool,
  selectAllLabel: PropTypes.string,
  buttonLabel: PropTypes.string,
  onSelect: PropTypes.func,
  onChangeGroup: PropTypes.func,
  onCancel: PropTypes.func,
  isOpen: PropTypes.bool,
  allowCreation: PropTypes.bool,
  onAddNewClick: PropTypes.func,
  allowAnyClickClose: PropTypes.bool,
  displayType: PropTypes.oneOf(displayTypes),
  hasNextPage: PropTypes.bool,
  isNextPageLoading: PropTypes.bool,
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
