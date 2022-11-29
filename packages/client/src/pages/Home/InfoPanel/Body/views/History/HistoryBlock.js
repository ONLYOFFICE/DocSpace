import React from "react";

import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import { parseAndFormatDate } from "../../helpers/DetailsHelper";
import HistoryBlockMessage from "./HistoryBlockMessage";
import HistoryBlockItemList from "./HistoryBlockItemList";
import HistoryBlockUser from "./HistoryBlockUser";
import { FeedItemTypes } from "@docspace/common/constants";

import { StyledHistoryBlock } from "../../styles/history";

const HistoryBlock = ({
  t,
  selection,
  feed,
  personal,
  culture,
  selectedFolder,
  selectionParentRoom,
  getInfoPanelItemIcon,
  checkAndOpenLocationAction,
  openUser,
  isVisitor,
}) => {
  const { target, initiator, json, groupedFeeds } = feed;

  const isUserAction = json.Item === FeedItemTypes.User && target;
  const isItemAction =
    json.Item === FeedItemTypes.File || json.Item === FeedItemTypes.Folder;

  return (
    <StyledHistoryBlock isUserAction={isUserAction}>
      <Avatar
        role="user"
        className="avatar"
        size="min"
        source={
          initiator.avatarSmall ||
          (initiator.displayName
            ? ""
            : initiator.email && "/static/images/@.react.svg")
        }
        userName={initiator.displayName}
      />
      <div className="info">
        <div className="title">
          <Text className="name">{initiator.displayName}</Text>
          {initiator.isOwner && (
            <Text className="secondary-info">
              {t("Common:Owner").toLowerCase()}
            </Text>
          )}
          <Text className="date">
            {parseAndFormatDate(json.ModifiedDate, personal, culture)}
          </Text>
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

        {isItemAction && (
          <HistoryBlockItemList
            t={t}
            items={[json, ...groupedFeeds]}
            getInfoPanelItemIcon={getInfoPanelItemIcon}
            checkAndOpenLocationAction={checkAndOpenLocationAction}
          />
        )}

        {isUserAction &&
          [target, ...groupedFeeds].map((user, i) => (
            <HistoryBlockUser
              isVisitor={isVisitor}
              key={user.id}
              user={user}
              withComma={i !== groupedFeeds.length}
              openUser={openUser}
            />
          ))}
      </div>
    </StyledHistoryBlock>
  );
};

export default HistoryBlock;
