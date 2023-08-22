import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import Textarea from "@docspace/components/textarea";
import { inject, observer } from "mobx-react";

import DangerIcon from "PUBLIC_DIR/images/danger.toast.react.svg?url";
import { useTranslation } from "react-i18next";

const DetailsWrapper = styled.div`
  width: 100%;

  .textareaBody {
    height: 50vh !important;
  }

  .mt-7 {
    margin-top: 7px;
  }

  .mt-16 {
    margin-top: 16px;
  }

  .mb-4 {
    margin-bottom: 4px;
  }
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

  color: black;

  img {
    margin-right: 8px;
  }
`;

function isJSON(jsonString) {
  try {
    const parsedJson = JSON.parse(jsonString);
    return parsedJson && typeof parsedJson === "object";
  } catch (e) {}

  return false;
}

const RequestDetails = ({ eventDetails }) => {
  const { t } = useTranslation(["Webhooks"]);

  return (
    <DetailsWrapper>
      {eventDetails.status === 0 && (
        <ErrorMessageTooltip>
          <img src={DangerIcon} alt="danger icon" />
          {t("FailedToConnect")}
        </ErrorMessageTooltip>
      )}
      <Text as="h3" fontWeight={600} className="mb-4 mt-7">
        {t("RequestPostHeader")}
      </Text>
      {!eventDetails.requestHeaders ? (
        <Textarea isDisabled />
      ) : (
        <Textarea
          classNameCopyIcon="request-header-copy"
          value={eventDetails.requestHeaders}
          enableCopy
          hasNumeration
          isFullHeight
          isJSONField
          copyInfoText={t("RequestHeaderCopied")}
        />
      )}

      <Text as="h3" fontWeight={600} className="mb-4 mt-16">
        {t("RequestPostBody")}
      </Text>
      {isJSON(eventDetails.requestPayload) ? (
        <Textarea
          classNameCopyIcon="request-body-copy"
          value={eventDetails.requestPayload}
          isJSONField
          enableCopy
          hasNumeration
          isFullHeight
          copyInfoText={t("RequestBodyCopied")}
        />
      ) : (
        <Textarea
          value={eventDetails.requestPayload}
          heightScale
          className="textareaBody"
        />
      )}
    </DetailsWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const { eventDetails } = webhooksStore;

  return { eventDetails };
})(observer(RequestDetails));
