import styled, { css } from "styled-components";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import StyledIcon from "@docspace/client/src/components/StyledIcon";
import Base from "@docspace/components/themes/base";

const getDefaultStyles = ({
  $currentColorScheme,
  shared,
  locked,
  isFavorite,
  isEditing,
  theme,
}) =>
  $currentColorScheme &&
  css`
    ${commonIconsStyles}
    svg {
      path {
        fill: ${(shared || locked || isFavorite || isEditing) &&
        theme.isBase &&
        $currentColorScheme.accentColor};
      }
    }

    &:hover {
      svg {
        path {
          fill: ${theme.isBase && $currentColorScheme.accentColor};
        }
      }
    }
  `;

StyledIcon.defaultProps = { theme: Base };

export default styled(StyledIcon)(getDefaultStyles);
