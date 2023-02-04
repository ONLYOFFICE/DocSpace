import styled from "styled-components";
import { DateItem } from "./";

export const SecondaryDateItem = styled(DateItem)`
  color: #A3A9AE;

  :hover{
    cursor: ${props => props.disabled ? 'auto' : 'pointer'};
    color: #333333;
  }
`;
