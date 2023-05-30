import styled, { css } from "styled-components";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import Base from "@docspace/components/themes/base";
import StyledPinIcon from "./sub-components/StyledPinIcon";

const getDefaultStyles = ({ $currentColorScheme, theme }) =>
  $currentColorScheme &&
  css`
    margin-top: 2px;
    ${commonIconsStyles}
    svg {
      path {
        fill: ${theme.isBase && $currentColorScheme.main.accent};
      }
    }

    &:hover {
      svg {
        path {
          fill: ${theme.isBase && $currentColorScheme.main.accent};
        }
      }
    }
  `;

StyledPinIcon.defaultProps = { theme: Base };

export default styled(StyledPinIcon)(getDefaultStyles);
