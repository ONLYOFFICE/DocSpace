import React, { useState, useRef } from "react";
import PropTypes from "prop-types";
import VerticalDotsReactSvgUrl from "PUBLIC_DIR/images/vertical-dots.react.svg?url";
import IconButton from "@docspace/components/icon-button";
import ContextMenu from "@docspace/components/context-menu";

const ContextButton = (props) => {
  const [isOpen, setIsOpen] = useState(false);
  const ref = useRef(null);
  const menuRef = useRef(null);

  const { className, getData, withMenu, ...rest } = props;

  const toggle = (e, isOpen) => {
    isOpen ? menuRef.current.show(e) : menuRef.current.hide(e);
    setIsOpen(isOpen);
  };

  const onClick = (e) => {
    if (withMenu) toggle(e, !isOpen);
  };

  const onHide = () => {
    setIsOpen(false);
  };

  const model = getData();

  return (
    <div ref={ref} className={className} {...rest}>
      <IconButton
        onClick={onClick}
        iconName={VerticalDotsReactSvgUrl}
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
        leftOffset={150}
      />
    </div>
  );
};

ContextButton.propTypes = {
  className: PropTypes.string,
  getData: PropTypes.func.isRequired,
  id: PropTypes.string,
};

ContextButton.defaultProps = {
  withMenu: true,
};

export default ContextButton;
