import styled from "styled-components";
import CrossReactSvg from "../../../public/images/cross.react.svg";
import commonIconsStyles from "../utils/common-icons-style";

const StyledCrossIcon = styled(CrossReactSvg)`
  ${commonIconsStyles}

  path {
    fill: #999976;
  }
`;

export default StyledCrossIcon;
