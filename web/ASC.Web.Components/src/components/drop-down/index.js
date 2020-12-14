import React, { memo } from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";
import DropDownItem from "../drop-down-item";
import Backdrop from "../backdrop";
import { VariableSizeList } from "react-window";
import onClickOutside from "react-onclickoutside";

const StyledDropdown = styled.div`
  font-family: "Open Sans", sans-serif, Arial;
  font-style: normal;
  font-weight: 600;
  font-size: 13px;
  ${(props) =>
    props.maxHeight &&
    `
      max-height: ${props.maxHeight}px;
      overflow-y: auto;
    `}
  height: fit-content;
  position: absolute;
  ${(props) => props.manualWidth && `width: ${props.manualWidth};`}
  ${(props) =>
    props.directionY === "top" &&
    css`
      bottom: ${(props) => (props.manualY ? props.manualY : "auto")};
    `}
    ${(props) =>
    props.directionY === "bottom" &&
    css`
      top: ${(props) => (props.manualY ? props.manualY : "auto")};
    `}
    ${(props) =>
    props.directionX === "right" &&
    css`
      right: ${(props) => (props.manualX ? props.manualX : "0px")};
    `}
    ${(props) =>
    props.directionX === "left" &&
    css`
      left: ${(props) => (props.manualX ? props.manualX : "0px")};
    `}
    z-index: 150;
  display: ${(props) => (props.open ? "block" : "none")};
  background: #ffffff;
  border-radius: 6px;
  -moz-border-radius: 6px;
  -webkit-border-radius: 6px;
  box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
  -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
  -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
  padding: ${(props) =>
    !props.maxHeight &&
    props.children &&
    props.children.length > 1 &&
    `4px 0px`};
  ${(props) =>
    props.columnCount &&
    `
      -webkit-column-count: ${props.columnCount};
      -moz-column-count: ${props.columnCount};
            column-count: ${props.columnCount};
    `}
`;

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
      rects.left < 250 &&
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

  render() {
    const { maxHeight, children } = this.props;
    const { directionX, directionY, width } = this.state;
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
        ) : (
          children
        )}
      </StyledDropdown>
    );
  }
}

DropDown.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string,
  clickOutsideAction: PropTypes.func,
  directionX: PropTypes.oneOf(["left", "right"]), //TODO: make more informative
  directionY: PropTypes.oneOf(["bottom", "top"]),
  disableOnClickOutside: PropTypes.func,
  enableOnClickOutside: PropTypes.func,
  id: PropTypes.string,
  manualWidth: PropTypes.string,
  manualX: PropTypes.string,
  manualY: PropTypes.string,
  maxHeight: PropTypes.number,
  open: PropTypes.bool,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  withBackdrop: PropTypes.bool,
  columnCount: PropTypes.number,
};

DropDown.defaultProps = {
  directionX: "left",
  directionY: "bottom",
  withBackdrop: false,
};

const EnhancedComponent = onClickOutside(DropDown);

class DropDownContainer extends React.Component {
  render() {
    const { withBackdrop = false, open } = this.props;
    const isTablet = window.innerWidth < 1024; //TODO: Make some better
    return (
      <>
        <EnhancedComponent disableOnClickOutside={true} {...this.props} />
        {withBackdrop && open && isTablet && (
          <Backdrop visible zIndex={149} onClick={this.toggleDropDown} />
        )}
      </>
    );
  }
}

DropDownContainer.propTypes = {
  open: PropTypes.bool,
  withBackdrop: PropTypes.bool,
};

export default DropDownContainer;
