import styled, { css } from "styled-components";
import IconButton from "@docspace/components/icon-button";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";
import Base from "@docspace/components/themes/base";

const StyledEditIcon = styled(IconButton)`
  ${commonIconsStyles}

  svg {
    path {
      fill: ${(props) => props.theme.filesSection.rowView.editingIconColor};
    }
  }
`;

const getDefaultStyles = ({
  $currentColorScheme,
  shared,
  locked,
  isFavorite,
  isEditing,
  theme,
}) =>
  $currentColorScheme
    ? css`
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
      `
    : isEditing
    ? css`
        ${StyledEditIcon}
      `
    : css`
        ${IconButton} {
          ${commonIconsStyles}
        }
      `;

IconButton.defaultProps = { theme: Base };

export default styled(IconButton)(getDefaultStyles);
