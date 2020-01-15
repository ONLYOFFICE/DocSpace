import React from "react";
import PropTypes from "prop-types";
import Selector from "./sub-components/Selector";
import { utils, Backdrop, DropDown, Aside } from "asc-web-components";
import throttle from "lodash/throttle";
const { desktop } = utils.device;

const displayTypes = ["dropdown", "aside", "auto"];
const sizes = ["compact", "full"];

class AdvancedSelector extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = { 
      displayType: this.getTypeByWidth() 
    };

    this.throttledResize = throttle(this.resize, 300);
  }

  componentDidMount() {
    if(this.props.isOpen) {
      window.addEventListener("resize", this.throttledResize);
    }
  }

  resize = () => {
    if (this.props.displayType !== "auto") 
      return;

    const type = this.getTypeByWidth();
    
    if (type === this.state.displayType) 
      return;
    
    this.setState({ displayType: type });
  };

  onClose = (e) => {
    //console.log("onClose");
    //this.setState({ isOpen: false });
    this.props.onCancel && this.props.onCancel(e);
  };

  componentDidUpdate(prevProps) {
    if(this.props.isOpen !== prevProps.isOpen) {
      console.log(`ADSelector#${this.props.id} componentDidUpdate isOpen=${this.props.isOpen}`);
      if(this.props.isOpen) {
        this.resize();
        window.addEventListener("resize", this.throttledResize);
      }
      else {
        this.throttledResize.cancel();
        window.removeEventListener("resize", this.throttledResize);
      }
    }

    if (this.props.displayType !== prevProps.displayType) {
      console.log(`ADSelector#${this.props.id} componentDidUpdate displayType=${this.props.displayType}`);
      this.setState({ displayType: this.getTypeByWidth() });
    }
  }

  componentWillUnmount() {
    if(this.throttledResize)
    {
      this.throttledResize && this.throttledResize.cancel();
      window.removeEventListener("resize", this.throttledResize);
    }
  }

  getTypeByWidth = () => {
    const displayType = this.props.displayType !== "auto" 
      ? this.props.displayType 
      : window.innerWidth < desktop.match(/\d+/)[0] ? "aside" : "dropdown";

    //console.log("AdvancedSelector2 displayType", displayType);

    return displayType;
  };

  render() {
    const { displayType } = this.state;
    const { isOpen, id, className, style } = this.props;

    console.log(`AdvancedSelector render() isOpen=${isOpen} displayType=${displayType}`);

    return (
      <div ref={this.ref} id={id} className={className} style={style}>
        {displayType === "dropdown" 
        ? 
            <DropDown open={isOpen} className="dropdown-container" clickOutsideAction={this.onClose}>
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
  id: PropTypes.string,
  className: PropTypes.oneOf([PropTypes.string, PropTypes.array]),
  style: PropTypes.object,
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
  allowGroupSelection: PropTypes.bool,
  allowCreation: PropTypes.bool,
  allowAnyClickClose: PropTypes.bool,
  hasNextPage: PropTypes.bool,
  isNextPageLoading: PropTypes.bool,

  onSearchChanged: PropTypes.func,
  onSelect: PropTypes.func,
  onGroupChange: PropTypes.func,
  onCancel: PropTypes.func,
  onAddNewClick: PropTypes.func,
  loadNextPage: PropTypes.func
};

AdvancedSelector.defaultProps = {
  isMultiSelect: false,
  size: "full",
  buttonLabel: "Add members",
  selectAllLabel: "Select all",
  allowGroupSelection: false,
  allowAnyClickClose: true,
  displayType: "auto",
  options: []
};

export default AdvancedSelector;
