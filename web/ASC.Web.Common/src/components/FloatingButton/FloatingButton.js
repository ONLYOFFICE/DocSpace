import React from "react";
import PropTypes from "prop-types";
import { StyledFloatingButton, StyledAlertIcon } from "./StyledFloatingButton";
import { Icons } from "asc-web-components";

const FloatingButton = ({ id, className, style, ...rest }) => {
  const { icon, alert, percent } = rest;

  return (
    <StyledFloatingButton
      id={id}
      className={className}
      style={style}
      icon={icon}
      percent={percent}
    >
      <div className="circle-wrap">
        <div className="circle">
          <div className="mask full">
            <div className="fill"></div>
          </div>
          <div className="mask half">
            <div className="fill"></div>
          </div>
          <div className="inside-circle">
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
          </div>
        </div>
      </div>
    </StyledFloatingButton>
  );
};

FloatingButton.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  icon: PropTypes.oneOf(["upload", "file", "trash", "move", "duplicate"]),
  alert: PropTypes.bool,
  percent: PropTypes.number,
};

FloatingButton.defaultProps = {
  id: undefined,
  className: undefined,
  style: undefined,
  icon: "upload",
  alert: false,
  percent: 0,
};

export default FloatingButton;
