import styled, { css } from "styled-components";
import React from "react";
import Text from "../text";
import Base from "../themes/base";
import NoUserSelect from "../utils/commonStyles";

const colorCss = css`
  color: ${(props) => (props.color ? props.color : props.theme.link.color)};
`;

const hoveredCss = css`
  ${colorCss};
  text-decoration: ${(props) =>
    props.type === "page"
      ? props.theme.link.hover.page.textDecoration
      : props.theme.link.hover.textDecoration};
`;

// eslint-disable-next-line react/prop-types, no-unused-vars
const PureText = ({ type, color, ...props }) => <Text {...props} />;

const StyledText = styled(PureText)`
  text-decoration: ${(props) => props.theme.link.textDecoration};

  ${(props) => !props.enableUserSelect && NoUserSelect}

  cursor: ${(props) => props.theme.link.cursor};
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  opacity: ${(props) => props.isSemitransparent && props.theme.link.opacity};
  line-height: ${(props) => props.theme.link.lineHeight};

  ${colorCss};

  &:hover {
    ${(props) => !props.noHover && hoveredCss};
  }

  ${(props) => !props.noHover && props.isHovered && hoveredCss}

  ${(props) =>
    props.isTextOverflow &&
    css`
      display: ${(props) => props.theme.link.display};
      max-width: 100%;
    `}
`;

StyledText.defaultProps = {
  theme: Base,
};

export default StyledText;
