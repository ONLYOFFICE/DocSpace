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
  const { getEvent, hideTitle, showTitle } = props;

  const { id, eventId } = useParams();

  const [isLoading, setIsLoading] = useState(true);

  const [webhookDetails, setWebhookDetails] = useState({});

  useEffect(() => {
    hideTitle();
    (async () => {
      const webhookDetailsData = await getEvent(eventId);
      setWebhookDetails(webhookDetailsData);
      setIsLoading(false);
    })();
    return showTitle;
  }, []);

  return (
    <DetailsWrapper>
      <DetailsNavigationHeader eventId={eventId} />
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
  const { getEvent, hideTitle, showTitle } = webhooksStore;

  return { getEvent, hideTitle, showTitle };
})(observer(WebhookEventDetails));
