import styled, { css } from "styled-components";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import StyledPinIcon from "@docspace/client/src/components/StyledPinIcon";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({ currentColorScheme, theme }) =>
  currentColorScheme &&
  css`
    ${commonIconsStyles}
    svg {
      path {
        fill: ${theme.isBase && currentColorScheme.accentColor};
      }
    }

    &:hover {
      svg {
        path {
          fill: ${theme.isBase && currentColorScheme.accentColor};
        }
      }
    }
  `;

StyledPinIcon.defaultProps = { theme: Base };

export default styled(StyledPinIcon)(getDefaultStyles);
