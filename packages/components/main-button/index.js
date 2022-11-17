import React, { useRef, useState } from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import Text from "../text";
import { GroupMainButton } from "./styled-main-button";
import ContextMenu from "../context-menu";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

const MainButton = (props) => {
  const { text, model, isDropdown, isDisabled, clickAction } = props;
  const { id, ...rest } = props;

  const ref = useRef();
  const menuRef = useRef(null);

  const [isOpen, setIsOpen] = useState(props.opened);

  const stopAction = (e) => e.preventDefault();

  const toggle = (e, isOpen) => {
    isOpen ? menuRef.current.show(e) : menuRef.current.hide(e);

    setIsOpen(isOpen);
  };

  const onHide = () => {
    setIsOpen(false);
  };

  const onMainButtonClick = (e) => {
    if (!isDisabled) {
      if (!isDropdown) {
        clickAction && clickAction(e);
      } else {
        toggle(e, !isOpen);
      }
    } else {
      stopAction(e);
    }
  };

  return (
    <GroupMainButton {...rest} ref={ref}>
      <ColorTheme
        {...rest}
        id={id}
        onClick={onMainButtonClick}
        themeId={ThemeType.MainButton}
      >
        <Text className="main-button_text">{text}</Text>
        {isDropdown && (
          <>
            <ReactSVG
              className="main-button_img"
              src={"/static/images/triangle-main-button.svg"}
            />

            <ContextMenu
              model={model}
              containerRef={ref}
              ref={menuRef}
              onHide={onHide}
              scaled={true}
            />
          </>
        )}
      </ColorTheme>
    </GroupMainButton>
  );
};

MainButton.propTypes = {
  /** Button text */
  text: PropTypes.string,
  /** Tells when the button should present a disabled state */
  isDisabled: PropTypes.bool,
  /** Select a state between two separate buttons or one with a drop-down list */
  isDropdown: PropTypes.bool,
  /** What the main button will trigger when clicked  */
  clickAction: PropTypes.func,
  /** What the secondary button will trigger when clicked  */
  clickActionSecondary: PropTypes.func,
  /** Open DropDown */
  opened: PropTypes.bool, //TODO: Make us whole
  /** Accepts class */
  className: PropTypes.string,
  /** Accepts id */
  id: PropTypes.string,
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Menu data model */
  model: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

MainButton.defaultProps = {
  text: "Button",
  isDisabled: false,
  isDropdown: true,
};

export default MainButton;
