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
  cursor: default;
  padding: 3px 10px;
  border-radius: 6px;

  p {
    font-size: 13px;
    font-weight: 400;
  }
`;

const noBorderCss = css`
  border: none;
  border-radius: 6px;
`;

const StyledBadge = styled.div`
  display: ${(props) =>
    props.label.length > 0 || props.label != "0" ? "flex" : "none"};
  align-items: center;
  justify-content: center;
  border: ${(props) => props.theme.badge.border};
  border-radius: ${(props) => props.borderRadius};
  width: fit-content;
  padding: ${(props) => props.theme.badge.padding};

  height: ${(props) => props.height};
  line-height: ${(props) => props.lineHeight};
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
  max-width: ${(props) => props.maxWidth};
  padding: ${(props) => props.padding};
  text-align: center;
  user-select: none;
  line-height: ${(props) => (props.compact ? "0.8" : "1.5")};
  display: flex;
  align-items: center;
  justify-content: center;
  ${(props) => props.type === "high" && highCss}
`;

StyledInner.defaultProps = { theme: Base };

const StyledText = styled(Text)`
  color: ${(props) =>
    props.color ? props.color : props.theme.badge.color} !important;
  border-radius: ${(props) => props.borderRadius};
`;

StyledText.defaultProps = { theme: Base };

export { StyledBadge, StyledInner, StyledText };
