import React, { Component } from "react";
import PropTypes from "prop-types";
import DomHelpers from "../utils/domHelpers";
import ObjectUtils from "../utils/objectUtils";
import { classNames } from "../utils/classNames";
import { CSSTransition } from "react-transition-group";
import { ReactSVG } from "react-svg";
import Portal from "../portal";
import StyledContextMenu from "./styled-context-menu";
import ArrowIcon from "./svg/arrow.right.react.svg";

class ContextMenuSub extends Component {
  constructor(props) {
    super(props);
    this.state = {
      activeItem: null,
    };

    this.onEnter = this.onEnter.bind(this);

    this.submenuRef = React.createRef();
  }

  onItemMouseEnter(e, item) {
    if (item.disabled) {
      e.preventDefault();
      return;
    }

    this.setState({
      activeItem: item,
    });
  }

  onItemClick(item, e) {
    if (item.disabled) {
      e.preventDefault();
      return;
    }

    if (!item.url) {
      e.preventDefault();
    }

    if (item.onClick) {
      item.onClick({
        originalEvent: e,
        action: item.action,
      });
    }

    if (!item.items) {
      this.props.onLeafClick(e);
    }
  }

  position() {
    const parentItem = this.submenuRef.current.parentElement;
    const containerOffset = DomHelpers.getOffset(
      this.submenuRef.current.parentElement
    );
    const viewport = DomHelpers.getViewport();
    const sublistWidth = this.submenuRef.current.offsetParent
      ? this.submenuRef.current.offsetWidth
      : DomHelpers.getHiddenElementOuterWidth(this.submenuRef.current);
    const itemOuterWidth = DomHelpers.getOuterWidth(parentItem.children[0]);

    this.submenuRef.current.style.top = "0px";

    if (
      parseInt(containerOffset.left, 10) + itemOuterWidth + sublistWidth >
      viewport.width - DomHelpers.calculateScrollbarWidth()
    ) {
      this.submenuRef.current.style.left = -1 * sublistWidth + "px";
    } else {
      this.submenuRef.current.style.left = itemOuterWidth + "px";
    }
  }

  onEnter() {
    this.position();
  }

  isActive() {
    return this.props.root || !this.props.resetMenu;
  }

  static getDerivedStateFromProps(nextProps, prevState) {
    if (nextProps.resetMenu === true) {
      return {
        activeItem: null,
      };
    }

    return null;
  }

  componentDidUpdate() {
    if (this.isActive()) {
      this.position();
    }
  }

  renderSeparator(index) {
    return (
      <li
        key={"separator_" + index}
        className="p-menu-separator not-selectable"
        role="separator"
      ></li>
    );
  }

  renderSubmenu(item) {
    if (item.items) {
      return (
        <ContextMenuSub
          model={item.items}
          resetMenu={item !== this.state.activeItem}
          onLeafClick={this.props.onLeafClick}
        />
      );
    }

    return null;
  }

  renderMenuitem(item, index) {
    if (item.disabled) return; //TODO: Not render disabled items
    const active = this.state.activeItem === item;
    const className = classNames(
      "p-menuitem",
      { "p-menuitem-active": active },
      item.className
    );
    const linkClassName = classNames("p-menuitem-link", "not-selectable", {
      "p-disabled": item.disabled,
    });
    const iconClassName = classNames("p-menuitem-icon", {
      "p-disabled": item.disabled,
    });
    const submenuIconClassName = "p-submenu-icon";
    const icon = item.icon && (
      <ReactSVG
        wrapper="span"
        className={iconClassName}
        src={item.icon}
      ></ReactSVG>
    );
    const label = item.label && (
      <span className="p-menuitem-text not-selectable">{item.label}</span>
    );
    const submenuIcon = item.items && (
      <ArrowIcon className={submenuIconClassName} />
    );
    const submenu = this.renderSubmenu(item);
    const dataKeys = Object.fromEntries(
      Object.entries(item).filter((el) => el[0].indexOf("data-") === 0)
    );
    const onClick = (e) => {
      this.onItemClick(item, e);
    };
    let content = (
      <a
        href={item.url || "#"}
        className={linkClassName}
        target={item.target}
        {...dataKeys}
        onClick={onClick}
        role="menuitem"
      >
        {icon}
        {label}
        {submenuIcon}
      </a>
    );

    if (item.template) {
      const defaultContentOptions = {
        onClick,
        className: linkClassName,
        labelClassName: "p-menuitem-text",
        iconClassName,
        submenuIconClassName,
        element: content,
        props: this.props,
        active,
      };

      content = ObjectUtils.getJSXElement(
        item.template,
        item,
        defaultContentOptions
      );
    }

    return (
      <li
        key={item.label + "_" + index}
        role="none"
        className={className}
        style={item.style}
        onMouseEnter={(event) => this.onItemMouseEnter(event, item)}
      >
        {content}
        {submenu}
      </li>
    );
  }

