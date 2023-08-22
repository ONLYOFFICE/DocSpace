import React from "react";
import styled, { css } from "styled-components";
import Textarea from "@docspace/components/textarea";
import Button from "@docspace/components/button";
import Text from "@docspace/components/text";
import { inject, observer } from "mobx-react";

import json_beautifier from "csvjson-json_beautifier";
import { isMobileOnly } from "react-device-detect";
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

const LargePayloadStub = styled.div`
  box-sizing: border-box;

  display: flex;
  justify-content: space-between;
  align-items: center;

  width: 100%;
  max-width: 1200px;
  padding: 12px 10px;
  margin-top: 4px;

  background: #f8f9f9;
  border: 1px solid #eceef1;
  border-radius: 3px;

  ${isMobileOnly &&
  css`
    justify-content: flex-start;
    flex-wrap: wrap;
    row-gap: 16px;
  `}
`;

function isJSON(jsonString) {
  try {
    const parsedJson = JSON.parse(jsonString);
    return parsedJson && typeof parsedJson === "object";
  } catch (e) {}

  return false;
}

const ResponseDetails = ({ eventDetails }) => {
  const responsePayload = eventDetails.responsePayload?.trim();
  const { t } = useTranslation(["Webhooks"]);

  const beautifiedJSON = isJSON(responsePayload)
    ? json_beautifier(JSON.parse(responsePayload), {
        inlineShortArrays: true,
      })
    : responsePayload;

  const numberOfLines = isJSON(responsePayload)
    ? beautifiedJSON.split("\n").length
    : responsePayload.split("\n").length;

  const openRawPayload = () => {
    const rawPayload = window.open("");
    isJSON(responsePayload)
      ? rawPayload.document.write(
          beautifiedJSON.replace(/(?:\r\n|\r|\n)/g, "<br/>")
        )
      : rawPayload.document.write(responsePayload);
    rawPayload.focus();
  };

  return (
    <DetailsWrapper>
      <Text as="h3" fontWeight={600} className="mb-4 mt-7">
        {t("ResponsePostHeader")}
      </Text>
      {isJSON(eventDetails.responseHeaders) ? (
        <Textarea
          classNameCopyIcon="response-header-copy"
          value={eventDetails.responseHeaders}
          enableCopy
          hasNumeration
          isFullHeight
          isJSONField
          copyInfoText={t("ResponseHeaderCopied")}
        />
      ) : (
        <Textarea
          value={eventDetails.responseHeaders}
          heightScale
          className="textareaBody"
        />
      )}
      <Text as="h3" fontWeight={600} className="mb-4 mt-16">
        {t("ResponsePostBody")}
      </Text>
      {responsePayload.length > 4000 || numberOfLines > 100 ? (
        <LargePayloadStub>
          <Text fontWeight={600} color="#657077">
            {t("PayloadIsTooLarge")}
          </Text>
          <Button
            className="view-raw-payload"
            size="small"
            onClick={openRawPayload}
            label={t("ViewRawPayload")}
            scale={isMobileOnly}
          />
        </LargePayloadStub>
      ) : responsePayload === "" ? (
        <Textarea isDisabled />
      ) : isJSON(responsePayload) ? (
        <Textarea
          classNameCopyIcon="response-body-copy"
          value={responsePayload}
          isJSONField
          enableCopy
          hasNumeration
          isFullHeight
          copyInfoText={t("ResponseBodyCopied")}
        />
      ) : (
        <Textarea
          classNameCopyIcon="response-body-copy"
          value={responsePayload}
          enableCopy
          heightScale
          className="textareaBody"
          copyInfoText={t("ResponseBodyCopied")}
        />
      )}
    </DetailsWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const { eventDetails } = webhooksStore;

  return { eventDetails };
})(observer(ResponseDetails));
