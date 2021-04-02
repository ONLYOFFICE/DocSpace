import styled from "styled-components";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";
import FavoriteIcon from "../../../../../../../public/images/favorite.react.svg";
import FileActionsConvertEditDocIcon from "../../../../../../../public/images/file.actions.convert.edit.doc.react.svg";
import FileActionsLockedIcon from "../../../../../../../public/images/file.actions.locked.react.svg";

export const StyledFavoriteIcon = styled(FavoriteIcon)`
  ${commonIconsStyles}
`;

export const StyledFileActionsConvertEditDocIcon = styled(
  FileActionsConvertEditDocIcon
)`
  ${commonIconsStyles}
  path {
    fill: #3b72a7;
  }
`;

export const StyledFileActionsLockedIcon = styled(FileActionsLockedIcon)`
  ${commonIconsStyles}
  path {
    fill: #3b72a7;
  }
`;
