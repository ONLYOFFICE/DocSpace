import React, { memo } from "react";
import PropTypes from "prop-types";

import onClickOutside from "react-onclickoutside";
import { isMobile } from "react-device-detect";
import Portal from "../portal";
import DomHelpers from "../utils/domHelpers";

import DropDownItem from "../drop-down-item";
import Backdrop from "../backdrop";
import StyledDropdown from "./styled-drop-down";
import VirtualList from "./VirtualList";
/* eslint-disable react/prop-types, react/display-name */

const Row = memo(({ data, index, style }) => {
  const { children, theme, activedescendant, handleMouseMove } = data;

  const option = children[index];

  const separator = option?.props?.isSeparator
    ? { width: `calc(100% - 32px)`, height: `1px` }
    : {};
  const newStyle = { ...style, ...separator };

  return (
    <DropDownItem
      theme={theme}
      // eslint-disable-next-line react/prop-types
      {...option?.props}
      noHover
      style={newStyle}
      onMouseMove={() => {
        handleMouseMove(index);
      }}
      isActiveDescendant={activedescendant === index}
    />
  );
});

class DropDown extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      directionX: props.directionX,
      directionY: props.directionY,
      manualY: props.manualY,
    };

    this.dropDownRef = React.createRef();
  }

  componentDidMount() {
    if (this.props.open) {
      this.props.enableOnClickOutside();
      if (this.props.isDefaultMode) {
        return setTimeout(() => this.checkPositionPortal(), 0); // ditry, but need after render for ref
      }
      return this.checkPosition();
    }
  }

  componentWillUnmount() {
    this.props.disableOnClickOutside();
    this.unbindDocumentResizeListener();
  }

  componentDidUpdate(prevProps) {
    if (this.props.open !== prevProps.open) {
      if (this.props.open) {
        this.props.enableOnClickOutside(); //fixed main-button-mobile click, remove !isMobile if have dd problem
        this.bindDocumentResizeListener();
        if (this.props.isDefaultMode) {
          return this.checkPositionPortal();
        }
        return this.checkPosition();
      } else {
        this.props.disableOnClickOutside();
      }
    }
  }

  handleClickOutside = (e) => {
    if (e.type !== "touchstart") {
      e.preventDefault();
    }

    this.toggleDropDown(e);
  };

  toggleDropDown = (e) => {
    this.props.clickOutsideAction &&
      this.props.clickOutsideAction(e, !this.props.open);
  };

  bindDocumentResizeListener() {
    if (!this.documentResizeListener) {
      this.documentResizeListener = (e) => {
        if (this.props.open) {
          if (this.props.isDefaultMode) {
            this.checkPositionPortal();
          } else {
            this.checkPosition();
          }
        }
      };

      window.addEventListener("resize", this.documentResizeListener);
    }
  }

  unbindDocumentResizeListener() {
    if (this.documentResizeListener) {
      window.removeEventListener("resize", this.documentResizeListener);
      this.documentResizeListener = null;
    }
  }

  checkPosition = () => {
    if (!this.dropDownRef.current || this.props.fixedDirection) return;
    const { smallSectionWidth, forwardedRef } = this.props;
    const { manualY } = this.state;

    const rects = this.dropDownRef.current.getBoundingClientRect();
    const parentRects = forwardedRef?.current?.getBoundingClientRect();

    const container = DomHelpers.getViewport();

    const dimensions = parentRects
      ? {
          toTopCorner: parentRects.top,
          parentHeight: parentRects.height,
          containerHeight: !parentRects.top,
        }
      : {
          toTopCorner: rects.top,
          parentHeight: 42,
          containerHeight: container.height,
        };

    const left = rects.left < 0 && rects.width < container.width;
    const right =
      rects.width &&
      rects.left < (rects.width || 250) &&
      rects.left > rects.width &&
      rects.width < container.width;
    const top =
      rects.bottom > dimensions.containerHeight &&
      dimensions.toTopCorner > rects.height;
    const bottom = rects.top < 0;

    const x = left
      ? "left"
      : right || smallSectionWidth
      ? "right"
      : this.state.directionX;
    const y = bottom ? "bottom" : top ? "top" : this.state.directionY;

    const mY = top ? `${dimensions.parentHeight}px` : manualY;

    this.setState({
      directionX: x,
      directionY: y,
      manualY: mY,
      width: this.dropDownRef ? this.dropDownRef.current.offsetWidth : 240,
    });
  };

  checkPositionPortal = () => {
    const parent = this.props.forwardedRef;
    if (!parent?.current || this.props.fixedDirection) return;

    const rects = parent.current.getBoundingClientRect();

    let dropDownHeight = this.dropDownRef.current?.offsetParent
      ? this.dropDownRef.current.offsetHeight
      : DomHelpers.getHiddenElementOuterHeight(this.dropDownRef.current);

    let left = rects.left;
    let bottom = rects.bottom;

    const viewport = DomHelpers.getViewport();
    const dropDownRects = this.dropDownRef.current.getBoundingClientRect();

    if (
      this.props.directionY === "top" ||
      (this.props.directionY === "both" &&
        rects.bottom + dropDownHeight > viewport.height)
    ) {
      bottom -= parent.current.clientHeight + dropDownHeight;
    }

    if (this.props.right) {
      this.dropDownRef.current.style.right = this.props.right;
    } else if (this.props.directionX === "right") {
      this.dropDownRef.current.style.left =
        rects.right - this.dropDownRef.current.clientWidth + "px";
    } else if (rects.left + dropDownRects.width > viewport.width) {
      if (rects.right - dropDownRects.width < 0) {
        this.dropDownRef.current.style.left = 0 + "px";
      } else {
        this.dropDownRef.current.style.left =
          rects.right - this.dropDownRef.current.clientWidth + "px";
      }
    } else {
      this.dropDownRef.current.style.left = left + this.props.offsetLeft + "px";
    }

    this.dropDownRef.current.style.top = this.props.top || bottom + "px";

    this.setState({
      directionX: this.props.directionX,
      directionY: this.props.directionY,
      width: this.dropDownRef ? this.dropDownRef.current.offsetWidth : 240,
    });
  };

  getItemHeight = (item) => {
    const isTablet = window.innerWidth < 1024; //TODO: Make some better

    //if (item && item.props.disabled) return 0;

    let height = item?.props.height;
    let heightTablet = item?.props.heightTablet;

    if (item && item.props.isSeparator) {
      return isTablet ? 16 : 12;
    }

    return isTablet ? heightTablet : height;
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

  renderDropDown() {
    const {
      maxHeight,
      children,
      showDisabledItems,
      theme,
      isMobileView,
      isNoFixedHeightOptions,
      open,
      enableKeyboardEvents,
    } = this.props;
    const { directionX, directionY, width, manualY } = this.state;

    let cleanChildren = children;
    let itemCount = children.length;

    if (!showDisabledItems) {
      cleanChildren = this.hideDisabledItems();
      if (cleanChildren) itemCount = cleanChildren.length;
    }

    const rowHeights = React.Children.map(cleanChildren, (child) =>
      this.getItemHeight(child)
    );
    const getItemSize = (index) => rowHeights[index];
    const fullHeight = cleanChildren && rowHeights.reduce((a, b) => a + b, 0);
    const calculatedHeight =
      fullHeight > 0 && fullHeight < maxHeight ? fullHeight : maxHeight;
    const dropDownMaxHeightProp = maxHeight
      ? { height: calculatedHeight + "px" }
      : {};

    return (
      <StyledDropdown
        ref={this.dropDownRef}
        {...this.props}
        directionX={directionX}
        directionY={directionY}
        manualY={manualY}
        isMobileView={isMobileView}
        itemCount={itemCount}
        {...dropDownMaxHeightProp}
      >
        <VirtualList
          Row={Row}
          theme={theme}
          width={width}
          itemCount={itemCount}
          maxHeight={maxHeight}
          cleanChildren={cleanChildren}
          calculatedHeight={calculatedHeight}
          isNoFixedHeightOptions={isNoFixedHeightOptions}
          getItemSize={getItemSize}
          children={children}
          isOpen={open}
          enableKeyboardEvents={enableKeyboardEvents}
        />
      </StyledDropdown>
    );
  }
  render() {
    const { isDefaultMode } = this.props;
    const element = this.renderDropDown();
    if (isDefaultMode) {
      return <Portal element={element} appendTo={this.props.appendTo} />;
    }

    return element;
  }
}

