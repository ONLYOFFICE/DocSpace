import React from "react";
import PropTypes from "prop-types";
import {
  LoadingLabel,
  LoadingWrapper,
  DotWrapper,
  Dot,
} from "../styled-loader";

const LoadingDots = (props) => {
  const { label, theme } = props;

  return (
    <LoadingWrapper {...props}>
      <LoadingLabel>{label}</LoadingLabel>
      <DotWrapper>
        <Dot {...props} delay="0s" />
        <Dot {...props} delay=".2s" />
        <Dot {...props} delay=".4s" />
      </DotWrapper>
    </LoadingWrapper>
  );
};

LoadingDots.propTypes = {
  color: PropTypes.string,
  size: PropTypes.number.isRequired,
  label: PropTypes.string.isRequired,
};

LoadingDots.defaultProps = {
  size: 18,
  label: "Loading content, please wait",
};

export { LoadingDots };
