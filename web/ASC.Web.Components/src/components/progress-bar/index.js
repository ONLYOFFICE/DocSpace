import React, { useState } from "react";
import styled from "styled-components";
import PropTypes from "prop-types";
import DropDown from "../drop-down";
import Link from "./link";

const StyledProgressBar = styled.div`
  position: relative;
  height: 22px;
  background-color: #f8f9f9;

  .progress-bar_full-text {
    display: block;
    padding: 0px 6px;
    position: absolute;
    font-weight: 600;
    margin: 0;
  }

  .progress-bar_percent {
    width: ${(props) => props.uploadedPercent}%;
    float: left;
    overflow: hidden;
    max-height: 22px;
    min-height: 22px;
  }
  .progress-bar_field {
    width: ${(props) => props.remainPercent}%;
    float: left;
    overflow: hidden;
    max-height: 22px;
    min-height: 22px;
  }

  .progress-bar_percent {
    transition: width 0.6s ease;
    background: linear-gradient(90deg, #20d21f 75%, #b9d21f 100%);
  }

  .progress-bar_text {
    min-width: 200%;

    .progress-bar_progress-text {
      padding: 2px 6px;
      position: relative;
      margin: 0;
      min-width: 100px;
      font-weight: 600;
    }
  }
  .progress-bar_field .progress-bar_text {
    margin-left: -100%;
  }

  .progress-bar_drop-down {
    padding: 16px 16px 16px 17px;
  }
`;

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
        <DropDown open={isOpen} clickOutsideAction={onClose}>
          <div className="progress-bar_drop-down">{dropDownContent}</div>
        </DropDown>
      )}
    </StyledProgressBar>
  );
};

ProgressBar.propTypes = {
  percent: PropTypes.number.isRequired,
  label: PropTypes.string,
  dropDownContent: PropTypes.any,
};

export default ProgressBar;
