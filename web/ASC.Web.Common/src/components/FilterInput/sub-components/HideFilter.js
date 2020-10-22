import React from "react";
import { Icons, DropDown } from "asc-web-components";
import PropTypes from "prop-types";
import { Caret, StyledHideFilterButton } from "../StyledFilterInput";

class HideFilter extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();
    this.dropDownRef = React.createRef();
  }

  onClick = (e) => {
    const { setShowHiddenFilter, open, isDisabled } = this.props;
    if (e && this.dropDownRef.current.contains(e.target)) {
      return;
    }
    if (!isDisabled) {
      setShowHiddenFilter(!open);
    }
  };

  handleClickOutside = (e) => {
    if (this.ref.current.contains(e.target)) return;
    this.props.setShowHiddenFilter(false);
  };

  render() {
    //console.log("HideFilter render");
    const { isDisabled, count, children, open } = this.props;

    return (
      <div className="styled-hide-filter" onClick={this.onClick} ref={this.ref}>
        <StyledHideFilterButton id="PopoverLegacy" isDisabled={isDisabled}>
          {count}
          <Caret isOpen={false}>
            <Icons.ExpanderDownIcon
              color="#A3A9AE"
              isfill={true}
              size="scale"
            />
          </Caret>
        </StyledHideFilterButton>

        <div className="filter_dropdown-style" ref={this.dropDownRef}>
          <DropDown
            className="filter_drop-down"
            clickOutsideAction={this.handleClickOutside}
            manualY="8px"
            open={open}
          >
            {children}
          </DropDown>
        </div>
      </div>
    );
  }
}
HideFilter.propTypes = {
  children: PropTypes.any,
  count: PropTypes.number,
  isDisabled: PropTypes.bool,
  open: PropTypes.bool,
  setShowHiddenFilter: PropTypes.func,
};
export default HideFilter;
