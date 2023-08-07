import React from "react";
import { useTranslation } from "react-i18next";
import RowContent from "@docspace/components/row-content";
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
  const { t } = useTranslation(["Management", "Common", "Settings"]);

  // const { rooms, users, storage } = item?.data;
  // const gb = t("Common:Gigabyte");
  // const translateStorage = `${storage} ${gb}/50 ${gb}`;

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
        {/** TODO: add display current space */}
        {/* {item.isCurrent ? "Current space" : ""} */}
      </Text>
      <Text fontSize="12px" as="div" fontWeight={600}>
        {/* {`${t("PortalStats", {
          roomCount: rooms,
          userCount: users,
        })} ${translateStorage}`} */}
      </Text>
    </StyledRowContent>
  );
};
