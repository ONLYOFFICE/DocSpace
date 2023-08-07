import React from "react";
import PropTypes from "prop-types";

import Text from "../text";
import StyledProgressBar from "./styled-progress-bar";

const ProgressBar = ({ percent, label, ...rest }) => {
  const progressPercent = percent > 100 ? 100 : percent;

  //console.log("ProgressBar render");
  return (
    <StyledProgressBar {...rest} percent={progressPercent}>
      <div className="progress-bar_percent" />
      <Text
        className="progress-bar_full-text"
        fontSize="12px"
        fontWeight="400"
        lineHeight="16px"
        title={label}
      >
        {label}
      </Text>
    </StyledProgressBar>
  );
};

ProgressBar.propTypes = {
  /** Progress value in %. Max value 100% */
  percent: PropTypes.number.isRequired,
  /** Text in progress-bar. */
  label: PropTypes.string,
};

export default ProgressBar;
