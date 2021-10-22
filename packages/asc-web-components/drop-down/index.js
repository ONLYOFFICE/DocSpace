import React, { memo } from "react";
import PropTypes from "prop-types";
import { VariableSizeList } from "react-window";
import onClickOutside from "react-onclickoutside";
import { isMobile } from "react-device-detect";

import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import DropDownItem from "../drop-down-item";
import Backdrop from "../backdrop";
import StyledDropdown from "./styled-drop-down";

// eslint-disable-next-line react/display-name, react/prop-types
const Row = memo(({ data, index, style }) => {
  const option = data[index];
  // eslint-disable-next-line react/prop-types
  const separator = option.props.isSeparator
    ? { width: `calc(100% - 32px)`, height: `1px` }
    : {};
  const newStyle = { ...style, ...separator };

  return (
    <DropDownItem
      // eslint-disable-next-line react/prop-types
      {...option.props}
      style={newStyle}
    />
  );
});

class DropDown extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      width: this.dropDownRef ? this.dropDownRef.current.offsetWidth : 240,
      directionX: props.directionX,
      directionY: props.directionY,
    };

    this.dropDownRef = React.createRef();
  }

  componentDidMount() {
    if (this.props.open) {
      this.props.enableOnClickOutside();
      this.checkPosition();
    }
  }

  componentWillUnmount() {
    this.props.disableOnClickOutside();
  }

  componentDidUpdate(prevProps) {
    if (this.props.open !== prevProps.open) {
      if (this.props.open) {
        this.props.enableOnClickOutside();
        this.checkPosition();
      } else {
        this.props.disableOnClickOutside();
      }
    }
  }

  handleClickOutside = (e) => {
    e.preventDefault();
    this.toggleDropDown(e);
  };

  toggleDropDown = (e) => {
    this.props.clickOutsideAction &&
      this.props.clickOutsideAction(e, !this.props.open);
  };

  checkPosition = () => {
    if (!this.dropDownRef.current) return;

    const rects = this.dropDownRef.current.getBoundingClientRect();
    const container = { width: window.innerWidth, height: window.innerHeight };
    const left = rects.left < 0 && rects.width < container.width;
    const right =
      rects.width &&
      rects.left < (rects.width || 250) &&
      rects.left > rects.width &&
      rects.width < container.width;
    const top = rects.bottom > container.height && rects.top > rects.height;
    const bottom = rects.top < 0;
    const x = left ? "left" : right ? "right" : this.state.directionX;
    const y = bottom ? "bottom" : top ? "top" : this.state.directionY;

    this.setState({
      directionX: x,
      directionY: y,
      width: rects.width,
    });
  };

  getItemHeight = (item) => {
    const isTablet = window.innerWidth < 1024; //TODO: Make some better

    if (item && item.props.isSeparator) return isTablet ? 16 : 12;

    return isTablet ? 36 : 32;
  };
  hideDisabledItems = () => {
    if (React.Children.count(this.props.children) > 0) {
      const { children } = this.props;
      const enabledChildren = React.Children.map(children, (child) => {
        if (child && !child.props.disabled) return child;
      });

      const sizeEnabledChildren = enabledChildren.length;

      const cleanChildren = React.Children.map(
        enabledChildren,
        (child, index) => {
          if (!child.props.isSeparator) return child;
          if (index !== 0 && index !== sizeEnabledChildren - 1) return child;
        }
      );

      return cleanChildren;
    }
  };

  render() {
    const { maxHeight, children, showDisabledItems } = this.props;
    const { directionX, directionY, width } = this.state;
    let cleanChildren;

    const rowHeights = React.Children.map(children, (child) =>
      this.getItemHeight(child)
    );
    const getItemSize = (index) => rowHeights[index];
    const fullHeight = children && rowHeights.reduce((a, b) => a + b, 0);
    const calculatedHeight =
      fullHeight > 0 && fullHeight < maxHeight ? fullHeight : maxHeight;
    const dropDownMaxHeightProp = maxHeight
      ? { height: calculatedHeight + "px" }
      : {};
    //console.log("DropDown render", this.props);

    if (!showDisabledItems) cleanChildren = this.hideDisabledItems();

    return (
      <StyledDropdown
        ref={this.dropDownRef}
        {...this.props}
        directionX={directionX}
        directionY={directionY}
        {...dropDownMaxHeightProp}
      >
        {maxHeight ? (
          <VariableSizeList
            height={calculatedHeight}
            width={width}
            itemSize={getItemSize}
            itemCount={children.length}
            itemData={children}
            outerElementType={CustomScrollbarsVirtualList}
          >
            {Row}
          </VariableSizeList>
        ) : cleanChildren ? (
          cleanChildren
        ) : (
          children
        )}
      </StyledDropdown>
    );
  }
}

const EnhancedComponent = onClickOutside(DropDown);

class DropDownContainer extends React.Component {
  toggleDropDown = (e) => {
    this.props.clickOutsideAction({}, !this.props.open);
  };
  render() {
    const { withBackdrop = true, open } = this.props;
    const eventTypesProp = isMobile ? { eventTypes: ["touchend"] } : {};

    return (
      <>
        {withBackdrop ? (
          <Backdrop visible={open} zIndex={199} onClick={this.toggleDropDown} />
        ) : null}
        <EnhancedComponent
          {...eventTypesProp}
          disableOnClickOutside={true}
          {...this.props}
        />
      </>
    );
  }
}

DropDown.propTypes = {
  disableOnClickOutside: PropTypes.func,
  enableOnClickOutside: PropTypes.func,
};

DropDownContainer.propTypes = {
  /** Children elements */
  children: PropTypes.any,
  /** Accepts class */
  className: PropTypes.string,
  /** Required for determining a click outside DropDown with the withBackdrop parameter */
  clickOutsideAction: PropTypes.func,
  /** Sets the opening direction relative to the parent */
  directionX: PropTypes.oneOf(["left", "right"]), //TODO: make more informative
  /** Sets the opening direction relative to the parent */
  directionY: PropTypes.oneOf(["bottom", "top"]),
  /** Accepts id */
  id: PropTypes.string,
  /** Required if you need to specify the exact width of the component, for example 100% */
  manualWidth: PropTypes.string,
  /** Required if you need to specify the exact distance from the parent component */
  manualX: PropTypes.string,
  /** Required if you need to specify the exact distance from the parent component */
  manualY: PropTypes.string,
  /** Required if the scrollbar is displayed */
  maxHeight: PropTypes.number,
  /** Tells when the dropdown should be opened */
  open: PropTypes.bool,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Used to display backdrop */
  withBackdrop: PropTypes.bool,
  /** Count of columns */
  columnCount: PropTypes.number,
  /** Display disabled items or not */
  showDisabledItems: PropTypes.bool,
};

DropDownContainer.defaultProps = {
  directionX: "left",
  directionY: "bottom",
  withBackdrop: true,
  showDisabledItems: false,
};

export default DropDownContainer;
