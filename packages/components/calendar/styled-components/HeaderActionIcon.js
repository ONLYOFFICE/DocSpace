import styled from "styled-components";
import { ArrowIcon } from "./ArrowIcon";
import Base from "../../themes/base";

export const HeaderActionIcon = styled(ArrowIcon)`
  transform: rotate(225deg);
  top: 11px;
  right: -14.29px;
  border-color: ${(props) => props.theme.calendar.accent};
`;

HeaderActionIcon.defaultProps = { theme: Base };
