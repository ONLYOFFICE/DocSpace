import React from "react";
import moment from "moment";
import styled from "styled-components";

import Text from "@docspace/components/text";
import RowContent from "@docspace/components/row-content";

import StatusBadge from "../../../../sub-components/StatusBadge";
import { useTranslation } from "react-i18next";

const StyledRowContent = styled(RowContent)`
  display: flex;
  padding-bottom: 10px;

  .rowMainContainer {
    height: 100%;
    width: 100%;
  }
`;

const ContentWrapper = styled.div`
  display: flex;
  flex-direction: column;
  justify-items: center;
`;

const StatusHeader = styled.div`
  display: flex;
`;

export const HistoryRowContent = ({ sectionWidth, historyItem }) => {
  const { t, i18n } = useTranslation("Webhooks");

  const formattedDelivery =
    moment(historyItem.delivery)
      .locale(i18n.language)
      .format("MMM D, YYYY, h:mm:ss A") +
    " " +
    t("UTC");
  return (
    <StyledRowContent sectionWidth={sectionWidth}>
      <ContentWrapper>
        <StatusHeader>
          <Text
            fontWeight={600}
            fontSize="14px"
            style={{ marginInlineEnd: "8px" }}
          >
            {historyItem.id}
          </Text>
          <StatusBadge status={historyItem.status} />
        </StatusHeader>
        <Text fontWeight={600} fontSize="12px" color="#A3A9AE">
          {formattedDelivery}
        </Text>
      </ContentWrapper>
      <span></span>
    </StyledRowContent>
  );
};
