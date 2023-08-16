import React, { useState, useRef } from "react";
import PropTypes from "prop-types";

import PlusReactSvgUrl from "PUBLIC_DIR/images/plus.svg?url";

import IconButton from "@docspace/components/icon-button";
import ContextMenu from "@docspace/components/context-menu";

const PlusButton = (props) => {
  const [isOpen, setIsOpen] = useState(false);
  const ref = useRef(null);
  const menuRef = useRef(null);

  const { className, getData, withMenu, onPlusClick, isFrame } = props;

  const toggle = (e, isOpen) => {
    isOpen ? menuRef.current.show(e) : menuRef.current.hide(e);
    setIsOpen(isOpen);
  };

  const onClick = (e) => {
    if (withMenu) toggle(e, !isOpen);
    else onPlusClick && onPlusClick();
  };

  const onHide = () => {
    setIsOpen(false);
  };

  const model = getData();

  return (
    <div ref={ref} className={className}>
      <IconButton
        onClick={onClick}
        iconName={PlusReactSvgUrl}
        id={props.id}
        size={15}
        isFill
      />
      <ContextMenu
        model={model}
        containerRef={ref}
        ref={menuRef}
        onHide={onHide}
        scaled={false}
        directionX="right"
        leftOffset={isFrame ? 190 : 150}
      />
    </div>
  );
};

PlusButton.propTypes = {
  className: PropTypes.string,
  getData: PropTypes.func.isRequired,
  onPlusClick: PropTypes.func,
  id: PropTypes.string,
};

PlusButton.defaultProps = {
  withMenu: true,
};

export default PlusButton;
