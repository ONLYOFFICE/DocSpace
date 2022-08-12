import styled, { css } from "styled-components";
import IconButton from "@docspace/components/icon-button";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";

const getDefaultStyles = ({
  currentColorScheme,
  shared,
  locked,
  isFavorite,
  isEditing,
  isPin,
}) => css`
  ${commonIconsStyles}
  svg {
    path {
      fill: ${(shared || locked || isFavorite || isEditing || isPin) &&
      currentColorScheme.accentColor};
    }
  }

  &:hover {
    svg {
      path {
        fill: ${currentColorScheme.accentColor};
      }
    }
  }
`;

export default styled(IconButton)(getDefaultStyles);
