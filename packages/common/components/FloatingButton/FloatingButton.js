import React, { useEffect, useState } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import {
  StyledFloatingButtonWrapper,
  StyledFloatingButton,
  StyledAlertIcon,
  StyledCircleWrap,
  StyledCircle,
  IconBox,
} from "./StyledFloatingButton";

import ButtonUploadIcon from "../../../../public/images/button.upload.react.svg";
import ButtonFileIcon from "../../../../public/images/button.file.react.svg";
import ButtonTrashIcon from "../../../../public/images/button.trash.react.svg";
import ButtonMoveIcon from "../../../../public/images/button.move.react.svg";
import ButtonDuplicateIcon from "../../../../public/images/button.duplicate.react.svg";
import ButtonAlertIcon from "../../../../public/images/button.alert.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import ButtonPlusIcon from "../../../../public/images/actions.button.plus.react.svg";
import ButtonMinusIcon from "../../../../public/images/actions.button.minus.react.svg";
import CloseIcon from "../../../../public/images/close-icon.react.svg";

const StyledButtonAlertIcon = styled(ButtonAlertIcon)`
  ${commonIconsStyles}
`;

const Delay = 1000;
const FloatingButton = ({ id, className, style, ...rest }) => {
  const {
    icon,
    alert,
    percent,
    onClick,
    color,
    clearUploadedFilesHistory,
  } = rest;

  const [animationCompleted, setAnimationCompleted] = useState(false);

  const onProgressClear = () => {
    clearUploadedFilesHistory && clearUploadedFilesHistory();
  };

  let timerId = null;

  useEffect(() => {
    timerId = setTimeout(
      () => setAnimationCompleted(percent === 100 ? true : false),
      Delay
    );

    return () => {
      clearTimeout(timerId);
    };
  }, [percent, setAnimationCompleted]);

  return (
    <StyledFloatingButtonWrapper>
      <StyledCircleWrap
        color={color}
        id={id}
        className={`${className} not-selectable`}
        style={style}
        icon={icon}
        onClick={onClick}
      >
        <StyledCircle
          displayProgress={
            !(percent === 100 && animationCompleted) && icon != "minus"
          }
          percent={percent}
        >
          <div className="circle__mask circle__full">
            <div className="circle__fill"></div>
          </div>
          <div className="circle__mask">
            <div className="circle__fill"></div>
          </div>

          <StyledFloatingButton className="circle__background" color={color}>
            <IconBox>
              {icon == "upload" ? (
                <ButtonUploadIcon />
              ) : icon == "file" ? (
                <ButtonFileIcon />
              ) : icon == "trash" ? (
                <ButtonTrashIcon />
              ) : icon == "move" ? (
                <ButtonMoveIcon />
              ) : icon == "plus" ? (
                <ButtonPlusIcon />
              ) : icon == "minus" ? (
                <ButtonMinusIcon />
              ) : (
                <ButtonDuplicateIcon />
              )}
            </IconBox>
            <StyledAlertIcon>
              {alert ? <StyledButtonAlertIcon size="medium" /> : <></>}
            </StyledAlertIcon>
          </StyledFloatingButton>
        </StyledCircle>
      </StyledCircleWrap>
      {clearUploadedFilesHistory && percent === 100 && (
        <CloseIcon
          className="layout-progress-bar_close-icon"
          onClick={onProgressClear}
        />
      )}
    </StyledFloatingButtonWrapper>
  );
};

FloatingButton.propTypes = {
  id: PropTypes.string,
  className: PropTypes.string,
  style: PropTypes.object,
  icon: PropTypes.oneOf([
    "upload",
    "file",
    "trash",
    "move",
    "duplicate",
    "plus",
    "minus",
  ]),
  alert: PropTypes.bool,
  percent: PropTypes.number,
  onClick: PropTypes.func,
  color: PropTypes.string,
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
