import styled from "styled-components";
import { DateItem } from "./";

export const SecondaryDateItem = styled(DateItem)`
  color: #a3a9ae;

  :hover {
    cursor: ${(props) => (props.disabled ? "auto" : "pointer")};
    color: ${(props) => (props.disabled ? "a3a9ae" : "#333333")};
  }
`;
