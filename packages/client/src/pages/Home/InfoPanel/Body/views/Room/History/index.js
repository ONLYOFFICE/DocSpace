import Avatar from "@docspace/components/avatar";
import ContextMenuButton from "@docspace/components/context-menu-button";
import Text from "@docspace/components/text";
import React from "react";
import { StyledTitle } from "../../../styles/styles";
import {
  StyledHistoryBlock,
  StyledHistoryList,
} from "../../../styles/VirtualRoom/history";
import { fillingFormsVR } from "../../mock_data";
import getCorrectDate from "@docspace/components/utils/getCorrectDate";
import HistoryBlockContent from "./historyBlockContent";

const History = ({ t, personal, culture }) => {
  const data = fillingFormsVR;

  const parseAndFormatDate = (date) => {
    const locale = personal ? localStorage.getItem(LANGUAGE) : culture;
    const correctDate = getCorrectDate(locale, date);
    return correctDate;
  };

  return (
    <>
      <StyledTitle withBottomBorder>
        <img className="icon is-room" src={data.icon} alt="thumbnail-icon" />
        <Text className="text">{data.title}</Text>
        <ContextMenuButton getData={() => {}} className="context-menu-button" />
      </StyledTitle>

      <StyledHistoryList>
        {data.history.map((operation) => (
          <StyledHistoryBlock key={operation.id}>
            <Avatar
              role="user"
              className="avatar"
              size="min"
              source={
                operation.user.avatar ||
                (operation.user.displayName
                  ? ""
                  : operation.user.email && "/static/images/@.react.svg")
              }
              userName={operation.user.displayName}
            />
            <div className="info">
              <div className="title">
                <Text className="name">{operation.user.displayName}</Text>
                {operation.user.isOwner && (
                  <Text className="secondary-info">
                    {t("Common:Owner").toLowerCase()}
                  </Text>
                )}
                <Text className="date">
                  {parseAndFormatDate(operation.date)}
                </Text>
              </div>

              <HistoryBlockContent
                t={t}
                action={operation.action}
                details={operation.details}
              />
            </div>
          </StyledHistoryBlock>
        ))}
      </StyledHistoryList>
    </>
  );
};

export default History;
