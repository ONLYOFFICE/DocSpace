import styled, { css } from "styled-components";
import { StyledPinIcon } from "@appserver/components/badge";
import IconButton from "@appserver/components/icon-button";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";

const getDefaultStyles = ({ currentColorScheme }) => css`
  /* svg {
    path {
      fill: ${theme === "Base"
    ? currentColorScheme.accentColor
    : theme.filesSection.rowView.editingIconColor};
    }
  }

  &:hover {
    svg {
      path {
        fill: ${theme === "Base"
    ? currentColorScheme.accentColor
    : theme.filesSection.rowView.editingIconColor};
      }
    }
  } */

  ${IconButton} {
    ${commonIconsStyles}
    svg {
      path {
        fill: ${currentColorScheme.accentColor};
      }
    }
  }
`;

export default styled(IconButton)([getDefaultStyles]);
