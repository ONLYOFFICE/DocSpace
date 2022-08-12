import React, { useRef, useState } from "react";
import PropTypes from "prop-types";
import { ReactSVG } from "react-svg";
import Text from "../text";
import {
  StyledSecondaryButton,
  StyledMainButton,
  GroupMainButton,
} from "./styled-main-button";
import ContextMenu from "../context-menu";
import { ColorTheme, ThemeType } from "@docspace/common/components/ColorTheme";

const MainButton = (props) => {
  const {
    text,
    model,
    iconName,
    isDropdown,
    isDisabled,
    clickAction,
    clickActionSecondary,
  } = props;

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

  const onSecondaryButtonClick = (e) => {
    if (!isDisabled) {
      clickActionSecondary && clickActionSecondary();
    } else {
      stopAction(e);
    }
  };

  const sideIcon = <ReactSVG src={iconName} width="16px" height="16px" />;

  return (
    <GroupMainButton {...props} ref={ref}>
      <ColorTheme
        {...props}
        onClick={onMainButtonClick}
        type={ThemeType.MainButton}
      >
        <Text className="main-button_text">{text}</Text>
      </ColorTheme>

      {isDropdown ? (
        <ContextMenu
          model={model}
          containerRef={ref}
          ref={menuRef}
          onHide={onHide}
          scaled={true}
        />
      ) : (
        <StyledSecondaryButton {...props} onClick={onSecondaryButtonClick}>
          {iconName && sideIcon}
        </StyledSecondaryButton>
      )}
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
  /** Icon inside button */
  iconName: PropTypes.string,
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
  iconName: "/static/images/people.react.svg",
};

export default MainButton;
