import styled, { css } from "styled-components";
import IconButton from "@docspace/components/icon-button";
import commonIconsStyles from "@docspace/components/utils/common-icons-style";

const StyledEditIcon = styled(IconButton)`
  ${commonIconsStyles}

  svg {
    path {
      fill: ${(props) => props.theme.filesSection.rowView.editingIconColor};
    }
  }
`;

const getDefaultStyles = ({
  currentColorScheme,
  shared,
  locked,
  isFavorite,
  isEditing,
}) =>
  currentColorScheme
    ? css`
        ${commonIconsStyles}
        svg {
          path {
            fill: ${(shared || locked || isFavorite || isEditing) &&
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

export default styled(IconButton)(getDefaultStyles);
