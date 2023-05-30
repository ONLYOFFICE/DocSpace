import styled, { css } from 'styled-components';
import {
  Container,
  DateItem,
  CurrentDateItem,
  RoundButton,
} from '@docspace/components/calendar/styled-components';

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
    ${CurrentDateItem} {
      background: ${$currentColorScheme.main.accent};
      :hover {
        background-color: ${$currentColorScheme.main.accent};
      }

      :focus {
        background-color: ${$currentColorScheme.main.accent};
      }
    }
    ${DateItem} {
      color: ${(props) =>
        props.disabled
          ? props.theme.calendar.disabledColor
          : props.focused
          ? $currentColorScheme.main.accent
          : props.theme.calendar.color};
      border-color: ${(props) => (props.focused ? $currentColorScheme.main.accent : 'transparent')};
      :focus {
        color: ${$currentColorScheme.main.accent};
        border-color: ${$currentColorScheme.main.accent};
      }
    }
    ${RoundButton} {
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
    }
  `;

export default styled(Container)(getDefaultStyles);
