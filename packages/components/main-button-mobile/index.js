import React, { useState, useRef, useEffect } from "react";
import PropTypes from "prop-types";
import {
  StyledFloatingButton,
  StyledDropDown,
  StyledDropDownItem,
  StyledContainerAction,
  StyledProgressBarContainer,
  StyledMobileProgressBar,
  StyledProgressContainer,
  StyledBar,
  StyledButtonWrapper,
  StyledButtonOptions,
  StyledAlertIcon,
  StyledRenderItem,
} from "./styled-main-button";
import IconButton from "../icon-button";
import Button from "../button";
import Text from "../text";
import Scrollbar from "@docspace/components/scrollbar";
import { isIOS, isMobile } from "react-device-detect";
import Backdrop from "../backdrop";

import styled from "styled-components";
import ButtonAlertReactSvg from "PUBLIC_DIR/images/button.alert.react.svg";
import commonIconsStyles from "../utils/common-icons-style";

import { isMobileOnly } from "react-device-detect";

import { ColorTheme, ThemeType } from "@docspace/components/ColorTheme";

const StyledButtonAlertIcon = styled(ButtonAlertReactSvg)`
  cursor: pointer;
  ${commonIconsStyles};
`;

const ProgressBarMobile = ({
  label,
  status,
  percent,
  open,
  onCancel,
  icon,
  onClickAction,
  hideButton,
  error,
}) => {
  const uploadPercent = percent > 100 ? 100 : percent;

  const onClickHeaderAction = () => {
    onClickAction && onClickAction();
    hideButton();
  };

  return (
    <StyledProgressBarContainer isUploading={open}>
      <div className="progress-container">
        <Text
          className="progress-header"
          fontSize={`14`}
          // color="#657077"
          onClick={onClickHeaderAction}
          truncate
        >
          {label}
        </Text>
        <div className="progress_info-container">
          <Text className="progress_count" fontSize={`13`} truncate>
            {status}
          </Text>
          <IconButton
            className="progress_icon"
            onClick={onCancel}
            iconName={icon}
            size={14}
          />
        </div>
      </div>

      <StyledMobileProgressBar>
        <ColorTheme
          themeId={ThemeType.MobileProgressBar}
          uploadPercent={uploadPercent}
          error={error}
        />
      </StyledMobileProgressBar>
    </StyledProgressBarContainer>
  );
};

ProgressBarMobile.propTypes = {
  label: PropTypes.string,
  status: PropTypes.string,
  percent: PropTypes.number,
  open: PropTypes.bool,
  onCancel: PropTypes.func,
  icon: PropTypes.string,
  /** The function called after the progress header is clicked  */
  onClickAction: PropTypes.func,
  /** The function that facilitates hiding the button */
  hideButton: PropTypes.func,
  /** Changes the progress bar color, if set to true */
  error: PropTypes.bool,
};

