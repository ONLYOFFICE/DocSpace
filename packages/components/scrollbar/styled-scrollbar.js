import Scrollbar from "react-scrollbars-custom";
import styled from "styled-components";
import Base from "../themes/base";

const StyledScrollbar = styled(Scrollbar)`
  .scroll-body {
    position: relative;
  }
  .nav-thumb-vertical {
    background-color: ${(props) =>
      props.color
        ? props.color
        : props.theme.scrollbar.backgroundColorVertical} !important;
    z-index: 201;
    position: relative;

    :hover,
    :active,
    &.dragging {
      background-color: ${(props) =>
        props.theme.scrollbar.hoverBackgroundColorVertical} !important;
    }
  }
  .nav-thumb-horizontal {
    background-color: ${(props) =>
      props.color
        ? props.color
        : props.theme.scrollbar.backgroundColorHorizontal} !important;
  }
`;

StyledScrollbar.defaultProps = {
  theme: Base,
};

export default StyledScrollbar;
