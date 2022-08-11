import styled from "styled-components";

import commonIconsStyles from "@docspace/components/utils/common-icons-style";

import FavoriteIcon from "../../public/images/favorite.react.svg";
import FileActionsConvertEditDocIcon from "../../public/images/file.actions.convert.edit.doc.react.svg";
import FileActionsLockedIcon from "../../public/images/file.actions.locked.react.svg";
import EditFormIcon from "../../public/images/file.actions.edit.form.react.svg";
import Base from "@docspace/components/themes/base";

export const EncryptedFileIcon = styled.div`
  background: url("images/security.svg") no-repeat 0 0 / 16px 16px transparent;
  height: 16px;
  position: absolute;
  width: 16px;
  margin-top: 14px;
  margin-left: 12px;
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
