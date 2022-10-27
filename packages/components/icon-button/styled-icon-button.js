import styled from "styled-components";
import { Base } from "../themes";

const StyledOuter = styled.div`
  width: ${(props) =>
    props.size ? Math.abs(parseInt(props.size)) + "px" : "20px"};
  height: ${(props) =>
    props.size ? Math.abs(parseInt(props.size)) + "px" : "20px"};
  cursor: ${(props) =>
    props.isDisabled || !props.isClickable ? "default" : "pointer"};
  line-height: 0;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);
  svg {
    &:not(:root) {
      width: 100%;
      height: 100%;
    }
    path {
      fill: ${(props) =>
        props.color ? props.color : props.theme.iconButton.color} !important;
    }
  }
  &:hover {
    svg {
      path {
        fill: ${(props) =>
          props.isDisabled
            ? props.theme.iconButton.color
            : props.color
            ? props.color
            : props.theme.iconButton.hoverColor};
      }
    }
  }
`;

StyledOuter.defaultProps = { theme: Base };

export default StyledOuter;
