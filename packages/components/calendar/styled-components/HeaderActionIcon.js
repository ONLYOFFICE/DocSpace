import styled from "styled-components";
import { ArrowIcon } from "./ArrowIcon";

export const HeaderActionIcon = styled(ArrowIcon)`
  transform: rotate(225deg);
  top: 11px;
  right: -14.29px;
  border-color: ${(props) => props.theme.calendar.outlineColor};
`;
