import styled from "styled-components";
import { ArrowIcon } from "./ArrowIcon";
import Base from "../../themes/base";

export const HeaderActionIcon = styled(ArrowIcon)`
  width: ${(props) => (props.isMobile ? "5px" : "6px")};
  height: ${(props) => (props.isMobile ? "5px" : "6px")};
  transform: rotate(225deg);
  top: ${(props) => (props.isMobile ? "11px" : "8.5px")};
  left: 104%;
  border-color: ${(props) => props.theme.calendar.accent};
`;

HeaderActionIcon.defaultProps = { theme: Base };
