import React from "react";
import PropTypes from "prop-types";
import styled, { keyframes } from "styled-components";

const BounceAnimation = keyframes`
  0% { margin-bottom: 0; display: none; }
  50% { margin-bottom: 1px;  }
  100% { margin-bottom: 0; display: none; }
`;

const DotWrapper = styled.div`
  display: flex;
  align-items: flex-end;
`;

const Dot = styled.div`
  background-color: ${(props) => props.color};
  border-radius: 50%;
  width: ${(props) => props.size / 9}px;
  height: ${(props) => props.size / 9}px;
  margin-right: 2px;
  /* Animation */
  animation: ${BounceAnimation} 0.5s linear infinite;
  animation-delay: ${(props) => props.delay};
`;

Dot.propTypes = {
  delay: PropTypes.string.isRequired,
  color: PropTypes.string.isRequired,
  size: PropTypes.number.isRequired,
};

const LoadingWrapper = styled.div`
  display: flex;
  align-items: baseline;

  color: ${(props) => props.color};
  font-size: ${(props) => props.size}px;
`;

const LoadingLabel = styled.span`
  margin-right: 2px;
`;

const LoadingDots = (props) => {
  const { label } = props;

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
  color: "#63686a",
  size: 18,
  label: "Loading content, please wait",
};

export { LoadingDots };
