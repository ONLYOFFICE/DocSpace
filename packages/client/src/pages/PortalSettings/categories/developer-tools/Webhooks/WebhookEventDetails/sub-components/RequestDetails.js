import React from "react";
import styled from "styled-components";
import { Text, Textarea } from "@docspace/components";

import DangerIcon from "PUBLIC_DIR/images/danger.toast.react.svg?url";
import { useTranslation } from "react-i18next";

const DetailsWrapper = styled.div`
  width: 100%;
`;
const ErrorMessageTooltip = styled.div`
  box-sizing: border-box;

  width: 100%;
  max-width: 1200px;
  padding: 8px 12px;
  background: #f7cdbe;

  box-shadow: 0px 5px 20px rgba(4, 15, 27, 0.07);
  border-radius: 6px;
  display: flex;
  align-items: center;

  margin-bottom: 16px;
`;

export const RequestDetails = ({ webhookDetails }) => {
  const { t } = useTranslation(["Webhooks"]);
  return (
    <DetailsWrapper>
      {webhookDetails.status === 0 && (
        <ErrorMessageTooltip>
          <img src={DangerIcon} alt="danger icon" style={{ marginRight: "8px" }} />
          {t("FailedToConnect", { ns: "Webhooks" })}
        </ErrorMessageTooltip>
      )}
      <Text as="h3" fontWeight={600} style={{ marginBottom: "4px" }}>
        {t("RequestPostHeader", { ns: "Webhooks" })}
      </Text>
      {!webhookDetails.requestHeaders ? (
        <Textarea isDisabled />
      ) : (
        <Textarea
          value={webhookDetails.requestHeaders}
          enableCopy
          hasNumeration
          isFullHeight
          isJSONField
          copyInfoText={t("RequestHeaderCopied", { ns: "Webhooks" })}
        />
      )}

      <Text as="h3" fontWeight={600} style={{ marginBottom: "4px", marginTop: "16px" }}>
        {t("RequestPostBody", { ns: "Webhooks" })}
      </Text>
      <Textarea
        value={webhookDetails.requestPayload}
        isJSONField
        enableCopy
        hasNumeration
        isFullHeight
        copyInfoText={t("RequestBodyCopied", { ns: "Webhooks" })}
      />
    </DetailsWrapper>
  );
};
