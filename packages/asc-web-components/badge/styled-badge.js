import styled from "styled-components";
import Base from "../themes/base";

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

  :hover {
    border-color: ${(props) => props.backgroundColor};
  }
`;
StyledBadge.defaultProps = { theme: Base };

const StyledInner = styled.div`
  background-color: ${(props) => props.backgroundColor};
  border-radius: ${(props) => props.borderRadius};
  padding: ${(props) => props.padding};
  max-width: ${(props) => props.maxWidth};
  text-align: center;
  user-select: none;
  line-height: ${(props) => props.lineHeight};
`;

export { StyledBadge, StyledInner };
