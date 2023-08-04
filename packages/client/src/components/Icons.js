import SecuritySvgUrl from "PUBLIC_DIR/images/security.svg?url";
import styled from "styled-components";

import commonIconsStyles from "@docspace/components/utils/common-icons-style";

import FavoriteIcon from "PUBLIC_DIR/images/favorite.react.svg";
import FileActionsConvertEditDocIcon from "PUBLIC_DIR/images/file.actions.convert.edit.doc.react.svg";
import FileActionsLockedIcon from "PUBLIC_DIR/images/file.actions.locked.react.svg";
import EditFormIcon from "PUBLIC_DIR/images/access.edit.form.react.svg";
import Base from "@docspace/components/themes/base";

export const EncryptedFileIcon = styled.div`
  background: url(${SecuritySvgUrl}) no-repeat 0 0 / 16px 16px transparent;
  height: 16px;
  position: absolute;
  width: 16px;
  margin-top: 14px;
  ${props =>
    props.theme.interfaceDirection === "rtl"
      ? css`
              margin-right: 12px;
            `
      : css`
              margin-left: 12px;
            `}
`;

export const StyledFavoriteIcon = styled(FavoriteIcon)`
  ${commonIconsStyles}
`;

export const StyledFileActionsConvertEditDocIcon = styled(
  FileActionsConvertEditDocIcon
)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.filesIcons.fill};
  }

  &:hover {
    path {
      fill: ${(props) => props.theme.filesIcons.hoverFill};
    }
  }
`;

StyledFileActionsConvertEditDocIcon.defaultProps = { theme: Base };

export const StyledFileActionsLockedIcon = styled(FileActionsLockedIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.filesIcons.fill};
  }

  &:hover {
    path {
      fill: ${(props) => props.theme.filesIcons.hoverFill};
    }
  }
`;

StyledFileActionsLockedIcon.defaultProps = { theme: Base };
export const StyledFileActionsEditFormIcon = styled(EditFormIcon)`
  ${commonIconsStyles}
  path {
    fill: ${(props) => props.theme.filesIcons.fill};
  }

  &:hover {
    path {
      fill: ${(props) => props.theme.filesIcons.hoverFill};
    }
  }
`;

StyledFileActionsEditFormIcon.defaultProps = { theme: Base };
