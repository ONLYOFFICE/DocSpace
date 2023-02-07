import styled from "styled-components";
import { DateItem } from "./";
import Base from "../../themes/base";

export const SecondaryDateItem = styled(DateItem)`
  color: ${(props) => props.theme.calendar.disabledColor};

  :hover {
    cursor: ${(props) => (props.disabled ? "auto" : "pointer")};
    color: ${(props) =>
      props.disabled
        ? props.theme.calendar.disabledColor
        : props.theme.calendar.color};
  }
`;
SecondaryDateItem.defaultProps = { theme: Base };
