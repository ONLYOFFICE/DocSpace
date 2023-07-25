import RowContent from "@docspace/components/row-content";
import React from "react";
import Text from "@docspace/components/text";
import styled from "styled-components";

const StyledRowContent = styled(RowContent)`
  padding-bottom: 10px;
  .row-main-container-wrapper {
    display: flex;
    justify-content: flex-start;
    width: min-content;
  }
`;

export const RoomContent = ({ item }) => {
  return (
    <StyledRowContent
      sectionWidth={"620px"}
      sideColor="#A3A9AE"
      nameColor="#D0D5DA"
      className="spaces_row-content"
    >
      <div className="user-container-wrapper">
        <Text fontWeight={600} fontSize="14px" isTextOverflow={true}>
          {`${item.domain}`}
        </Text>
      </div>

      <Text
        containerMinWidth="120px"
        fontSize="14px"
        fontWeight={600}
        truncate={true}
        color="#A3A9AE"
        className="spaces_row-current"
      >
        {item.isCurrent ? "Current space" : ""}
      </Text>
      <Text fontSize="12px" as="div" fontWeight={600}>
        {`Rooms: 0 | Users:  1 | Storage space used: 0 GB/50 GB`}
      </Text>
    </StyledRowContent>
  );
};