  renderItem(item, index) {
    if (!item) return null;
    if (item.isSeparator) return this.renderSeparator(index);
    else return this.renderMenuitem(item, index);
  }

  renderMenu() {
    if (this.props.model) {
      return this.props.model.map((item, index) => {
        return this.renderItem(item, index);
      });
    }

    return null;
  }

  render() {
    const className = classNames({ "p-submenu-list": !this.props.root });
    const submenu = this.renderMenu();
    const isActive = this.isActive();

    return (
      <CSSTransition
        nodeRef={this.submenuRef}
        classNames="p-contextmenusub"
        in={isActive}
        timeout={{ enter: 0, exit: 0 }}
        unmountOnExit={true}
        onEnter={this.onEnter}
      >
        <ul ref={this.submenuRef} className={`${className} not-selectable`}>
          {submenu}
        </ul>
      </CSSTransition>
    );
  }
}

ContextMenuSub.propTypes = {
  model: PropTypes.any,
  root: PropTypes.bool,
  className: PropTypes.string,
  resetMenu: PropTypes.bool,
  onLeafClick: PropTypes.func,
};

ContextMenuSub.defaultProps = {
  model: null,
  root: false,
  className: null,
  resetMenu: false,
  onLeafClick: null,
};

class ContextMenu extends Component {
  constructor(props) {
    super(props);

    this.state = {
      visible: false,
      reshow: false,
      resetMenu: false,
    };

    this.onMenuClick = this.onMenuClick.bind(this);
    this.onLeafClick = this.onLeafClick.bind(this);
    this.onMenuMouseEnter = this.onMenuMouseEnter.bind(this);
    this.onEnter = this.onEnter.bind(this);
    this.onEntered = this.onEntered.bind(this);
    this.onExit = this.onExit.bind(this);
    this.onExited = this.onExited.bind(this);

    this.menuRef = React.createRef();
  }

  onMenuClick() {
    this.setState({
      resetMenu: false,
    });
  }

  onMenuMouseEnter() {
    this.setState({
      resetMenu: false,
    });
  }