const MainButtonMobile = (props) => {
  const {
    className,
    style,
    opened,
    onUploadClick,
    actionOptions,
    progressOptions,
    buttonOptions,
    percent,
    title,
    withButton,
    withoutButton,
    manualWidth,
    isOpenButton,
    onClose,
    sectionWidth,
    alert,
    withMenu,
    onClick,
    onAlertClick,
    withAlertClick,
    dropdownStyle,
  } = props;

  const [isOpen, setIsOpen] = useState(opened);
  const [isUploading, setIsUploading] = useState(false);
  const [height, setHeight] = useState(window.innerHeight - 48 + "px");
  const [isOpenSubMenu, setIsOpenSubMenu] = useState(false);

  const divRef = useRef();
  const ref = useRef();
  const dropDownRef = useRef();

  useEffect(() => {
    if (opened !== isOpen) {
      setIsOpen(opened);
    }
  }, [opened]);

  let currentPosition, prevPosition, buttonBackground, scrollElem;

  useEffect(() => {
    if (!isIOS) return;

    scrollElem = document.getElementsByClassName("section-scroll")[0];

    if (scrollElem?.scrollTop === 0) {
      scrollElem.classList.add("dialog-background-scroll");
    }

    scrollElem?.addEventListener("scroll", scrollChangingBackground);

    return () => {
      scrollElem?.removeEventListener("scroll", scrollChangingBackground);
    };
  }, []);

  const scrollChangingBackground = () => {
    currentPosition = scrollElem.scrollTop;
    const scrollHeight = scrollElem.scrollHeight;

    if (currentPosition < prevPosition) {
      setDialogBackground(scrollHeight);
    } else {
      if (currentPosition > 0 && currentPosition > prevPosition) {
        setButtonBackground();
      }
    }
    prevPosition = currentPosition;
  };

  const onAlertClickAction = () => {
    withAlertClick && onAlertClick && onAlertClick();
  };

  const setDialogBackground = (scrollHeight) => {
    if (!buttonBackground) {
      document
        .getElementsByClassName("section-scroll")[0]
        .classList.add("dialog-background-scroll");
    }
    if (currentPosition < scrollHeight / 3) {
      buttonBackground = false;
    }
  };
  const setButtonBackground = () => {
    buttonBackground = true;
    scrollElem.classList.remove("dialog-background-scroll");
  };
  const recalculateHeight = () => {
    let height =
      divRef?.current?.getBoundingClientRect()?.height || window.innerHeight;

    height >= window.innerHeight
      ? setHeight(window.innerHeight - 48 + "px")
      : setHeight(height + "px");
  };

  useEffect(() => {
    recalculateHeight();
  }, [isOpen, isOpenButton, window.innerHeight, isUploading, isOpenSubMenu]);

  useEffect(() => {
    window.addEventListener("resize", recalculateHeight);
    return () => {
      window.removeEventListener("resize", recalculateHeight);
    };
  }, [recalculateHeight]);

  const toggle = (isOpen) => {
    if (isOpenButton && onClose) {
      onClose();
    }

    return setIsOpen(isOpen);
  };

  const onMainButtonClick = (e) => {
    if (!withMenu) {
      onClick && onClick(e);
      return;
    }

    toggle(!isOpen);
  };

  const outsideClick = (e) => {
    if (isOpen && ref.current.contains(e.target)) return;
    toggle(false);
  };

  React.useEffect(() => {
    if (progressOptions) {
      const openProgressOptions = progressOptions.filter(
        (option) => option.open
      );

      setIsUploading(openProgressOptions.length > 0);
    }
  }, [progressOptions]);

  const noHover = isMobileOnly ? true : false;

  const renderItems = () => {
    return (
      <StyledRenderItem ref={divRef}>
        <StyledContainerAction>
          {actionOptions.map((option) => {
            const optionOnClickAction = () => {
              toggle(false);
              option.onClick && option.onClick({ action: option.action });
            };

            const onClickSub = () => {
              setIsOpenSubMenu(!isOpenSubMenu);
            };

            if (option.items)
              return (
                <div key="mobile-submenu">
                  <StyledDropDownItem
                    key={option.key}
                    label={option.label}
                    className={`${option.className} ${
                      option.isSeparator && "is-separator"
                    }`}
                    onClick={onClickSub}
                    icon={option.icon ? option.icon : ""}
                    action={option.action}
                    isActive={isOpenSubMenu}
                    isSubMenu={true}
                    noHover={noHover}
                  />
                  {isOpenSubMenu &&
                    option.items.map((item) => {
                      const subMenuOnClickAction = () => {
                        toggle(false);
                        setIsOpenSubMenu(false);
                        item.onClick && item.onClick({ action: item.action });
                      };

                      return (
                        <StyledDropDownItem
                          key={item.key}
                          label={item.label}
                          className={`${item.className} sublevel`}
                          onClick={subMenuOnClickAction}
                          icon={item.icon ? item.icon : ""}
                          action={item.action}
                          withoutIcon={item.withoutIcon}
                          noHover={noHover}
                        />
                      );
                    })}
                </div>
              );

            return (
              <StyledDropDownItem
                key={option.key}
                label={option.label}
                className={`${option.className} ${
                  option.isSeparator && "is-separator"
                }`}
                onClick={optionOnClickAction}
                icon={option.icon ? option.icon : ""}
                action={option.action}
                noHover={noHover}
              />
            );
          })}
        </StyledContainerAction>
        <StyledProgressContainer
          isUploading={isUploading}
          isOpenButton={isOpenButton}
        >
          {progressOptions &&
            progressOptions.map((option) => (
              <ProgressBarMobile
                key={option.key}
                label={option.label}
                icon={option.icon}
                className={option.className}
                percent={option.percent}
                status={option.status}
                open={option.open}
                onCancel={option.onCancel}
                onClickAction={option.onClick}
                hideButton={() => toggle(false)}
                error={option.error}
              />
            ))}
        </StyledProgressContainer>

        <StyledButtonOptions withoutButton={withoutButton}>
          {buttonOptions
            ? buttonOptions.map((option) =>
                option.isSeparator ? (
                  <div key={option.key} className="separator-wrapper">
                    <div className="is-separator" />
                  </div>
                ) : (
                  <StyledDropDownItem
                    className={`drop-down-item-button ${
                      option.isSeparator ? "is-separator" : ""
                    }`}
                    key={option.key}
                    label={option.label}
                    onClick={option.onClick}
                    icon={option.icon ? option.icon : ""}
                    action={option.action}
                  />
                )
              )
            : ""}
        </StyledButtonOptions>
      </StyledRenderItem>
    );
  };

  const children = renderItems();

  return (
    <>
      <Backdrop zIndex={210} visible={isOpen} onClick={outsideClick} />
      <div
        ref={ref}
        className={className}
        style={{ zIndex: `${isOpen ? "211" : "201"}`, ...style }}
      >
        <StyledFloatingButton
          icon={isOpen ? "minus" : "plus"}
          isOpen={isOpen}
          onClick={onMainButtonClick}
          percent={percent}
        />

        <StyledDropDown
          style={dropdownStyle}
          open={isOpen}
          withBackdrop={false}
          manualWidth={manualWidth || "400px"}
          directionY="top"
          directionX="right"
          isMobile={isMobile}
          fixedDirection={true}
          heightProp={height}
          sectionWidth={sectionWidth}
          isDefaultMode={false}
        >
          {isMobile ? (
            <Scrollbar
              style={{ position: "absolute" }}
              scrollclass="section-scroll"
              stype="mediumBlack"
              ref={dropDownRef}
            >
              {children}
            </Scrollbar>
          ) : (
            children
          )}
        </StyledDropDown>
        <StyledAlertIcon>
          {alert && !isOpen ? (
            <StyledButtonAlertIcon onClick={onAlertClickAction} size="medium" />
          ) : (
            <></>
          )}
        </StyledAlertIcon>
      </div>
    </>
  );
};

