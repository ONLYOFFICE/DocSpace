import EyeReactSvgUrl from "PUBLIC_DIR/images/eye.react.svg?url";
import CopyReactSvgUrl from "PUBLIC_DIR/images/copy.react.svg?url";
import AccessEditReactSvgUrl from "PUBLIC_DIR/images/access.edit.react.svg?url";
import InvitationLinkReactSvgUrl from "PUBLIC_DIR/images/invitation.link.react.svg?url";
import DownloadReactSvgUrl from "PUBLIC_DIR/images/download.react.svg?url";
import DownloadAsReactSvgUrl from "PUBLIC_DIR/images/download-as.react.svg?url";
import RenameReactSvgUrl from "PUBLIC_DIR/images/rename.react.svg?url";
import TrashReactSvgUrl from "PUBLIC_DIR/images/trash.react.svg?url";
import DuplicateReactSvgUrl from "PUBLIC_DIR/images/duplicate.react.svg?url";

import {
  ContextMenuAction,
  IFile,
  OmitSecondArg,
  TranslationType,
} from "../types";

type Functions = {
  onClickDownloadAs: VoidFunction;
  onMoveAction: VoidFunction;
  onCopyAction: VoidFunction;
  onClickRename: OmitSecondArg<ContextMenuAction>;
  onDuplicate: ContextMenuAction;
  onClickDelete: ContextMenuAction;
  onClickDownload: ContextMenuAction;
  onClickLinkEdit: OmitSecondArg<ContextMenuAction>;
  onPreviewClick: OmitSecondArg<ContextMenuAction>;
  onCopyLink: ContextMenuAction;
};

export const getPDFContextModel = (
  t: TranslationType,
  item: IFile,
  funcs: Functions
) => {
  const options = [
    {
      id: "option_edit",
      key: "edit",
      label: t("Common:EditButton"),
      icon: AccessEditReactSvgUrl,
      onClick: () => funcs.onClickLinkEdit(item),
      disabled: !item.security.Edit,
    },
    {
      id: "option_preview",
      key: "preview",
      label: t("Common:Preview"),
      icon: EyeReactSvgUrl,
      onClick: () => funcs.onPreviewClick(item),
      disabled: !item.security.Read,
    },
    {
      key: "separator0",
      isSeparator: true,
    },
    {
      id: "option_link-for-room-members",
      key: "link-for-room-members",
      label: t("LinkForRoomMembers"),
      icon: InvitationLinkReactSvgUrl,
      onClick: () => funcs.onCopyLink(item, t),
      disabled: false,
    },
    {
      id: "option_copy-to",
      key: "copy-to",
      label: t("Translations:Copy"),
      icon: CopyReactSvgUrl,
      onClick: funcs.onCopyAction,
      disabled: !item.security.Copy,
    },
    {
      id: "option_create-copy",
      key: "copy",
      label: t("Common:Duplicate"),
      icon: DuplicateReactSvgUrl,
      onClick: () => funcs.onDuplicate(item, t),
      disabled: !item.security.Duplicate,
    },
    {
      key: "download",
      label: t("Common:Download"),
      icon: DownloadReactSvgUrl,
      onClick: () => funcs.onClickDownload(item, t),
      disabled: false,
    },
    {
      id: "option_download-as",
      key: "download-as",
      label: t("Translations:DownloadAs"),
      icon: DownloadAsReactSvgUrl,
      onClick: funcs.onClickDownloadAs,
      disabled: false,
    },
    {
      id: "option_rename",
      key: "rename",
      label: t("Rename"),
      icon: RenameReactSvgUrl,
      onClick: () => funcs.onClickRename(item),
      disabled: !item.security.Rename,
    },
    {
      key: "separator1",
      isSeparator: true,
    },
    {
      key: "delete",
      label: t("Common:Delete"),
      icon: TrashReactSvgUrl,
      onClick: () => funcs.onClickDelete(item, t),
      disabled: !item.security.Delete,
    },
  ];

  return options;
};
