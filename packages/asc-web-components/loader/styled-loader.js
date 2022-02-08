import styled, { keyframes } from "styled-components";
import PropTypes from "prop-types";
import Base from "../themes/base";

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
  background-color: ${(props) =>
    props.color ? props.color : props.theme.loader.color};
  border-radius: ${(props) => props.theme.loader.borderRadius};
  width: ${(props) => props.size / 9}px;
  height: ${(props) => props.size / 9}px;
  margin-right: ${(props) => props.theme.loader.marginRight};
  /* Animation */
  animation: ${BounceAnimation} 0.5s linear infinite;
  animation-delay: ${(props) => props.delay};
`;

Dot.propTypes = {
  delay: PropTypes.string.isRequired,
  color: PropTypes.string.isRequired,
  size: PropTypes.number.isRequired,
};
Dot.defaultProps = { theme: Base };

const LoadingWrapper = styled.div`
  display: flex;
  align-items: baseline;

  color: ${(props) => props.color};
  font-size: ${(props) => props.size}px;
`;

const LoadingLabel = styled.span`
  margin-right: ${(props) => props.theme.loader.marginRight};
`;
LoadingLabel.defaultProps = { theme: Base };

const StyledOval = styled.svg`
  width: ${(props) => (props.size ? props.size : props.theme.loader.size)};
  height: ${(props) => (props.size ? props.size : props.theme.loader.size)};
  stroke: ${(props) => (props.color ? props.color : props.theme.loader.color)};
`;
StyledOval.defaultProps = { theme: Base };

const StyledDualRing = styled.svg`
  width: ${(props) => (props.size ? props.size : props.theme.loader.size)};
  height: ${(props) => (props.size ? props.size : props.theme.loader.size)};
  stroke: ${(props) => (props.color ? props.color : props.theme.loader.color)};
`;
StyledDualRing.defaultProps = { theme: Base };

export {
  LoadingLabel,
  LoadingWrapper,
  DotWrapper,
  Dot,
  StyledOval,
  StyledDualRing,
};
