import React, { useState, useEffect } from "react";
import styled from "styled-components";

import { useParams } from "react-router-dom";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";

import { DetailsBar } from "./sub-components/DetailsBar";
import { MessagesDetails } from "./sub-components/MessagesDetails";
import { WebhookDetailsLoader } from "../sub-components/Loaders";

const DetailsWrapper = styled.div`
  width: 100%;
`;

const EventDetailsHeader = styled.header`
  padding: 20px 0;
`;

const WebhookEventDetails = (props) => {
  const { getEvent, setTitleDetails, setTitleDefault } = props;

  const { id, eventId } = useParams();

  const [isLoading, setIsLoading] = useState(false);

  const [webhookDetails, setWebhookDetails] = useState({});

  useEffect(() => {
    setTitleDetails();
    (async () => {
      const timer = setTimeout(() => {
        webhookDetails.status === undefined && setIsLoading(true);
      }, 300);
      const webhookDetailsData = await getEvent(eventId);
      setWebhookDetails(webhookDetailsData);
      clearTimeout(timer);
      setIsLoading(false);
    })();
    return setTitleDefault;
  }, []);

  return (
    <DetailsWrapper>
      {isLoading && <WebhookDetailsLoader />}
      {webhookDetails.status !== undefined && (
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
  const { getEvent, setTitleDetails, setTitleDefault } = webhooksStore;

  return { getEvent, setTitleDetails, setTitleDefault };
})(observer(WebhookEventDetails));
