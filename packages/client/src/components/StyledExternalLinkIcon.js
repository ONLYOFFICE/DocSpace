import ExternalLinkIcon from "PUBLIC_DIR/images/external.link.react.svg";
import styled from "styled-components";

const StyledExternalLinkIcon = styled(ExternalLinkIcon)`
  height: 12px;
  width: 12px;
  margin: 0 4px;
  path {
    fill: ${(props) => props.color};
  }
`;

export default StyledExternalLinkIcon;
