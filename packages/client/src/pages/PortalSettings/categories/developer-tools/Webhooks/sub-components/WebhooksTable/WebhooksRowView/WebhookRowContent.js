import React from "react";
import styled from "styled-components";

import Text from "@docspace/components/text";
import RowContent from "@docspace/components/row-content";
import { ToggleButton } from "@docspace/components";

import { StatusBadge } from "../../StatusBadge";

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

const ToggleButtonWrapper = styled.div`
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: flex-end;
`;

const FlexWrapper = styled.div`
  display: flex;
`;

export const WebhookRowContent = ({ sectionWidth, webhook, isChecked, handleToggleEnabled }) => {
  return (
    <StyledRowContent sectionWidth={sectionWidth}>
      <ContentWrapper>
        <FlexWrapper>
          <Text fontWeight={600} fontSize="14px" style={{ marginRight: "8px" }}>
            {webhook.title}
          </Text>
          {webhook.status && <StatusBadge status={webhook.status} />}
        </FlexWrapper>
        <Text fontWeight={600} fontSize="12px" color="#A3A9AE">
          {webhook.url}
        </Text>
      </ContentWrapper>

      <ToggleButtonWrapper>
        <ToggleButton
          className="toggle toggleButton"
          id="toggle id"
          isChecked={isChecked}
          onChange={handleToggleEnabled}
        />
      </ToggleButtonWrapper>
    </StyledRowContent>
  );
};
