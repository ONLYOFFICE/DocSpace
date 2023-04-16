import React from "react";
import styled from "styled-components";
import { Text, Textarea } from "@docspace/components";

const DetailsWrapper = styled.div`
  width: 100%;
`;

export const ResponseDetails = ({ webhookDetails }) => {
  const responsePayload = webhookDetails.responsePayload.trim();

  return (
    <DetailsWrapper>
      <Text as="h3" fontWeight={600} style={{ marginBottom: "4px" }}>
        Response post header
      </Text>
      <Textarea
        value={webhookDetails.responseHeaders}
        enableCopy
        hasNumeration
        isFullHeight
        isJSONField
      />
      <Text as="h3" fontWeight={600} style={{ marginBottom: "4px", marginTop: "16px" }}>
        Response post body
      </Text>
      <Textarea value={responsePayload} isJSONField enableCopy hasNumeration isFullHeight />
    </DetailsWrapper>
  );
};
