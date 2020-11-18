import React from "react";
import PropTypes from "prop-types";
import {
  StyledFloatingButton,
  StyledAlertIcon,
  StyledCircleWrap,
  StyledCircle,
} from "./StyledFloatingButton";
import { Icons } from "asc-web-components";

const FloatingButton = ({ id, className, style, ...rest }) => {
  const { icon, alert, percent } = rest;

  return (
    <StyledCircleWrap id={id} className={className} style={style} icon={icon}>
      <StyledCircle percent={percent}>
        <div className="circle__mask circle__full">
          <div className="circle__fill"></div>
        </div>
        <div className="circle__mask">
          <div className="circle__fill"></div>
        </div>

        <StyledFloatingButton>
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
