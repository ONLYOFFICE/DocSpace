import styled, { css } from "styled-components";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import Base from "@docspace/components/themes/base";
import StyledIcon from "./sub-components/StyledIcon";

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
        $currentColorScheme.main.accent};
      }
    }

    &:hover {
      svg {
        path {
          fill: ${$currentColorScheme.main.accent};
        }
      }
    }
  `;

StyledIcon.defaultProps = { theme: Base };

export default styled(StyledIcon)(getDefaultStyles);
