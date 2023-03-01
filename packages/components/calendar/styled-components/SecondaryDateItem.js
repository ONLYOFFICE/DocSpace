import styled from "styled-components";
import { DateItem } from "./";
import Base from "../../themes/base";

export const SecondaryDateItem = styled(DateItem)`
  color: ${(props) =>
    props.disabled
      ? props.theme.calendar.disabledColor
      : props.theme.calendar.pastColor};

  :hover {
    cursor: ${(props) => (props.disabled ? "auto" : "pointer")};
    color: ${(props) =>
      props.disabled
        ? props.theme.calendar.disabledColor
        : props.theme.calendar.pastColor};
  }
`;
SecondaryDateItem.defaultProps = { theme: Base };
