import React, { Component } from "react";
import PropTypes from "prop-types";
import DomHelpers from "../utils/domHelpers";
import { classNames } from "../utils/classNames";
import { CSSTransition } from "react-transition-group";
import Portal from "../portal";
import StyledContextMenu from "./styled-context-menu";
import SubMenu from "./sub-components/sub-menu";
import MobileSubMenu from "./sub-components/mobile-sub-menu";

import { isMobile, isMobileOnly } from "react-device-detect";
import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "../utils/device";

import Backdrop from "../backdrop";
import Text from "../text";
import Avatar from "../avatar";
import IconButton from "../icon-button";
import ArrowLeftReactUrl from "PUBLIC_DIR/images/arrow-left.react.svg?url";

class ContextMenu extends Component {
  constructor(props) {
    super(props);

    this.state = {
      visible: false,
      reshow: false,
      resetMenu: false,
      model: null,
      changeView: false,
      showMobileMenu: false,
      onLoad: null,
    };

    this.menuRef = React.createRef();
  }

  onMenuClick = () => {
    this.setState({
      resetMenu: false,
    });
  };

  onMenuMouseEnter = () => {
    this.setState({
      resetMenu: false,
    });
  };

  show = (e) => {
    if (this.props.getContextModel) {
      const model = this.props.getContextModel();
      this.setState({ model });
    }

    e.stopPropagation();
    e.preventDefault();

    this.currentEvent = e;

    if (this.state.visible) {
      !isMobileOnly && this.setState({ reshow: true });
    } else {
      this.setState({ visible: true }, () => {
        if (this.props.onShow) {
          this.props.onShow(this.currentEvent);
        }
      });
    }
  };

  componentDidUpdate(prevProps, prevState) {
    if (this.state.visible && prevState.reshow !== this.state.reshow) {
      let event = this.currentEvent;
      this.setState(
        {
          visible: false,
          reshow: false,
          resetMenu: true,
          changeView: false,
        },
        () => this.show(event)
      );
    }
  }

  hide = (e) => {
    this.currentEvent = e;

    this.props.onHide && this.props.onHide(e);
    this.setState({
      visible: false,
      reshow: false,
      changeView: false,
      showMobileMenu: false,
    });
  };

  onEnter = () => {
    if (this.props.autoZIndex) {
      this.menuRef.current.style.zIndex = String(
        this.props.baseZIndex + DomHelpers.generateZIndex()
      );
    }

    this.position(this.currentEvent);
  };

  onEntered = () => {
    this.bindDocumentListeners();
  };

  onExit = () => {
    this.currentEvent = null;
    this.unbindDocumentListeners();
  };

  onExited = () => {
    DomHelpers.revertZIndex();
  };

  position = (event) => {
    if (event) {
      const rects = this.props.containerRef?.current.getBoundingClientRect();

      let left = rects ? rects.left - this.props.leftOffset : event.pageX + 1;
      let top = rects ? rects.top : event.pageY + 1;
      let width = this.menuRef.current.offsetParent
        ? this.menuRef.current.offsetWidth
        : DomHelpers.getHiddenElementOuterWidth(this.menuRef.current);
      let height = this.menuRef.current.offsetParent
        ? this.menuRef.current.offsetHeight
        : DomHelpers.getHiddenElementOuterHeight(this.menuRef.current);
      let viewport = DomHelpers.getViewport();

      if ((isMobile || isTabletUtils()) && height > 483) {
        this.setState({ changeView: true });
        return;
      }

      if ((isMobileOnly || isMobileUtils()) && height > 210) {
        this.setState({ changeView: true });
        return;
      }

      //flip
      if (left + width - document.body.scrollLeft > viewport.width) {
        left -= width;
      }

      //flip
      if (top + height - document.body.scrollTop > viewport.height) {
        top -= height;
      }

      //fit
      if (left < document.body.scrollLeft) {
        left = document.body.scrollLeft;
      }

      //fit
      if (top < document.body.scrollTop) {
        top = document.body.scrollTop;
      }

      if (this.props.containerRef) {
        top += rects.height + 4;

        if (this.props.scaled) {
          this.menuRef.current.style.width = rects.width + "px";
        }
        this.menuRef.current.style.minWidth = "210px";
      }

      this.menuRef.current.style.left = left + "px";
      this.menuRef.current.style.top = top + "px";
    }
  };

