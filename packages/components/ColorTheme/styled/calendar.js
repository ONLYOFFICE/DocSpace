import styled, { css } from "styled-components";
import {
  Container,
  CurrentDateItem,
  HeaderActionIcon,
} from "@docspace/components/calendar/styled-components";

const getDefaultStyles = ({ $currentColorScheme }) =>
  $currentColorScheme &&
  css`
    ${HeaderActionIcon} {
      border-color: ${$currentColorScheme.main.accent};
    }
  `;

export default styled(Container)(getDefaultStyles);
