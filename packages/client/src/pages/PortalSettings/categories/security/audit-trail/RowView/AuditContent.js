import RowContent from "@docspace/components/row-content";
import React from "react";
import Text from "@docspace/components/text";
import moment from "moment";
import styled from "styled-components";
import { UnavailableStyles } from "../../../../utils/commonSettingsStyles";

const StyledRowContent = styled(RowContent)`
  padding-bottom: 10px;
  .row-main-container-wrapper {
    display: flex;
    justify-content: flex-start;
  }

  ${(props) => props.isSettingNotPaid && UnavailableStyles}
`;

export const AuditContent = ({ sectionWidth, item, isSettingNotPaid }) => {
  const DATE_FORMAT = "YYYY-MM-DD LT";
  const to = moment(item.date).local();

  const dateStr = to.format(DATE_FORMAT);

  return (
    <StyledRowContent
      sideColor="#A3A9AE"
      nameColor="#D0D5DA"
      sectionWidth={sectionWidth}
      isSettingNotPaid={isSettingNotPaid}>
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
        className="settings_unavailable">
        {dateStr}
      </Text>
      <Text
        fontSize="12px"
        as="div"
        fontWeight={600}
        className="settings_unavailable">
        {`${item.context ? item.context + " |" : ""} ${item.action}`}
      </Text>
    </StyledRowContent>
  );
};
