import styled from "styled-components";
import CrossIcon from "../../../public/images/cross.react.svg";
import commonIconsStyles from "../utils/common-icons-style";

const StyledCrossIcon = styled(CrossIcon)`
  ${commonIconsStyles}

  path {
    fill: #999976;
  }
`;

export default StyledCrossIcon;
