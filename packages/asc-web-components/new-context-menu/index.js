import React from "react";
import PropTypes from "prop-types";
import DomHelpers from "../utils/domHelpers";
import { classNames } from "../utils/classNames";
import { CSSTransition } from "react-transition-group";
import { VariableSizeList } from "react-window";

import CustomScrollbarsVirtualList from "../scrollbar/custom-scrollbars-virtual-list";

import { isMobile, isMobileOnly } from "react-device-detect";
import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "../utils/device";

import Portal from "../portal";
import MenuItem from "../menu-item";
import Backdrop from "../backdrop";

import StyledContextMenu from "./styled-new-context-menu";

// eslint-disable-next-line react/display-name, react/prop-types
const Row = React.memo(({ data, index, style }) => {
  // eslint-disable-next-line react/prop-types
  if (!data.data[index]) return null;
  return (
    <MenuItem
      // eslint-disable-next-line react/prop-types
      key={data.data[index].key}
      // eslint-disable-next-line react/prop-types
      name={data.data[index].key}
      // eslint-disable-next-line react/prop-types
      icon={data.data[index].icon}
      // eslint-disable-next-line react/prop-types
      label={data.data[index].label}
      // eslint-disable-next-line react/prop-types
      isSeparator={data.data[index].isSeparator}
      // eslint-disable-next-line react/prop-types
      options={data.data[index].options}
      // eslint-disable-next-line react/prop-types
      onClick={data.data[index].onClick}
      // eslint-disable-next-line react/prop-types
      hideMenu={data.hideMenu}
      style={style}
    />
  );
});

class NewContextMenu extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      visible: false,
      reshow: false,
      resetMenu: false,
      changeView: false,
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
          changeView: false,
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
    this.setState({ visible: false, reshow: false, changeView: false }, () => {
      if (this.props.onHide) {
        this.props.onHide(this.currentEvent);
      }
    });
  }

  hideMenu() {
    this.setState({ visible: false, reshow: false, changeView: false }, () => {
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
    if (this.props.position) {
      let width = this.menuRef.current.offsetParent
        ? this.menuRef.current.offsetWidth
        : DomHelpers.getHiddenElementOuterWidth(this.menuRef.current);
      let viewport = DomHelpers.getViewport();
      if (
        this.props.position.left + width >
        viewport.width - DomHelpers.calculateScrollbarWidth()
      ) {
        this.menuRef.current.style.right =
          // -1 * this.props.position.width + width + "px";
          0 + "px";
      } else {
        this.menuRef.current.style.left = this.props.position.left + "px";
      }

      this.menuRef.current.style.top = this.props.position.top + "px";
      return;
    }
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

  renderContextMenuItems() {
    if (!this.props.model) return null;
    if (this.state.changeView) {
      const model = this.props.model.filter((item) => !!item);

      const rowHeights = model.map((item, index) => {
        if (!item) return 0;
        if (item.isSeparator) return 13;
        return 36;
      });

      const getItemSize = (index) => rowHeights[index];

      const height = rowHeights.reduce((a, b) => a + b);

      const viewport = DomHelpers.getViewport();

      const listHeight =
        height + 61 > viewport.height - 64 ? viewport.height - 125 : height;

      return (
        <VariableSizeList
          height={listHeight}
          width={"auto"}
          itemCount={model.length}
          itemSize={getItemSize}
          itemData={{
            data: model,
            hideMenu: this.hideMenu.bind(this),
          }}
          outerElementType={CustomScrollbarsVirtualList}
        >
          {Row}
        </VariableSizeList>
      );
    } else {
      const items = this.props.model.map((item, index) => {
        if (!item) return;
        return (
          <MenuItem
            key={item.key}
            name={item.key}
            icon={item.icon}
            label={item.label}
            isSeparator={item.isSeparator}
            options={item.options}
            onClick={item.onClick}
            {...item}
            hideMenu={this.hideMenu.bind(this)}
          />
        );
      });

      return items;
    }
  }

  renderContextMenu() {
    const className = classNames("p-contextmenu", this.props.className);

    const items = this.renderContextMenuItems();
    return (
      <>
        {this.props.withBackdrop && (
          <Backdrop visible={this.state.visible} withBackground={true} />
        )}
        <StyledContextMenu changeView={this.state.changeView}>
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
              {this.state.changeView && (
                <MenuItem
                  key={"header"}
                  isHeader={true}
                  icon={this.props.header.icon}
                  label={this.props.header.title}
                />
              )}
              {items}
            </div>
          </CSSTransition>
        </StyledContextMenu>
      </>
    );
  }

  render() {
    const element = this.renderContextMenu();

    return <Portal element={element} appendTo={this.props.appendTo} />;
  }
}

NewContextMenu.propTypes = {
  /** Unique identifier of the element */
  id: PropTypes.string,
  /** An array of objects */
  model: PropTypes.array,
  /** An object of header with icon and label */
  header: PropTypes.object,
  /** Position of context menu */
  position: PropTypes.object,
  /** Inline style of the component */
  style: PropTypes.object,
  /** Style class of the component */
  className: PropTypes.string,
  /** Attaches the menu to document instead of a particular item */
  global: PropTypes.bool,
  /** Tell when context menu was render with backdrop */
  withBackdrop: PropTypes.bool,
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

NewContextMenu.defaultProps = {
  id: null,
  model: null,
  position: null,
  header: null,
  style: null,
  className: null,
  global: false,
  autoZIndex: true,
  baseZIndex: 0,
  appendTo: null,
  onShow: null,
  onHide: null,
  withBackdrop: true,
};

export default NewContextMenu;
