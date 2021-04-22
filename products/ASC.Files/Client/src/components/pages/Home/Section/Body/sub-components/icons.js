import React from "react";
import styled from "styled-components";

import commonIconsStyles from "@appserver/components/utils/common-icons-style";

import CheckIcon from "../../../../../../../public/images/check.react.svg";
import CrossIcon from "../../../../../../../../../../public/images/cross.react.svg";
import FavoriteIcon from "../../../../../../../public/images/favorite.react.svg";
import FileActionsConvertEditDocIcon from "../../../../../../../public/images/file.actions.convert.edit.doc.react.svg";
import FileActionsLockedIcon from "../../../../../../../public/images/file.actions.locked.react.svg";

export const EncryptedFileIcon = styled.div`
  background: url("images/security.svg") no-repeat 0 0 / 16px 16px transparent;
  height: 16px;
  position: absolute;
  width: 16px;
  margin-top: 14px;
  margin-left: ${(props) => (props.isEdit ? "40px" : "12px")};
`;

const StyledCheckIcon = styled(CheckIcon)`
  ${commonIconsStyles}
  path {
    fill: #a3a9ae;
  }
  :hover {
    fill: #657077;
  }
`;
export const okIcon = <StyledCheckIcon className="edit-ok-icon" size="scale" />;

const StyledCrossIcon = styled(CrossIcon)`
  ${commonIconsStyles}
  path {
    fill: #a3a9ae;
  }
  :hover {
    fill: #657077;
  }
`;
export const cancelIcon = (
  <StyledCrossIcon className="edit-cancel-icon" size="scale" />
);

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
