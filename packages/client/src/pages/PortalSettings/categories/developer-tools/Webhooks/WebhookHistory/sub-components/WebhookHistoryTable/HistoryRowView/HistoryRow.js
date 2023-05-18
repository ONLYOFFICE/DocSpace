import React from "react";
import { inject, observer } from "mobx-react";

import { useNavigate } from "react-router-dom";

import Row from "@docspace/components/row";
import { HistoryRowContent } from "./HistoryRowContent";

import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";
import InfoIcon from "PUBLIC_DIR/images/info.outline.react.svg?url";

import toastr from "@docspace/components/toast/toastr";

import { useTranslation } from "react-i18next";

const HistoryRow = (props) => {
  const { historyItem, sectionWidth, toggleEventId, isIdChecked, retryWebhookEvent } = props;
  const { t } = useTranslation(["Webhooks", "Common"]);
  const navigate = useNavigate();

  const redirectToDetails = () => navigate(window.location.pathname + `/${historyItem.id}`);
  const handleRetryEvent = async () => {
    await retryWebhookEvent(historyItem.id);
    toastr.success(t("WebhookRedilivered"), <b>{t("Common:Done")}</b>);
  };
  const handleOnSelect = () => toggleEventId(historyItem.id);

  const contextOptions = [
    {
      key: "Webhook details dropdownItem",
      label: t("WebhookDetails"),
      icon: InfoIcon,
      onClick: redirectToDetails,
    },
    {
      key: "Retry dropdownItem",
      label: t("Retry"),
      icon: RetryIcon,
      onClick: handleRetryEvent,
    },
  ];

  return (
    <Row
      sectionWidth={sectionWidth}
      key={historyItem.id}
      contextOptions={contextOptions}
      checkbox
      checked={isIdChecked(historyItem.id)}
      onSelect={handleOnSelect}
      className={isIdChecked(historyItem.id) ? "selected-row-item" : ""}
      onClick={handleOnSelect}>
      <HistoryRowContent sectionWidth={sectionWidth} historyItem={historyItem} />
    </Row>
  );
};

export default inject(({ webhooksStore }) => {
  const { toggleEventId, isIdChecked, retryWebhookEvent } = webhooksStore;

  return { toggleEventId, isIdChecked, retryWebhookEvent };
})(observer(HistoryRow));