const EnhancedComponent = onClickOutside(DropDown);

class DropDownContainer extends React.Component {
  toggleDropDown = () => {
    this.props.clickOutsideAction({}, !this.props.open);
  };
  render() {
    const {
      withBackdrop = true,
      withBlur = false,
      open,
      isAside,
      withBackground,
      eventTypes,
    } = this.props;
    const eventTypesProp = isMobile
      ? { eventTypes: ["click, touchend"] }
      : eventTypes
      ? { eventTypes }
      : {};

    return (
      <>
        {withBackdrop ? (
          <Backdrop
            visible={open}
            zIndex={199}
            onClick={this.toggleDropDown}
            withoutBlur={!withBlur}
            isAside={isAside}
            withBackground={withBackground}
          />
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
  directionY: PropTypes.oneOf(["bottom", "top", "both"]),
  /** Accepts id */
  id: PropTypes.string,
  /** Required for specifying the exact width of the component, for example, 100% */
  manualWidth: PropTypes.string,
  /** Required for specifying the exact distance from the parent component */
  manualX: PropTypes.string,
  /** Required for specifying the exact distance from the parent component */
  manualY: PropTypes.string,
  /** Required if the scrollbar is displayed */
  maxHeight: PropTypes.number,
  /** Sets the dropdown to be opened */
  open: PropTypes.bool,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Used to display backdrop */
  withBackdrop: PropTypes.bool,
  /** Count of columns */
  columnCount: PropTypes.number,
  /** Sets the disabled items to display */
  showDisabledItems: PropTypes.bool,
  forwardedRef: PropTypes.shape({ current: PropTypes.any }),
  /** Sets the operation mode of the component. The default option is set to portal mode */
  isDefaultMode: PropTypes.bool,
  /** Used to open people and group selectors correctly when the section width is small */
  smallSectionWidth: PropTypes.bool,
  /** Disables check position. Used to set the direction explicitly */
  fixedDirection: PropTypes.bool,
  /** Enables blur for backdrop */
  withBlur: PropTypes.bool,
  /** Specifies the offset */
  offsetLeft: PropTypes.number,
};

DropDownContainer.defaultProps = {
  directionX: "left",
  directionY: "bottom",
  withBackdrop: true,
  showDisabledItems: false,
  isDefaultMode: true,
  fixedDirection: false,
  offsetLeft: 0,
  enableKeyboardEvents: true,
};

export default DropDownContainer;
