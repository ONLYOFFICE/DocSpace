import React from "react";
import AtReactSvgUrl from "PUBLIC_DIR/images/@.react.svg?url";
import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import HistoryBlockMessage from "./HistoryBlockMessage";
import HistoryBlockItemList from "./HistoryBlockItemList";
import HistoryBlockUser from "./HistoryBlockUser";
import { FeedItemTypes } from "@docspace/common/constants";
import DefaultUserAvatarSmall from "PUBLIC_DIR/images/default_user_photo_size_32-32.png";
import { StyledHistoryBlock } from "../../styles/history";
import { getDateTime } from "../../helpers/HistoryHelper";
import { decode } from "he";

const HistoryBlock = ({
  t,
  selection,
  selectionIsFile,
  feed,
  selectedFolder,
  selectionParentRoom,
  getInfoPanelItemIcon,
  checkAndOpenLocationAction,
  openUser,
  isVisitor,
  isCollaborator,
  withFileList,
  isLastEntity,
}) => {
  const { target, initiator, json, groupedFeeds } = feed;

  const users = [target, ...groupedFeeds].filter(
    (user, index, self) =>
      self.findIndex((user2) => user2?.id === user?.id) === index
  );

  const isUserAction = json.Item === FeedItemTypes.User && target;
  const isItemAction =
    json.Item === FeedItemTypes.File || json.Item === FeedItemTypes.Folder;

  const userAvatar = initiator.hasAvatar
    ? initiator.avatarSmall
    : DefaultUserAvatarSmall;

  return (
    <StyledHistoryBlock
      withBottomDivider={!isLastEntity}
      isUserAction={isUserAction}
    >
      <Avatar
        role="user"
        className="avatar"
        size="min"
        source={
          userAvatar ||
          (initiator.displayName ? "" : initiator.email && AtReactSvgUrl)
        }
        userName={initiator.displayName}
      />
      <div className="info">
        <div className="title">
          <Text className="name">{decode(initiator.displayName)}</Text>
          {initiator.isOwner && (
            <Text className="secondary-info">
              {t("Common:Owner").toLowerCase()}
            </Text>
          )}
          <Text className="date">{getDateTime(json.ModifiedDate)}</Text>
        </div>

        <HistoryBlockMessage
          t={t}
          className="message"
          action={json}
          groupedActions={groupedFeeds}
          selection={selection}
          selectedFolder={selectedFolder}
          selectionParentRoom={selectionParentRoom}
        />

        {isItemAction && withFileList && (
          <HistoryBlockItemList
            t={t}
            items={[json, ...groupedFeeds]}
            getInfoPanelItemIcon={getInfoPanelItemIcon}
            checkAndOpenLocationAction={checkAndOpenLocationAction}
          />
        )}

        {isUserAction &&
          users.map((user, i) => (
            <HistoryBlockUser
              isVisitor={isVisitor}
              isCollaborator={isCollaborator}
              key={user.id}
              user={user}
              withComma={i < users.length - 1}
              openUser={openUser}
            />
          ))}
      </div>
    </StyledHistoryBlock>
  );
};

export default HistoryBlock;
