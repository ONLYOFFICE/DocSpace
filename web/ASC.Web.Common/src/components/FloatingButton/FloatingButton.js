import React from "react";
import PropTypes from "prop-types";
import { StyledFloatingButton, StyledAlertIcon } from "./StyledFloatingButton";
import { Icons } from "asc-web-components";

const FloatingButton = ({ id, className, style, ...rest }) => {
  const { icon, alert } = rest;

  return (
    <StyledFloatingButton
      id={id}
      className={className}
      style={style}
      icon={icon}
    >
      {icon == "upload" ? (
        <Icons.ButtonUploadIcon />
      ) : icon == "file" ? (
        <Icons.ButtonFileIcon />
      ) : icon == "trash" ? (
        <Icons.ButtonTrashIcon />
      ) : icon == "move" ? (
        <Icons.ButtonMoveIcon />
      ) : (
        <Icons.ButtonDuplicateIcon />
      )}
      <StyledAlertIcon>
        {alert ? <Icons.ButtonAlertIcon size="medium" /> : <></>}
      </StyledAlertIcon>
    </StyledFloatingButton>
  );
};

FloatingButton.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  icon: PropTypes.oneOf(["upload", "file", "trash", "move", "duplicate"]),
  alert: PropTypes.bool,
};

FloatingButton.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
  icon: "upload",
  alert: false,
};

export default FloatingButton;
