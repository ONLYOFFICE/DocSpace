import React, { useRef, useState, useEffect } from "react";
import DomHelpers from "../../utils/domHelpers";
import { classNames } from "../../utils/classNames";
import { CSSTransition } from "react-transition-group";
import { ReactSVG } from "react-svg";
import ArrowIcon from "PUBLIC_DIR/images/arrow.right.react.svg";
import Scrollbar from "../../scrollbar";
import Loaders from "@docspace/common/components/Loaders";

const MobileSubMenu = (props) => {
  const { onLeafClick, root, resetMenu, onLoad } = props;

  const [submenu, setSubmenu] = useState(null);

  const subMenuRef = useRef();

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

  const isActive = () => {
    return root || !resetMenu;
  };

  useEffect(() => {
    if (isActive()) {
      position();
    }
  });

  useEffect(() => {
    onLoad && fetchSubMenu();
  }, [onLoad]);

  const fetchSubMenu = async () => {
    const res = await onLoad();
    setSubmenu(res);

    position();
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

  const renderSeparator = (index, style) => (
    <li
      key={"separator_" + index}
      className="p-menu-separator not-selectable"
      role="separator"
      style={style}
    ></li>
  );

  const renderMenuitem = (item, index, style) => {
    if (item.disabled) return;
    //TODO: Not render disabled items

    const className = classNames(
      "p-menuitem",
      { "p-menuitem-active": false },
      item.className
    );
    const linkClassName = classNames("p-menuitem-link", "not-selectable", {
      "p-disabled": item.disabled || item.disableColor,
    });
    const iconClassName = classNames("p-menuitem-icon", {
      "p-disabled": item.disabled || item.disableColor,
    });
    const subMenuIconClassName = "p-submenu-icon";

    const icon =
      item.icon &&
      (!item.icon.includes("images/") ? (
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

    const dataKeys = Object.fromEntries(
      Object.entries(item).filter((el) => el[0].indexOf("data-") === 0)
    );
    const onClick = (e) => {
      onItemClick(e, item);
    };

    return (
      <li
        id={item.id}
        key={item.key}
        role="none"
        className={className}
        style={{ ...item.style, ...style }}
      >
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

  const renderMenu = (model) => {
    if (model) {
      const newModel = model.filter((item) => item && !item.disabled);
      const rowHeights = newModel.map((item) => {
        if (!item) return 0;
        if (item.isSeparator) return 13;
        return 36;
      });

      const height = rowHeights.reduce((a, b) => a + b);
      const viewport = DomHelpers.getViewport();
      const listHeight =
        height + 61 > viewport.height - 64 ? viewport.height - 125 : height + 5;

      return (
        <Scrollbar style={{ height: listHeight }} stype="mediumBlack">
          {model.map((item, index) => {
            if (item.disabled) return null;
            return renderItem(item, index);
          })}
        </Scrollbar>
      );
    }

    return null;
  };

  const className = classNames({ "p-submenu-list": !root });
  const active = isActive();

  return (
    <CSSTransition
      nodeRef={subMenuRef}
      classNames="p-contextmenusub"
      in={active}
      timeout={{ enter: 0, exit: 0 }}
      unmountOnExit={true}
    >
      <ul ref={subMenuRef} className={`${className} not-selectable`}>
        {submenu ? renderMenu(submenu) : <Loaders.ContextMenuLoader />}
      </ul>
    </CSSTransition>
  );
};

export default MobileSubMenu;
