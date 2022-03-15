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
import Backdrop from "../backdrop";

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
      <Text
        onClick={onClickHeaderAction}
        className="progress-header"
        fontSize={`14`}
        // color="#657077"
      >
        {label}
      </Text>
      <Text className="progress_count" fontSize={`13`}>
        {status}
      </Text>
      <IconButton
        className="progress_icon"
        onClick={onCancel}
        iconName={icon}
        size={14}
      />
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
  onClickAction: PropTypes.func,
  /** The function that hide button */
  hideButton: PropTypes.func,
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
    sectionWidth,
  } = props;

  const [isOpen, setIsOpen] = useState(opened);
  const [isUploading, setIsUploading] = useState(false);
  const [height, setHeight] = useState("90vh");

  const divRef = useRef();

  useEffect(() => {
    if (opened !== isOpen) {
      setIsOpen(opened);
    }
  }, [opened]);

  useEffect(() => {
    let height =
      divRef?.current?.getBoundingClientRect()?.height || window.innerHeight;
    height >= window.innerHeight ? setHeight("90vh") : setHeight(height + "px");
  }, [isOpen, isOpenButton, window.innerHeight, isUploading]);

  const ref = useRef();

  const dropDownRef = useRef();

  const toggle = (isOpen) => {
    if (isOpenButton && onClose) {
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

  React.useEffect(() => {
    if (progressOptions) {
      const openProgressOptions = progressOptions.filter(
        (option) => option.open
      );

      setIsUploading(openProgressOptions.length > 0);
    }
  }, [progressOptions]);

  const renderItems = () => {
    return (
      <div ref={divRef}>
        <StyledContainerAction>
          {actionOptions.map((option) => {
            const optionOnClickAction = () => {
              toggle(false);
              option.onClick && option.onClick({ action: option.action });
            };

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
                    action={option.action}
                  />
                )
              )
            : ""}
        </StyledButtonOptions>
        {withButton && (
          <StyledButtonWrapper
            isUploading={isUploading}
            isOpenButton={isOpenButton}
          >
            <Button
              label={title}
              className="action-mobile-button"
              primary
              size="medium"
              onClick={onUploadClick}
            />
          </StyledButtonWrapper>
        )}
      </div>
    );
  };

  const children = renderItems();

  return (
    <>
      <Backdrop zIndex={200} visible={isOpen} onClick={outsideClick} />
      <div ref={ref} className={className} style={{ zIndex: "201", ...style }}>
        <StyledFloatingButton
          icon={isOpen ? "minus" : "plus"}
          isOpen={isOpen}
          onClick={onMainButtonClick}
          percent={percent}
        />
        <StyledDropDown
          open={isOpen}
          withBackdrop={false}
          manualWidth={manualWidth || "400px"}
          directionY="top"
          directionX="right"
          isMobile={isMobile || isTablet}
          heightProp={height}
          sectionWidth={sectionWidth}
          isDefaultMode={false}
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
    </>
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
  /** Section width */
  sectionWidth: PropTypes.number,
  /** Required if you need to specify the exact width of the drop down component */
  manualWidth: PropTypes.string,
  className: PropTypes.string,
  /** Tells when the dropdown should be opened */
  opened: PropTypes.bool,
  /** If you need close drop down  */
  onClose: PropTypes.func,
};

export default MainButtonMobile;
