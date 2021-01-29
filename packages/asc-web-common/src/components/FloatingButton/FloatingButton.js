import React from "react";
import PropTypes from "prop-types";
import {
  StyledFloatingButton,
  StyledAlertIcon,
  StyledCircleWrap,
  StyledCircle,
} from "./StyledFloatingButton";

import {
  ButtonUploadIcon,
  ButtonFileIcon,
  ButtonTrashIcon,
  ButtonMoveIcon,
  ButtonDuplicateIcon,
  ButtonAlertIcon,
} from "@appserver/components/src/components/icons/svg";

const FloatingButton = ({ id, className, style, ...rest }) => {
  const { icon, alert, percent, onClick } = rest;

  return (
    <StyledCircleWrap
      id={id}
      className={className}
      style={style}
      icon={icon}
      onClick={onClick}
    >
      <StyledCircle percent={percent}>
        <div className="circle__mask circle__full">
          <div className="circle__fill"></div>
        </div>
        <div className="circle__mask">
          <div className="circle__fill"></div>
        </div>

        <StyledFloatingButton>
          {icon == "upload" ? (
            <ButtonUploadIcon />
          ) : icon == "file" ? (
            <ButtonFileIcon />
          ) : icon == "trash" ? (
            <ButtonTrashIcon />
          ) : icon == "move" ? (
            <ButtonMoveIcon />
          ) : (
            <ButtonDuplicateIcon />
          )}

          <StyledAlertIcon>
            {alert ? <ButtonAlertIcon size="medium" /> : <></>}
          </StyledAlertIcon>
        </StyledFloatingButton>
      </StyledCircle>
    </StyledCircleWrap>
  );
};

FloatingButton.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  icon: PropTypes.oneOf(["upload", "file", "trash", "move", "duplicate"]),
  alert: PropTypes.bool,
  percent: PropTypes.number,
  onClick: PropTypes.func,
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
