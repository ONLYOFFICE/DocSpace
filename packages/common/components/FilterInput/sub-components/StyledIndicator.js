import { Base } from "@appserver/components/themes";
import styled, { css } from "styled-components";

const StyledIndicator = styled.div`
  border-radius: 50%;
  width: 8px;
  height: 8px;
  position: absolute;
  top: 25px;
  left: 25px;

  z-index: 10;
`;

StyledIndicator.defaultProps = { theme: Base };

export default StyledIndicator;
