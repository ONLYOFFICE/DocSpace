import React, { useEffect, useTransition, Suspense } from "react";
import styled from "styled-components";

import { useParams } from "react-router-dom";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";

import DetailsBar from "./sub-components/DetailsBar";
import MessagesDetails from "./sub-components/MessagesDetails";
import { WebhookDetailsLoader } from "../sub-components/Loaders";

const DetailsWrapper = styled.div`
  width: 100%;
`;

const EventDetailsHeader = styled.header`
  padding: 20px 0;
`;

const WebhookEventDetails = (props) => {
  const { fetchEventData } = props;
  const { id, eventId } = useParams();

  const [isPending, startTransition] = useTransition();

  useEffect(() => {
    startTransition(() => {
      fetchEventData(eventId);
    });
  }, []);

  return (
    <Suspense fallback={WebhookDetailsLoader}>
      <DetailsWrapper>
        <main>
          <EventDetailsHeader>
            <Text fontWeight={600}>Webhook {id}</Text>
            <DetailsBar />
          </EventDetailsHeader>
          <MessagesDetails />
        </main>
      </DetailsWrapper>
    </Suspense>
  );
};

export default inject(({ webhooksStore }) => {
  const { fetchEventData } = webhooksStore;

  return { fetchEventData };
})(observer(WebhookEventDetails));
