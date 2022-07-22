import React, { useState } from "react";
import PropTypes from "prop-types";

import DropDown from "../drop-down";
import Link from "./link";
import StyledProgressBar from "./styled-progress-bar";

const ProgressBar = (props) => {
  const { percent, label, dropDownContent, ...rest } = props;
  const progressPercent = percent > 100 ? 100 : percent;
  const remainPercent = 100 - progressPercent;

  const ref = React.createRef();
  const [isOpen, setIsOpen] = useState(false);

  const onLinkClick = () => setIsOpen(!isOpen);

  const onClose = (e) => {
    if (ref.current.contains(e.target)) return;
    setIsOpen(!isOpen);
  };

  //console.log("ProgressBar render");
  return (
    <StyledProgressBar
      ref={ref}
      {...rest}
      uploadedPercent={progressPercent}
      remainPercent={remainPercent}
    >
      <Link
        className="progress-bar_full-text"
        color="#333"
        onClick={onLinkClick}
        isOpen={isOpen}
        showIcon={dropDownContent}
      >
        {label}
      </Link>
      <div className="progress-bar_percent">
        <div className="progress-bar_text">
          <Link
            className="progress-bar_progress-text"
            color="#fff"
            onClick={onLinkClick}
            isOpen={isOpen}
            showIcon={dropDownContent}
          >
            {label}
          </Link>
        </div>
      </div>
      <div className="progress-bar_field" />
      {dropDownContent && (
        <DropDown open={isOpen} clickOutsideAction={onClose} forwardedRef={ref}>
          <div className="progress-bar_drop-down">{dropDownContent}</div>
        </DropDown>
      )}
    </StyledProgressBar>
  );
};

ProgressBar.propTypes = {
  /** Progress value. */
  percent: PropTypes.number.isRequired,
  /** Text in progress-bar. */
  label: PropTypes.string,
  /** Drop-down content. */
  dropDownContent: PropTypes.any,
};

export default ProgressBar;
