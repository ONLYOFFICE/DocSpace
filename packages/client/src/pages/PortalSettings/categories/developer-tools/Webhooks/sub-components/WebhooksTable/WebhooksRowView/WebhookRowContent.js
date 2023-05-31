import React from "react";
import styled from "styled-components";

import Text from "@docspace/components/text";
import RowContent from "@docspace/components/row-content";
import ToggleButton from "@docspace/components/toggle-button";

import StatusBadge from "../../StatusBadge";

import { isMobileOnly } from "react-device-detect";

const StyledRowContent = styled(RowContent)`
  display: flex;
  padding-bottom: 10px;

  .rowMainContainer {
    height: 100%;
    width: 100%;
  }

  .mainIcons {
    min-width: 76px;
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
  margin-left: -52px;
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
            {webhook.name}
          </Text>
          <StatusBadge status={webhook.status} />
        </FlexWrapper>

        {!isMobileOnly && (
          <Text fontWeight={600} fontSize="12px" color="#A3A9AE">
            {webhook.uri}
          </Text>
        )}
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
