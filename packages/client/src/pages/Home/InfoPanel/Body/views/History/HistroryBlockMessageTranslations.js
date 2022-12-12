import React from "react";
import { Trans } from "react-i18next";

const {
  FeedActionTypes,
  FeedItemTypes,
} = require("@docspace/common/constants");

const getBlockMessageTranslation = (
  t,
  actionType,
  itemType,
  isSeveral,
  data
) => {
  const keys = [
    // FILE //
    {
      key: t("FeedCreateFileSingle", data),
      actionType: FeedActionTypes.Create,
      itemType: FeedItemTypes.File,
      isSeveral: false,
    },
    {
      key: t("FeedCreateFileSeveral", data),
      actionType: FeedActionTypes.Create,
      itemType: FeedItemTypes.File,
      isSeveral: true,
    },
    {
      key: t("FeedUpdateFile", data),
      actionType: FeedActionTypes.Update,
      itemType: FeedItemTypes.File,
    },
    {
      key: t("FeedRenameFile", data),
      actionType: FeedActionTypes.Rename,
      itemType: FeedItemTypes.File,
    },
    {
      key: t("FeedMoveFile", data),
      actionType: FeedActionTypes.Move,
      itemType: FeedItemTypes.File,
    },
    {
      key: t("FeedDeleteFile", data),
      actionType: FeedActionTypes.Delete,
      itemType: FeedItemTypes.File,
    },
    // FOLDER //
    {
      key: t("FeedCreateFolderSingle", data),
      actionType: FeedActionTypes.Create,
      itemType: FeedItemTypes.Folder,
      isSeveral: false,
    },
    {
      key: t("FeedCreateFolderSeveral", data),
      actionType: FeedActionTypes.Create,
      itemType: FeedItemTypes.Folder,
      isSeveral: true,
    },
    {
      key: t("FeedRenameFolder", data),
      actionType: FeedActionTypes.Rename,
      itemType: FeedItemTypes.Folder,
    },
    {
      key: t("FeedMoveFolder", data),
      actionType: FeedActionTypes.Move,
      itemType: FeedItemTypes.Folder,
    },
    {
      key: t("FeedDeleteFolder", data),
      actionType: FeedActionTypes.Delete,
      itemType: FeedItemTypes.Folder,
    },
    // ROOM //
    {
      key: t("FeedCreateRoom", data),
      actionType: FeedActionTypes.Create,
      itemType: FeedItemTypes.Room,
    },
    {
      key: t("FeedRenameRoom", data),
      actionType: FeedActionTypes.Rename,
      itemType: FeedItemTypes.Room,
    },
    {
      key: t("FeedUpdateRoom", data),
      actionType: FeedActionTypes.Update,
      itemType: FeedItemTypes.Room,
    },
    // TAG //
    {
      key: t("FeedCreateRoomTag", data),
      actionType: FeedActionTypes.Create,
      itemType: FeedItemTypes.Tag,
    },
    {
      key: t("FeedDeleteRoomTag", data),
      actionType: FeedActionTypes.Delete,
      itemType: FeedItemTypes.Tag,
    },
    // USER //
    {
      key: t("FeedCreateUser", data),
      actionType: FeedActionTypes.Create,
      itemType: FeedItemTypes.User,
    },
    {
      key: t("FeedUpdateUser", data),
      actionType: FeedActionTypes.Update,
      itemType: FeedItemTypes.User,
    },
    {
      key: t("FeedDeleteUser", data),
      actionType: FeedActionTypes.Delete,
      itemType: FeedItemTypes.User,
    },
  ];

  const [result] = keys.filter(
    (key) =>
      key.actionType === actionType &&
      key.itemType === itemType &&
      !!key.isSeveral === isSeveral
  );

  if (!result) return `${actionType} ${itemType}`;
  return (
    <Trans
      t={t}
      ns="InfoPanel"
      i18nKey={result.key}
      components={{ bold: <strong /> }}
    />
  );
};

export default getBlockMessageTranslation;