  show(e) {
    if (!(e instanceof Event)) {
      e.persist();
    }

    e.stopPropagation();
    e.preventDefault();

    this.currentEvent = e;

    if (this.state.visible) {
      this.setState({ reshow: true });
    } else {
      this.setState({ visible: true }, () => {
        if (this.props.onShow) {
          this.props.onShow(this.currentEvent);
        }
      });
    }
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.state.visible && prevState.reshow !== this.state.reshow) {
      let event = this.currentEvent;
      this.setState(
        {
          visible: false,
          reshow: false,
          rePosition: false,
          resetMenu: true,
        },
        () => this.show(event)
      );
    }
  }

  hide(e) {
    if (!(e instanceof Event)) {
      e.persist();
    }

    this.currentEvent = e;
    this.setState({ visible: false, reshow: false }, () => {
      if (this.props.onHide) {
        this.props.onHide(this.currentEvent);
      }
    });
  }

  onEnter() {
    if (this.props.autoZIndex) {
      this.menuRef.current.style.zIndex = String(
        this.props.baseZIndex + DomHelpers.generateZIndex()
      );
    }

    this.position(this.currentEvent);
  }

  onEntered() {
    this.bindDocumentListeners();
  }

  onExit() {
    this.currentEvent = null;
    this.unbindDocumentListeners();
  }

  onExited() {
    DomHelpers.revertZIndex();
  }

  position(event) {
    if (event) {
      let left = event.pageX + 1;
      let top = event.pageY + 1;
      let width = this.menuRef.current.offsetParent
        ? this.menuRef.current.offsetWidth
        : DomHelpers.getHiddenElementOuterWidth(this.menuRef.current);
      let height = this.menuRef.current.offsetParent
        ? this.menuRef.current.offsetHeight
        : DomHelpers.getHiddenElementOuterHeight(this.menuRef.current);
      let viewport = DomHelpers.getViewport();

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

      this.menuRef.current.style.left = left + "px";
      this.menuRef.current.style.top = top + "px";
    }
  }

  onLeafClick(e) {
    this.setState({
      resetMenu: true,
    });

    this.hide(e);

    e.stopPropagation();
  }

  isOutsideClicked(e) {
    return (
      this.menuRef &&
      this.menuRef.current &&
      !(
        this.menuRef.current.isSameNode(e.target) ||
        this.menuRef.current.contains(e.target)
      )
    );
  }

  bindDocumentListeners() {
    this.bindDocumentResizeListener();
    this.bindDocumentClickListener();
  }

  unbindDocumentListeners() {
    this.unbindDocumentResizeListener();
    this.unbindDocumentClickListener();
  }

  bindDocumentClickListener() {
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
  }

  bindDocumentContextMenuListener() {
    if (!this.documentContextMenuListener) {
      this.documentContextMenuListener = (e) => {
        this.show(e);
      };

      document.addEventListener(
        "contextmenu",
        this.documentContextMenuListener
      );
    }
  }

  bindDocumentResizeListener() {
    if (!this.documentResizeListener) {
      this.documentResizeListener = (e) => {
        if (this.state.visible) {
          this.hide(e);
        }
      };

      window.addEventListener("resize", this.documentResizeListener);
    }
  }

  unbindDocumentClickListener() {
    if (this.documentClickListener) {
      document.removeEventListener("click", this.documentClickListener);
      document.removeEventListener("mousedown", this.documentClickListener);
      this.documentClickListener = null;
    }
  }

  unbindDocumentContextMenuListener() {
    if (this.documentContextMenuListener) {
      document.removeEventListener(
        "contextmenu",
        this.documentContextMenuListener
      );
      this.documentContextMenuListener = null;
    }
  }

  unbindDocumentResizeListener() {
    if (this.documentResizeListener) {
      window.removeEventListener("resize", this.documentResizeListener);
      this.documentResizeListener = null;
    }
  }

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

  renderContextMenu() {
    const className = classNames(
      "p-contextmenu p-component",
      this.props.className
    );

    return (
      <StyledContextMenu>
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
            <ContextMenuSub
              model={this.props.model}
              root
              resetMenu={this.state.resetMenu}
              onLeafClick={this.onLeafClick}
            />
          </div>
        </CSSTransition>
      </StyledContextMenu>
    );
  }

  render() {
    const element = this.renderContextMenu();

    return <Portal element={element} appendTo={this.props.appendTo} />;
  }
}

ContextMenu.propTypes = {
  /** Unique identifier of the element */
  id: PropTypes.string,
  /** An array of menuitems */
  model: PropTypes.array,
  /** Inline style of the component */
  style: PropTypes.object,
  /** Style class of the component */
  className: PropTypes.string,
  /** Attaches the menu to document instead of a particular item */
  global: PropTypes.bool,
  /** Base zIndex value to use in layering */
  autoZIndex: PropTypes.bool,
  /** Whether to automatically manage layering */
  baseZIndex: PropTypes.number,
  /** DOM element instance where the menu should be mounted */
  appendTo: PropTypes.any,
  /** Callback to invoke when a popup menu is shown */
  onShow: PropTypes.func,
  /** Callback to invoke when a popup menu is hidden */
  onHide: PropTypes.func,
};

ContextMenu.defaultProps = {
  id: null,
  model: null,
  style: null,
  className: null,
  global: false,
  autoZIndex: true,
  baseZIndex: 0,
  appendTo: null,
  onShow: null,
  onHide: null,
};

export default ContextMenu;
