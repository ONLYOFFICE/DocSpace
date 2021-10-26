import { Scrollbars } from "react-custom-scrollbars";
import styled from "styled-components";
import Base from "../themes/base";

const StyledScrollbar = styled(Scrollbars)`
  .nav-thumb-vertical {
    background-color: ${(props) =>
      props.color
        ? props.color
        : props.theme.scrollbar.backgroundColorVertical};
    z-index: 1;
  }
  .nav-thumb-horizontal {
    background-color: ${(props) =>
      props.color
        ? props.color
        : props.theme.scrollbar.backgroundColorHorizontal};
  }

  .nav-thumb-vertical:hover {
    background-color: ${(props) =>
      props.theme.scrollbar.hoverBackgroundColorVertical};
  }
`;

StyledScrollbar.defaultProps = {
  theme: Base,
};

export default StyledScrollbar;
