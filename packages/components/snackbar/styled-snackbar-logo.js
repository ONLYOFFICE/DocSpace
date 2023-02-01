import styled from "styled-components";
import InfoReactSvg from "PUBLIC_DIR/images/danger.toast.react.svg";
import commonIconsStyles from "../utils/common-icons-style";

const StyledLogoIcon = styled(InfoReactSvg)`
  ${commonIconsStyles}

  path {
    fill: ${(props) => props.color};
  }
`;

export default StyledLogoIcon;