  onLeafClick = (e) => {
    this.setState({
      resetMenu: true,
    });

    this.hide(e);

    e.stopPropagation();
  };

  isOutsideClicked = (e) => {
    return (
      this.menuRef &&
      this.menuRef.current &&
      !(
        this.menuRef.current.isSameNode(e.target) ||
        this.menuRef.current.contains(e.target)
      )
    );
  };

  bindDocumentListeners = () => {
    this.bindDocumentResizeListener();
    this.bindDocumentClickListener();
  };

  unbindDocumentListeners = () => {
    this.unbindDocumentResizeListener();
    this.unbindDocumentClickListener();
  };

  bindDocumentClickListener = () => {
    if (!this.documentClickListener) {
      this.documentClickListener = (e) => {
        if (this.isOutsideClicked(e)) {
          //TODO: (&& e.button !== 2) restore after global usage
          this.hide(e);

          this.setState({
            resetMenu: true,
          });
        }
      };

      document.addEventListener("click", this.documentClickListener);
      document.addEventListener("mousedown", this.documentClickListener);
    }
  };

  bindDocumentContextMenuListener = () => {
    if (!this.documentContextMenuListener) {
      this.documentContextMenuListener = (e) => {
        this.show(e);
      };

      document.addEventListener(
        "contextmenu",
        this.documentContextMenuListener
      );
    }
  };

  bindDocumentResizeListener = () => {
    if (!this.documentResizeListener) {
      this.documentResizeListener = (e) => {
        if (this.state.visible) {
          this.hide(e);
        }
      };

      window.addEventListener("resize", this.documentResizeListener);
    }
  };

  unbindDocumentClickListener = () => {
    if (this.documentClickListener) {
      document.removeEventListener("click", this.documentClickListener);
      document.removeEventListener("mousedown", this.documentClickListener);
      this.documentClickListener = null;
    }
  };

  unbindDocumentContextMenuListener = () => {
    if (this.documentContextMenuListener) {
      document.removeEventListener(
        "contextmenu",
        this.documentContextMenuListener
      );
      this.documentContextMenuListener = null;
    }
  };

  unbindDocumentResizeListener = () => {
    if (this.documentResizeListener) {
      window.removeEventListener("resize", this.documentResizeListener);
      this.documentResizeListener = null;
    }
  };

  componentDidMount() {
    if (this.props.global) {
      this.bindDocumentContextMenuListener();
    }
  }

  componentWillUnmount() {
    this.unbindDocumentListeners();
    this.unbindDocumentContextMenuListener();

    DomHelpers.revertZIndex();
  }

  onMobileItemClick = (e, onLoad) => {
    e.stopPropagation();
    this.setState({
      showMobileMenu: true,
      onLoad,
    });
  };

  onBackClick = (e) => {
    e.stopPropagation();
    this.setState({
      showMobileMenu: false,
    });
  };

