import React, { useRef, useState, useEffect } from "react";
import PropTypes from "prop-types";
import DomHelpers from "../../utils/domHelpers";
import ObjectUtils from "../../utils/objectUtils";
import { classNames } from "../../utils/classNames";
import { CSSTransition } from "react-transition-group";
import { ReactSVG } from "react-svg";
import ArrowIcon from "../svg/arrow.right.react.svg";
import Scrollbar from "../../scrollbar";

//import CustomScrollbarsVirtualList from "../../scrollbar/custom-scrollbars-virtual-list";
//import { VariableSizeList } from "react-window";

const SubMenu = (props) => {
  const { onLeafClick, root, resetMenu, model, changeView } = props;

  const [activeItem, setActiveItem] = useState(null);
  const subMenuRef = useRef();

  const onItemMouseEnter = (e, item) => {
    if (item.disabled) {
      e.preventDefault();
      return;
    }

    setActiveItem(item);
  };

  const onItemClick = (e, item) => {
    const { disabled, url, onClick, items, action } = item;
    if (disabled) {
      e.preventDefault();
      return;
    }

    if (!url) {
      e.preventDefault();
    }

    if (onClick) {
      onClick({
        originalEvent: e,
        action: action,
        item,
      });
    }

    if (!items) {
      onLeafClick(e);
    }
  };

  const position = () => {
    const parentItem = subMenuRef.current.parentElement;
    const containerOffset = DomHelpers.getOffset(
      subMenuRef.current.parentElement
    );
    const viewport = DomHelpers.getViewport();
    const subListWidth = subMenuRef.current.offsetParent
      ? subMenuRef.current.offsetWidth
      : DomHelpers.getHiddenElementOuterWidth(subMenuRef.current);
    const itemOuterWidth = DomHelpers.getOuterWidth(parentItem.children[0]);

    subMenuRef.current.style.top = "0px";

    if (
      parseInt(containerOffset.left, 10) + itemOuterWidth + subListWidth >
      viewport.width - DomHelpers.calculateScrollbarWidth()
    ) {
      subMenuRef.current.style.left = -1 * subListWidth + "px";
    } else {
      subMenuRef.current.style.left = itemOuterWidth + "px";
    }
  };

  const onEnter = () => {
    position();
  };

  const isActive = () => {
    return root || !resetMenu;
  };

  useEffect(() => {
    if (isActive()) {
      position();
    }
  });

  const renderSeparator = (index, style) => (
    <li
      key={"separator_" + index}
      className="p-menu-separator not-selectable"
      role="separator"
      style={style}
    ></li>
  );

  const renderSubMenu = (item) => {
    if (item.items) {
      return (
        <SubMenu
          model={item.items}
          resetMenu={item !== activeItem}
          onLeafClick={onLeafClick}
        />
      );
    }

    return null;
  };

  const renderMenuitem = (item, index, style) => {
    if (item.disabled) return;
    //TODO: Not render disabled items
    const active = activeItem === item;
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
    const subMenuIconClassName = "p-submenu-icon";
    const icon =
      item.icon &&
      (item.icon.includes("https://") || item.icon.includes("http://") ? (
        <img src={item.icon} className={iconClassName} />
      ) : (
        <ReactSVG wrapper="span" className={iconClassName} src={item.icon} />
      ));

    const label = item.label && (
      <span className="p-menuitem-text not-selectable">{item.label}</span>
    );
    const subMenuIcon = item.items && (
      <ArrowIcon className={subMenuIconClassName} />
    );
    const subMenu = renderSubMenu(item);
    const dataKeys = Object.fromEntries(
      Object.entries(item).filter((el) => el[0].indexOf("data-") === 0)
    );
    const onClick = (e) => {
      onItemClick(e, item);
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
        {subMenuIcon}
      </a>
    );

    if (item.template) {
      const defaultContentOptions = {
        onClick,
        className: linkClassName,
        labelClassName: "p-menuitem-text",
        iconClassName,
        subMenuIconClassName,
        element: content,
        props: props,
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
        id={item.id}
        key={item.key}
        role="none"
        className={className}
        style={{ ...item.style, ...style }}
        onMouseEnter={(e) => onItemMouseEnter(e, item)}
      >
        {content}
        {subMenu}
      </li>
    );
  };

  const renderItem = (data, idx) => {
    let item = data;
    let index = idx;
    let style = {};

    if (Array.isArray(data?.data)) {
      item = data.data[data.index] ? data.data[data.index] : null;
      index = data.index;
      style = data.style;
    }

    if (!item) return null;
    if (item.isSeparator)
      return (
        <React.Fragment key={"fragment" + item.key}>
          {renderSeparator(index, style)}
        </React.Fragment>
      );

    return (
      <React.Fragment key={"fragment" + item.key}>
        {renderMenuitem(item, index, style)}
      </React.Fragment>
    );
  };

  const renderMenu = () => {
    if (model) {
      if (changeView) {
        const newModel = model.filter((item) => item && !item.disabled);
        const rowHeights = newModel.map((item) => {
          if (!item) return 0;
          if (item.isSeparator) return 13;
          return 36;
        });

        //const getItemSize = (index) => rowHeights[index];

        const height = rowHeights.reduce((a, b) => a + b);

        const viewport = DomHelpers.getViewport();

        const listHeight =
          height + 61 > viewport.height - 64
            ? viewport.height - 125
            : height + 5;

        return (
          <Scrollbar style={{ height: listHeight }} stype="mediumBlack">
            {model.map((item, index) => {
              if (item.disabled) return null;
              return renderItem(item, index);
            })}
          </Scrollbar>
        );

        // return (
        //   <VariableSizeList
        //     height={listHeight}
        //     width={"auto"}
        //     itemCount={newModel.length}
        //     itemSize={getItemSize}
        //     itemData={newModel}
        //     outerElementType={CustomScrollbarsVirtualList}
        //   >
        //     {renderItem}
        //   </VariableSizeList>
        // );
      }

      return model.map((item, index) => {
        if (item?.disabled) return null;
        return renderItem(item, index);
      });
    }

    return null;
  };

  const className = classNames({ "p-submenu-list": !root });
  const submenu = renderMenu();
  const active = isActive();

  return (
    <CSSTransition
      nodeRef={subMenuRef}
      classNames="p-contextmenusub"
      in={active}
      timeout={{ enter: 0, exit: 0 }}
      unmountOnExit={true}
      onEnter={onEnter}
    >
      <ul ref={subMenuRef} className={`${className} not-selectable`}>
        {submenu}
      </ul>
    </CSSTransition>
  );
};

SubMenu.propTypes = {
  model: PropTypes.any,
  root: PropTypes.bool,
  className: PropTypes.string,
  resetMenu: PropTypes.bool,
  onLeafClick: PropTypes.func,
  changeView: PropTypes.bool,
};

SubMenu.defaultProps = {
  model: null,
  root: false,
  className: null,
  resetMenu: false,
  onLeafClick: null,
};

export default SubMenu;
