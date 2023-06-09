import styled, { css } from "styled-components";
import { DateItem } from "@docspace/components/calendar/styled-components";

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
    ${(props) =>
      props.isCurrent &&
      css`
        background: ${$currentColorScheme.main.accent};
        :hover {
          background-color: ${$currentColorScheme.main.accent};
        }

        :focus {
          background-color: ${$currentColorScheme.main.accent};
        }
      `}
    color: ${(props) =>
      props.disabled
        ? props.theme.calendar.disabledColor
        : props.focused
        ? $currentColorScheme.main.accent
        : props.theme.calendar.color};
    border-color: ${(props) => (props.focused ? $currentColorScheme.main.accent : "transparent")};
  `;

export default styled(DateItem)(getDefaultStyles);
