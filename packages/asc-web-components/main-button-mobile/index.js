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
} from "./styled-main-button";
import IconButton from "../icon-button";
import Button from "../button";
import Text from "../text";
import Scrollbar from "@appserver/components/scrollbar";
import { isMobile, isTablet } from "react-device-detect";

const ProgressBarMobile = ({
  label,
  status,
  percent,
  open,
  onCancel,
  icon,
  onClick,
  error,
}) => {
  const uploadPercent = percent > 100 ? 100 : percent;

  return (
    <StyledProgressBarContainer isUploading={open}>
      <Text onClick={onClick} className="progress-header" color="#657077">
        {label}
      </Text>
      <Text className="progress_count" color="#657077">
        {status}
      </Text>
      <IconButton onClick={onCancel} iconName={icon} size={14} />
      <StyledMobileProgressBar>
        <StyledBar uploadPercent={uploadPercent} error={error} />
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
  /** The function that will be called after the progress header click  */
  onClick: PropTypes.func,
  /** If true the progress bar changes color */
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
    manualWidth,
    isOpenButton,
    onClose,
  } = props;

  const [isOpen, setIsOpen] = useState(opened);
  const [height, setHeight] = useState("90vh");

  const divRef = useRef();

  useEffect(() => {
    if (opened !== isOpen) {
      setIsOpen(opened);
    }
  }, [opened]);

  useEffect(() => {
    let height = divRef.current.getBoundingClientRect().height;
    height >= window.innerHeight ? setHeight("90vh") : setHeight(height + "px");
  }, [isOpen, window.innerHeight]);

  const ref = useRef();

  const dropDownRef = useRef();

  const toggle = (isOpen) => {
    if (isOpen && onClose) {
      onClose();
    }
    return setIsOpen(isOpen);
  };

  const onMainButtonClick = (e) => {
    if (isOpen && ref.current.contains(e.target)) return;
    toggle(!isOpen);
  };

  const outsideClick = (e) => {
    if (isOpen && ref.current.contains(e.target)) return;
    toggle(false);
  };

  const isUploading = progressOptions
    ? progressOptions.filter((option) => option.open)
    : [];

  const renderItems = () => {
    return (
      <div ref={divRef}>
        <StyledContainerAction>
          {actionOptions.map((option) => (
            <StyledDropDownItem
              key={option.key}
              label={option.label}
              className={option.className}
              onClick={option.onClick}
              icon={option.icon ? option.icon : ""}
            />
          ))}
        </StyledContainerAction>
        <StyledProgressContainer
          isUploading={isUploading.length > 0 ? true : false}
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
                error={option.error}
              />
            ))}
        </StyledProgressContainer>
        <StyledButtonOptions isOpenButton={isOpenButton}>
          {isOpenButton && buttonOptions
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
                  />
                )
              )
            : ""}
        </StyledButtonOptions>
        {withButton && (
          <StyledButtonWrapper
            isUploading={isUploading.length > 0 ? true : false}
            isOpenButton={isOpenButton}
          >
            <Button
              label={title}
              className="action-mobile-button"
              primary
              size="large"
              onClick={onUploadClick}
            />
          </StyledButtonWrapper>
        )}
      </div>
    );
  };

  const children = renderItems();

  return (
    <div ref={ref} className={className} style={style}>
      <StyledFloatingButton
        icon={isOpen ? "minus" : "plus"}
        onClick={onMainButtonClick}
        percent={percent}
        color={"#ed7309"}
      />
      <StyledDropDown
        open={isOpen}
        clickOutsideAction={outsideClick}
        manualWidth={manualWidth || "400px"}
        directionY="top"
        directionX="right"
        isMobile={isMobile || isTablet}
        heightProp={height}
      >
        {isMobile || isTablet ? (
          <Scrollbar
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
    </div>
  );
};

MainButtonMobile.propTypes = {
  /** Accepts css style */
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  /** Options for drop down items  */
  actionOptions: PropTypes.array.isRequired,
  /** If you need display progress bar components */
  progressOptions: PropTypes.array,
  /** Menu that opens by clicking on the button  */
  buttonOptions: PropTypes.array,
  /** The function that will be called after the button click  */
  onUploadClick: PropTypes.func,
  /** Show button inside drop down */
  withButton: PropTypes.bool,
  /** The parameter that is used with buttonOptions is needed to open the menu by clicking on the button */
  isOpenButton: PropTypes.bool,
  /** The name of the button in the drop down */
  title: PropTypes.string,
  /** Loading indicator */
  percent: PropTypes.number,
  /** Required if you need to specify the exact width of the drop down component */
  manualWidth: PropTypes.string,
  className: PropTypes.string,
  /** Tells when the dropdown should be opened */
  opened: PropTypes.bool,
  /** If you need close drop down  */
  onClose: PropTypes.func,
};

export default MainButtonMobile;
