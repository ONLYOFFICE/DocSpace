import React from "react";

import { fillingFormsVR } from "../mock_data";

import {
  StyledHistoryBlock,
  StyledHistoryList,
  StyledHistorySubtitle,
} from "../../styles/history";
import HistoryBlockContent from "./historyBlockContent";

import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import getCorrectDate from "@docspace/components/utils/getCorrectDate";

const History = ({ t, personal, culture }) => {
  const data = fillingFormsVR;

  const parseAndFormatDate = (date) => {
    const locale = personal ? localStorage.getItem(LANGUAGE) : culture;
    const correctDate = getCorrectDate(locale, date);
    return correctDate;
  };

  return (
    <>
      <StyledHistoryList>
        <StyledHistorySubtitle>Recent activities</StyledHistorySubtitle>

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
