import React from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import DropDown from "@appserver/components/drop-down";

import ExpanderDownIcon from "../../../../../public/images/expander-down.react.svg";
import { Caret, StyledHideFilterButton } from "../StyledFilterInput";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";

const StyledExpanderDownIcon = styled(ExpanderDownIcon)`
  ${commonIconsStyles}
  path {
    fill: "#A3A9AE";
  }
`;
class HideFilter extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();
    this.dropDownRef = React.createRef();
    this.state = {
      popoverOpen: this.props.open,
    };
  }

  onClick = (state, e) => {
    if (!state && e && this.ref.current.contains(e.target)) {
      return;
    }
    if (!this.props.isDisabled) {
      this.setState({
        popoverOpen: state,
      });
    }
  };

  handleClickOutside = (e) => {
    if (this.ref.current.contains(e.target)) return;
    this.setState({ popoverOpen: !this.state.popoverOpen });
  };

  render() {
    //console.log("HideFilter render");
    const { isDisabled, count, children } = this.props;
    const { popoverOpen } = this.state;
    return (
      <div
        className="styled-hide-filter"
        onClick={this.onClick.bind(this, !popoverOpen)}
        ref={this.ref}
        id="styled-hide-filter"
      >
        <StyledHideFilterButton id="PopoverLegacy" isDisabled={isDisabled}>
          {count}
          <Caret isOpen={popoverOpen}>
            <StyledExpanderDownIcon size="scale" />
          </Caret>
        </StyledHideFilterButton>
        <div className="dropdown-style" ref={this.dropDownRef}>
          <DropDown
            forwardedRef={this.ref}
            className="drop-down hide-filter-drop-down"
            clickOutsideAction={this.handleClickOutside}
            isDefaultMode={false}
            manualY="8px"
            open={popoverOpen}
            fixedDirection={true}
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
};
export default HideFilter;
