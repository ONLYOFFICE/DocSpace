import styled, { css } from "styled-components";
import {
  Container,
  CurrentDateItem,
  HeaderActionIcon,
} from "@docspace/components/calendar/styled-components";

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
    ${HeaderActionIcon} {
      border-color: ${$currentColorScheme.main.accent};
    }
  `;

export default styled(Container)(getDefaultStyles);
