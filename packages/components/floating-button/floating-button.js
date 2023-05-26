import React, { useEffect, useState, useMemo } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";

import {
  StyledFloatingButtonWrapper,
  StyledFloatingButton,
  StyledAlertIcon,
  StyledCircle,
  IconBox,
} from "./styled-floating-button";

import ButtonUploadIcon from "PUBLIC_DIR/images/button.upload.react.svg";
import ButtonFileIcon from "PUBLIC_DIR/images/button.file.react.svg";
import ButtonTrashIcon from "PUBLIC_DIR/images/button.trash.react.svg";
import ButtonMoveIcon from "PUBLIC_DIR/images/button.move.react.svg";
import ButtonDuplicateIcon from "PUBLIC_DIR/images/button.duplicate.react.svg";
import ButtonAlertIcon from "PUBLIC_DIR/images/button.alert.react.svg";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import ButtonPlusIcon from "PUBLIC_DIR/images/actions.button.plus.react.svg";
import ButtonMinusIcon from "PUBLIC_DIR/images/actions.button.minus.react.svg";
import CloseIcon from "PUBLIC_DIR/images/close-icon.react.svg";

import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

const StyledButtonAlertIcon = styled(ButtonAlertIcon)`
  ${commonIconsStyles}
`;

const Delay = 1000;
const FloatingButton = (props) => {
  const {
    id,
    className,
    style,
    icon,
    alert,
    percent,
    onClick,
    color,
    clearUploadedFilesHistory,
    ...rest
  } = props;

  const [animationCompleted, setAnimationCompleted] = useState(false);

  const onProgressClear = () => {
    clearUploadedFilesHistory && clearUploadedFilesHistory();
  };

  const displayProgress = useMemo(() => {
    return !(percent === 100 && animationCompleted) && icon != "minus";
  }, [percent, animationCompleted, icon]);

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
    <StyledFloatingButtonWrapper className="layout-progress-bar_wrapper">
      <ColorTheme
        {...props}
        themeId={ThemeType.FloatingButton}
        color={color}
        id={id}
        className={`${className} not-selectable`}
        style={style}
        icon={icon}
        onClick={onClick}
        displayProgress={displayProgress}
      >
        <StyledCircle displayProgress={displayProgress} percent={percent}>
          <div className="circle__mask circle__full">
            <div className="circle__fill"></div>
          </div>
          <div className="circle__mask">
            <div className="circle__fill"></div>
          </div>

          <StyledFloatingButton className="circle__background" color={color}>
            <IconBox className="icon-box">
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
      </ColorTheme>
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
