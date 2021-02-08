import { Scrollbars } from "react-custom-scrollbars";
import styled from "styled-components";
import { Base } from "../../themes";

const StyledScrollbar = styled(Scrollbars)`
  .nav-thumb-vertical {
    background-color: ${props =>
      props.color
        ? props.color
        : props.theme.scrollbar.backgroundColorVertical};
  }
  .nav-thumb-horizontal {
    background-color: ${props =>
      props.color
        ? props.color
        : props.theme.scrollbar.backgroundColorHorizontal};
  }
`;

StyledScrollbar.defaultProps = {
  theme: Base
};

export default StyledScrollbar;