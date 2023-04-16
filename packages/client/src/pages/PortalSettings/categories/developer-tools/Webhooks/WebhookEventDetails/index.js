import React, { useState, useEffect } from "react";
import styled from "styled-components";

import { useParams } from "react-router-dom";
import { inject, observer } from "mobx-react";

import { Text } from "@docspace/components";

import { DetailsBar } from "./sub-components/DetailsBar";
import DetailsNavigationHeader from "./sub-components/DetailsNavigationHeader";
import { MessagesDetails } from "./sub-components/MessagesDetails";
import { WebhookDetailsLoader } from "../sub-components/Loaders";

const DetailsWrapper = styled.div`
  width: 100%;
`;

const EventDetailsHeader = styled.header`
  padding: 20px 0;
`;

const WebhookEventDetails = (props) => {
  const { getWebhookHistory } = props;

  const { id, eventId } = useParams();

  const [isLoading, setIsLoading] = useState(true);

  const [webhookDetails, setWebhookDetails] = useState({});

  useEffect(() => {
    (async () => {
      const [webhookDetailsData] = await getWebhookHistory({ eventId });
      setWebhookDetails(webhookDetailsData);
      console.log(webhookDetailsData);
      setIsLoading(false);
    })();
  }, []);

  return (
    <DetailsWrapper>
      <DetailsNavigationHeader />
      {isLoading ? (
        <WebhookDetailsLoader />
      ) : (
        <main>
          <EventDetailsHeader>
            <Text fontWeight={600}>Webhook {id}</Text>
            <DetailsBar webhookDetails={webhookDetails} />
          </EventDetailsHeader>
          <MessagesDetails webhookDetails={webhookDetails} />
        </main>
      )}
    </DetailsWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const { getWebhookHistory } = webhooksStore;

  return { getWebhookHistory };
})(observer(WebhookEventDetails));
