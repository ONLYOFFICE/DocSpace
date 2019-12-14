import React from "react";
import PropTypes from "prop-types";
import Selector from "./sub-components/Selector";
import { utils, Backdrop, DropDown, Aside } from "asc-web-components";
import throttle from "lodash/throttle";
import onClickOutside from "react-onclickoutside";
const { desktop } = utils.device;

const displayTypes = ["dropdown", "aside", "auto"];
const sizes = ["compact", "full"];

class AdvancedSelector extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = { 
      isOpen: props.isOpen, 
      displayType: this.getTypeByWidth() 
    };
  }

  handleClickOutside = evt => {
    // ..handling code goes here...
    console.log("handleClickOutside", evt);
    this.onClose();
  };

  componentDidMount() {
    if(this.state.isOpen) {
      this.throttledResize = throttle(this.resize, 300);
      //handleAnyClick(true, this.handleClick);
      window.addEventListener("resize", this.throttledResize);
    }
  }

  resize = () => {
    if (this.props.displayType !== "auto") return;
    const type = this.getTypeByWidth();
    if (type === this.state.displayType) return;
    this.setState({ displayType: type });
  };

  onClose = () => {
    console.log("onClose");
    //this.setState({ isOpen: false });
    this.props.onCancel && this.props.onCancel();
  };

  componentDidUpdate(prevProps) {
    if (this.props.displayType !== prevProps.displayType) {
      this.setState({ displayType: this.getTypeByWidth() });
    }

    if(this.props.isOpen !== prevProps.isOpen) {
      console.log("componentDidUpdate isOpen changed", this.props.isOpen);
      this.setState({ 
        isOpen: this.props.isOpen, 
        displayType: this.getTypeByWidth() 
      }, () => {
        if(this.state.isOpen)
          this.throttledResize = throttle(this.resize, 300);
        else
          this.throttledResize.cancel();
      });
    }
  }

  componentWillUnmount() {
    this.throttledResize && this.throttledResize.cancel();
    window.removeEventListener("resize", this.throttledResize);
  }

  getTypeByWidth = () => {
    const displayType = this.props.displayType !== "auto" 
      ? this.props.displayType 
      : window.innerWidth < desktop.match(/\d+/)[0] ? "aside" : "dropdown";

    //console.log("AdvancedSelector2 displayType", displayType);

    return displayType;
  };

  render() {
    const { isOpen, displayType } = this.state;
    console.log(`AdvancedSelector render() isOpen=${isOpen} displayType=${displayType}`);

    return (
      <div ref={this.ref}>
        {displayType === "dropdown" 
        ? 
            <DropDown opened={isOpen} className="dropdown-container">
              <Selector {...this.props} displayType={displayType} />
            </DropDown>
        : 
        <>
          <Backdrop onClick={this.onClose} visible={isOpen} zIndex={310} />
          <Aside visible={isOpen} scale={false} className="aside-container">
            <Selector {...this.props} displayType={displayType} />
          </Aside>
        </>
        }
      </div>
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
  size: "full",
  buttonLabel: "Add members",
  selectAllLabel: "Select all",
  allowAnyClickClose: true,
  displayType: "auto",
  options: []
};

export default onClickOutside(AdvancedSelector);
