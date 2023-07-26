import React from "react";
import throttle from "lodash/throttle";
import PropTypes from "prop-types";

import DropDownItem from "../drop-down-item";
import DropDown from "../drop-down";
import IconButton from "../icon-button";
import Backdrop from "../backdrop";
import Aside from "../aside";
import Heading from "../heading";
import Link from "../link";
import { desktop } from "../utils/device";
import { isMobile } from "react-device-detect";
import {
  StyledBodyContent,
  StyledHeaderContent,
  StyledContent,
  StyledOuter,
} from "./styled-context-menu-button";

import VerticalDotsReactSvgUrl from "PUBLIC_DIR/images/vertical-dots.react.svg?url";

class ContextMenuButton extends React.Component {
  constructor(props) {
    super(props);

    this.ref = React.createRef();
    const displayType =
      props.displayType === "auto" ? this.getTypeByWidth() : props.displayType;

    this.state = {
      isOpen: props.opened,
      data: props.data,
      displayType,
    };
    this.throttledResize = throttle(this.resize, 300);
  }

  getTypeByWidth = () => {
    if (this.props.displayType !== "auto") return this.props.displayType;
    return window.innerWidth < desktop.match(/\d+/)[0] ? "aside" : "dropdown";
  };

  resize = () => {
    if (this.props.displayType !== "auto") return;
    const type = this.getTypeByWidth();
    if (type === this.state.displayType) return;
    this.setState({ displayType: type });
  };

  popstate = () => {
    window.removeEventListener("popstate", this.popstate, false);
    this.onClose();
    window.history.go(1);
  };

  componentDidMount() {
    window.addEventListener("resize", this.throttledResize);
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.throttledResize);
    window.removeEventListener("popstate", this.popstate, false);
    this.throttledResize.cancel();
  }

  stopAction = (e) => e.preventDefault();
  toggle = (isOpen) => this.setState({ isOpen: isOpen });
  onClose = () => {
    this.setState({ isOpen: !this.state.isOpen });
    this.props.onClose && this.props.onClose();
  };

  componentDidUpdate(prevProps) {
    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }

    if (this.props.opened && this.state.displayType === "aside") {
      window.addEventListener("popstate", this.popstate, false);
    }

    if (this.props.displayType !== prevProps.displayType) {
      this.setState({ displayType: this.getTypeByWidth() });
    }
  }

  onIconButtonClick = (e) => {
    if (this.props.isDisabled || this.state.displayType === "toggle") {
      this.stopAction;
      return;
    }

    this.setState(
      {
        data: this.props.getData(),
        isOpen: !this.state.isOpen,
      },
      () =>
        !this.props.isDisabled &&
        this.state.isOpen &&
        this.props.onClick &&
        this.props.onClick(e)
    ); // eslint-disable-line react/prop-types
  };

  clickOutsideAction = (e) => {
    const path = e.path || (e.composedPath && e.composedPath());
    const dropDownItem = path ? path.find((x) => x === this.ref.current) : null;
    if (dropDownItem) return;

    this.onClose();
  };

  onDropDownItemClick = (item, e) => {
    const open = this.state.displayType === "dropdown";
    item.onClick && item.onClick(e, open, item);
    this.toggle(!this.state.isOpen);
  };

  shouldComponentUpdate(nextProps, nextState) {
    if (
      this.props.opened === nextProps.opened &&
      this.state.isOpen === nextState.isOpen &&
      this.props.displayType === nextProps.displayType &&
      this.props.isDisabled === nextProps.isDisabled
    ) {
      return false;
    }
    return true;
  }

  callNewMenu = (e) => {
    if (this.props.isDisabled || this.state.displayType !== "toggle") {
      this.stopAction;
      return;
    }

    this.setState(
      {
        data: this.props.getData(),
      },
      () => this.props.onClick(e)
    );
  };

  render() {
    //console.log("ContextMenuButton render", this.props);
    const {
      className,
      clickColor,
      color,
      columnCount,
      directionX,
      directionY,
      hoverColor,
      iconClickName,
      iconHoverName,
      iconName,
      iconOpenName,
      id,
      isDisabled,
      onMouseEnter,
      onMouseLeave,
      onMouseOut,
      onMouseOver,
      size,
      style,
      isFill, // eslint-disable-line react/prop-types
      asideHeader, // eslint-disable-line react/prop-types
      title,
      zIndex,
      usePortal,
      dropDownClassName,
      iconClassName,
      displayIconBorder,
    } = this.props;

    const { isOpen, displayType, offsetX, offsetY } = this.state;
    const iconButtonName = isOpen && iconOpenName ? iconOpenName : iconName;
    return (
      <StyledOuter
        ref={this.ref}
        className={className}
        id={id}
        style={style}
        onClick={this.callNewMenu}
        displayIconBorder={displayIconBorder}
      >
        <IconButton
          className={iconClassName}
          color={color}
          hoverColor={hoverColor}
          clickColor={clickColor}
          size={size}
          iconName={iconButtonName}
          iconHoverName={iconHoverName}
          iconClickName={iconClickName}
          isFill={isFill}
          isDisabled={isDisabled}
          onClick={this.onIconButtonClick}
          onMouseEnter={onMouseEnter}
          onMouseLeave={onMouseLeave}
          onMouseOver={onMouseOver}
          onMouseOut={onMouseOut}
          title={title}
        />
        {displayType === "dropdown" ? (
          <DropDown
            className={dropDownClassName}
            directionX={directionX}
            directionY={directionY}
            open={isOpen}
            forwardedRef={this.ref}
            clickOutsideAction={this.clickOutsideAction}
            columnCount={columnCount}
            withBackdrop={!!isMobile}
            zIndex={zIndex}
            isDefaultMode={usePortal}
          >
            {this.state.data?.map(
              (item, index) =>
                item &&
                (item.label || item.icon || item.key) && (
                  <DropDownItem
                    {...item}
                    id={item.id}
                    key={item.key || index}
                    onClick={this.onDropDownItemClick.bind(this, item)}
                  />
                )
            )}
          </DropDown>
        ) : (
          displayType === "aside" && (
            <>
              <Backdrop
                onClick={this.onClose}
                visible={isOpen}
                zIndex={310}
                isAside={true}
              />
              <Aside
                visible={isOpen}
                scale={false}
                zIndex={310}
                onClose={this.onClose}
              >
                <StyledContent>
                  <StyledHeaderContent>
                    <Heading className="header" size="medium" truncate={true}>
                      {asideHeader}
                    </Heading>
                  </StyledHeaderContent>
                  <StyledBodyContent>
                    {this.state.data.map(
                      (item, index) =>
                        item &&
                        (item.label || item.icon || item.key) && (
                          <Link
                            className={`context-menu-button_link${
                              item.isHeader ? "-header" : ""
                            }`}
                            key={item.key || index}
                            fontSize={item.isHeader ? "15px" : "13px"}
                            noHover={item.isHeader}
                            fontWeight={600}
                            onClick={this.onDropDownItemClick.bind(this, item)}
                          >
                            {item.label}
                          </Link>
                        )
                    )}
                  </StyledBodyContent>
                </StyledContent>
              </Aside>
            </>
          )
        )}
      </StyledOuter>
    );
  }
}

