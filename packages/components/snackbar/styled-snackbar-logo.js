import styled from "styled-components";
import InfoIcon from "../../../public/images/alert.react.svg";
import commonIconsStyles from "../utils/common-icons-style";

const StyledLogoIcon = styled(InfoIcon)`
  ${commonIconsStyles}

  path {
    fill: ${(props) => props.color};
  }
`;

export default StyledLogoIcon;
