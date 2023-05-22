import styled, { css } from "styled-components";
import Base from "../themes/base";

import Text from "../text";

const hoveredCss = css`
  border-color: ${(props) =>
    props.backgroundColor
      ? props.backgroundColor
      : props.theme.badge.backgroundColor};
`;

const highCss = css`
  background-color: #f2675a !important;
  border-radius: 6px;
  padding: 0 10px;
  height: 24px;
  cursor: default;
  p {
    font-size: 13px;
    line-height: 20px;
    font-weight: 400;
  }
`;

const noBorderCss = css`
  border: none;
`;

const StyledBadge = styled.div`
  display: ${(props) =>
    props.label.length > 0 || props.label != "0" ? "inline-block" : "none"};
  border: ${(props) => props.theme.badge.border};
  border-radius: ${(props) => props.borderRadius};
  width: fit-content;
  padding: ${(props) => props.theme.badge.padding};
  line-height: ${(props) => props.theme.badge.lineHeight};
  cursor: pointer;
  overflow: ${(props) => props.theme.badge.overflow};
  flex-shrink: 0;
  border: ${(props) => props.border};
  ${(props) => props.type === "high" && noBorderCss}
  &:hover {
    ${(props) => !props.noHover && hoveredCss};
  }
  ${(props) => !props.noHover && props.isHovered && hoveredCss};
`;
StyledBadge.defaultProps = { theme: Base };

const StyledInner = styled.div`
  background-color: ${(props) =>
    props.backgroundColor
      ? props.backgroundColor
      : props.theme.badge.backgroundColor};
  border-radius: ${(props) => props.borderRadius};
  padding: ${(props) => props.padding};
  max-width: ${(props) => props.maxWidth};
  height: ${(props) => props.height};
  text-align: center;
  user-select: none;
  line-height: ${(props) => props.lineHeight};
  display: flex;
  align-items: center;
  justify-content: center;
  ${(props) => props.type === "high" && highCss}
`;

StyledInner.defaultProps = { theme: Base };

const StyledText = styled(Text)`
  color: ${(props) =>
    props.color ? props.color : props.theme.badge.color} !important;
`;

StyledText.defaultProps = { theme: Base };

export { StyledBadge, StyledInner, StyledText };