MainButtonMobile.propTypes = {
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Drop down items options */
  actionOptions: PropTypes.array.isRequired,
  /** Displays progress bar components */
  progressOptions: PropTypes.array,
  /** Menu that opens by clicking on the button */
  buttonOptions: PropTypes.array,
  /** The function called after the button is clicked */
  onUploadClick: PropTypes.func,
  /** Displays button inside the drop down */
  withButton: PropTypes.bool,
  /** Opens a menu on clicking the button. Used with buttonOptions */
  isOpenButton: PropTypes.bool,
  /** The button name in the drop down */
  title: PropTypes.string,
  /** Loading indicator */
  percent: PropTypes.number,
  /** Width section */
  sectionWidth: PropTypes.number,
  /** Specifies the exact width of the drop down component */
  manualWidth: PropTypes.string,
  /** Accepts class */
  className: PropTypes.string,
  /** Sets the dropdown to open */
  opened: PropTypes.bool,
  /** Closes the drop down */
  onClose: PropTypes.func,
  /** If you need open upload panel when clicking on alert button  */
  onAlertClick: PropTypes.func,
  /** Enables alert click  */
  withAlertClick: PropTypes.bool,
  /** Enables the submenu */
  withMenu: PropTypes.bool,
};

MainButtonMobile.defaultProps = {
  withMenu: true,
};

export default MainButtonMobile;
