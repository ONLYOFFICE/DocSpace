import RowContent from "@docspace/components/row-content";
import React from "react";
import Text from "@docspace/components/text";
import moment from "moment";
import styled from "styled-components";

const StyledRowContent = styled(RowContent)`
  .row-main-container-wrapper {
    display: flex;
    justify-content: space-between;
    width: 100%;
  }
`;

export const HistoryContent = ({ sectionWidth, item }) => {
  const DATE_FORMAT = "YYYY-MM-DD LT";
  const to = moment(item.date).local();

  const dateStr = to.format(DATE_FORMAT);
  return (
    <StyledRowContent
      sideColor="#A3A9AE"
      nameColor="#D0D5DA"
      sectionWidth={sectionWidth}
    >
      <div className="user-container-wrapper">
        <Text fontWeight={600} fontSize="14px" isTextOverflow={true}>
          {item.user}
        </Text>
      </div>

      <Text
        containerMinWidth="120px"
        fontSize="12px"
        fontWeight={600}
        truncate={true}
      >
        {dateStr}
      </Text>
      <Text fontSize="12px" as="div" fontWeight={600}>
        {item.action}
      </Text>
    </StyledRowContent>
  );
};