  renderContextMenu = () => {
    const className = classNames(
      "p-contextmenu p-component",
      this.props.className
    );

    const changeView = this.state.changeView;
    const isIconExist = this.props.header?.icon;
    const isAvatarExist = this.props.header?.avatar;

    const withHeader = !!this.props.header?.title;

    return (
      <>
        <StyledContextMenu
          changeView={changeView}
          isRoom={this.props.isRoom}
          fillIcon={this.props.fillIcon}
          isIconExist={isIconExist}
          isAvatarExist={isAvatarExist}
        >
          <CSSTransition
            nodeRef={this.menuRef}
            classNames="p-contextmenu"
            in={this.state.visible}
            timeout={{ enter: 250, exit: 0 }}
            unmountOnExit
            onEnter={this.onEnter}
            onEntered={this.onEntered}
            onExit={this.onExit}
            onExited={this.onExited}
          >
            <div
              ref={this.menuRef}
              id={this.props.id}
              className={className}
              style={this.props.style}
              onClick={this.onMenuClick}
              onMouseEnter={this.onMenuMouseEnter}
            >
              {changeView && withHeader && (
                <div className="contextmenu-header">
                  {isIconExist &&
                    (this.state.showMobileMenu ? (
                      <IconButton
                        className="edit_icon"
                        iconName={ArrowLeftReactUrl}
                        onClick={this.onBackClick}
                        size={16}
                      />
                    ) : (
                      <div className="icon-wrapper">
                        <img
                          src={this.props.header.icon}
                          className="drop-down-item_icon"
                          alt="drop-down_icon"
                        />
                      </div>
                    ))}
                  {isAvatarExist && (
                    <div className="avatar-wrapper">
                      <Avatar
                        source={this.props.header.avatar}
                        size={"min"}
                        className="drop-down-item_avatar"
                      />
                    </div>
                  )}
                  <Text className="text" truncate={true}>
                    {this.props.header.title}
                  </Text>
                </div>
              )}

              {this.state.showMobileMenu ? (
                <MobileSubMenu
                  root
                  resetMenu={this.state.resetMenu}
                  onLeafClick={this.onLeafClick}
                  changeView={true}
                  onLoad={this.state.onLoad}
                />
              ) : (
                <SubMenu
                  model={
                    this.props.getContextModel
                      ? this.state.model
                      : this.props.model
                  }
                  root
                  resetMenu={this.state.resetMenu}
                  onLeafClick={this.onLeafClick}
                  changeView={changeView}
                  onMobileItemClick={this.onMobileItemClick}
                />
              )}
            </div>
          </CSSTransition>
        </StyledContextMenu>
      </>
    );
  };

  render() {
    const element = this.renderContextMenu();

    return (
      <>
        {this.props.withBackdrop && (
          <Backdrop
            visible={this.state.visible}
            withBackground={false}
            withoutBlur={true}
          />
        )}
        <Portal element={element} appendTo={this.props.appendTo} />
      </>
    );
  }
}

ContextMenu.propTypes = {
  /** Unique identifier of the element */
  id: PropTypes.string,
  /** An array of menuitems */
  model: PropTypes.array,
  /** An object of header with icon and label */
  header: PropTypes.object,
  /** Inline style of the component */
  style: PropTypes.object,
  /** Style class of the component */
  className: PropTypes.string,
  /** Attaches the menu to document instead of a particular item */
  global: PropTypes.bool,
  /** Sets the context menu to be rendered with a backdrop */
  withBackdrop: PropTypes.bool,
  /** Sets zIndex layering value automatically */
  autoZIndex: PropTypes.bool,
  /** Sets automatic layering management */
  baseZIndex: PropTypes.number,
  /** DOM element instance where the menu is mounted */
  appendTo: PropTypes.any,
  /** Specifies a callback function that is invoked when a popup menu is shown */
  onShow: PropTypes.func,
  /** Specifies a callback function that is invoked when a popup menu is hidden */
  onHide: PropTypes.func,
  /** Displays a reference to another component */
  containerRef: PropTypes.any,
  /** Scales width by the container component */
  scaled: PropTypes.bool,
  /** Fills the icons with default colors */
  fillIcon: PropTypes.bool,
  /** Function that returns an object containing the elements of the context menu */
  getContextModel: PropTypes.func,
  /** Specifies the offset  */
  leftOffset: PropTypes.number,
};

ContextMenu.defaultProps = {
  id: null,
  style: null,
  className: null,
  global: false,
  autoZIndex: true,
  baseZIndex: 0,
  appendTo: null,
  onShow: null,
  onHide: null,
  scaled: false,
  containerRef: null,
  leftOffset: 0,
  fillIcon: true,
};

export default ContextMenu;
