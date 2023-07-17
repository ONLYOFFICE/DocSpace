import styled, { css } from "styled-components";
import { RoundButton } from "@docspace/components/calendar/styled-components";

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
    :hover {
      outline: ${(props) =>
        props.disabled
          ? `1px solid ${props.theme.calendar.outlineColor}`
          : `2px solid ${$currentColorScheme.main.accent}`};
      span {
        border-color: ${(props) =>
          props.disabled ? props.theme.calendar.disabledArrow : $currentColorScheme.main.accent};
      }
    }
  `;

export default styled(RoundButton)(getDefaultStyles);
