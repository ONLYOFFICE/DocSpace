import React from "react";
import styled from "styled-components";

import { useParams } from "react-router-dom";

import { Text } from "@docspace/components";
import { StatusBadge } from "../../sub-components/StatusBadge";

const BarWrapper = styled.div`
  width: 100%;
  max-width: 1200px;
  min-width: 600px;
  display: flex;
  align-items: center;
  justify-content: space-between;

  margin-top: 24px;

  background: #f8f9f9;
  border-radius: 3px;
`;

const BarItem = styled.div`
  box-sizing: border-box;
  width: 257.25px;
  height: 76px;
  padding: 16px;
`;

const BarItemHeader = ({ children }) => (
  <Text as="h3" color="#A3A9AE" fontSize="12px" fontWeight={600} style={{ marginBottom: "8px" }}>
    {children}
  </Text>
);

const FlexWrapper = styled.div`
  display: flex;
  align-items: center;
`;

export const DetailsBar = () => {
  const { eventId } = useParams();

  return (
    <BarWrapper>
      <BarItem>
        <BarItemHeader>Status</BarItemHeader>
        <FlexWrapper>
          <StatusBadge status={"404"} />
          <Text isInline fontWeight={600}>
            {" "}
            Client Error - Not Found
          </Text>
        </FlexWrapper>
      </BarItem>
      <BarItem>
        <BarItemHeader>Event ID</BarItemHeader>
        <Text isInline fontWeight={600}>
          {eventId}
        </Text>
      </BarItem>
      <BarItem>
        <BarItemHeader>Event time</BarItemHeader>
        <Text isInline fontWeight={600}>
          Nov 15, 2022, 11:10:00 PM
        </Text>
      </BarItem>
      <BarItem>
        <BarItemHeader>Delivery time</BarItemHeader>
        <Text isInline fontWeight={600}>
          Nov 15, 2022, 11:10:00 PM
        </Text>
      </BarItem>
    </BarWrapper>
  );
};
