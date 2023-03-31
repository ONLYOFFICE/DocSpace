import React from "react";
import styled from "styled-components";

import { useParams } from "react-router-dom";

import { Text } from "@docspace/components";

import { DetailsBar } from "./sub-components/DetailsBar";
import DetailsNavigationHeader from "./sub-components/DetailsNavigationHeader";
import { MessagesDetails } from "./sub-components/MessagesDetails";

const DetailsWrapper = styled.div`
  width: 100%;
`;

const EventDetailsHeader = styled.header`
  padding: 20px 0;
`;

const WebhookEventDetails = () => {
  const { id } = useParams();

  return (
    <DetailsWrapper>
      <DetailsNavigationHeader />
      <main>
        <EventDetailsHeader>
          <Text fontWeight={600}>Webhook {id}</Text>
          <DetailsBar />
        </EventDetailsHeader>
        <MessagesDetails />
      </main>
    </DetailsWrapper>
  );
};

export default WebhookEventDetails;
