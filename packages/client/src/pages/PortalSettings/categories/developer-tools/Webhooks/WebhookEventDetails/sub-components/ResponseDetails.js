import React from "react";
import styled from "styled-components";
import { Text, Textarea } from "@docspace/components";

import DangerIcon from "PUBLIC_DIR/images/danger.toast.react.svg?url";

const DetailsWrapper = styled.div`
  width: 100%;
`;

const ErrorMessageTooltip = styled.div`
  width: 100%;
  padding: 8px 12px;
  background: #f7cdbe;

  box-shadow: 0px 5px 20px rgba(4, 15, 27, 0.07);
  border-radius: 6px;
  display: flex;
  align-items: center;
`;

export const ResponseDetails = ({ webhookDetails }) => {
  if (
    !(
      webhookDetails.hasOwnProperty("responseHeaders") ||
      webhookDetails.hasOwnProperty("responsePayload")
    )
  ) {
    return (
      <ErrorMessageTooltip>
        <img src={DangerIcon} alt="danger icon" style={{ marginRight: "8px" }} />
        The SSL connection could not be established, see inner exception.
      </ErrorMessageTooltip>
    );
  }

  const responsePayload = webhookDetails.responsePayload?.trim();

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
      {responsePayload === "" ? (
        <Textarea isDisabled />
      ) : (
        <Textarea value={responsePayload} isJSONField enableCopy hasNumeration isFullHeight />
      )}
    </DetailsWrapper>
  );
};