ContextMenuButton.propTypes = {
  /** Sets the button to present an opened state */
  opened: PropTypes.bool,
  /** Array of options for display */
  data: PropTypes.array,
  /** Function for converting to inner data */
  getData: PropTypes.func.isRequired,
  /** Specifies the icon title */
  title: PropTypes.string,
  /** Specifies the icon name */
  iconName: PropTypes.string,
  /** Specifies the icon size */
  size: PropTypes.number,
  /** Specifies the icon color */
  color: PropTypes.string,
  /** Sets the button to present a disabled state */
  isDisabled: PropTypes.bool,
  /** Specifies the icon hover color */
  hoverColor: PropTypes.string,
  /** Specifies the icon click color */
  clickColor: PropTypes.string,
  /** Specifies the icon hover name */
  iconHoverName: PropTypes.string,
  /** Specifies the icon click name */
  iconClickName: PropTypes.string,
  /** Specifies the icon open name */
  iconOpenName: PropTypes.string,
  /** Triggers a callback function when the mouse enters the button borders */
  onMouseEnter: PropTypes.func,
  /** Triggers a callback function when the mouse leaves the button borders */
  onMouseLeave: PropTypes.func,
  /** Triggers a callback function when the mouse moves over the button borders */
  onMouseOver: PropTypes.func,
  /** Triggers a callback function when the mouse moves out of the button borders */
  onMouseOut: PropTypes.func,
  /** Direction X */
  directionX: PropTypes.string,
  /** Direction Y */
  directionY: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Sets the number of columns */
  columnCount: PropTypes.number,
  /** Sets the display type */
  displayType: PropTypes.oneOf(["dropdown", "toggle", "aside", "auto"]),
  /** Closing event */
  onClose: PropTypes.func,
  /** Sets the drop down open with the portal */
  usePortal: PropTypes.bool,
  /** Sets the class of the drop down element */
  dropDownClassName: PropTypes.string,
  /** Sets the class of the icon button */
  iconClassName: PropTypes.string,
  /** Enables displaying the icon borders  */
  displayIconBorder: PropTypes.bool,
};

ContextMenuButton.defaultProps = {
  opened: false,
  data: [],
  title: "",
  iconName: VerticalDotsReactSvgUrl,
  size: 16,
  isDisabled: false,
  directionX: "left",
  isFill: false,
  displayType: "dropdown",
  usePortal: true,
  displayIconBorder: false,
};

export default ContextMenuButton;
