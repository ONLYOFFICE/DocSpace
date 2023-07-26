import React from "react";
import { inject, observer } from "mobx-react";

import { useNavigate, useParams } from "react-router-dom";

import Row from "@docspace/components/row";
import { HistoryRowContent } from "./HistoryRowContent";

import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";
import InfoIcon from "PUBLIC_DIR/images/info.outline.react.svg?url";

import toastr from "@docspace/components/toast/toastr";

import { useTranslation } from "react-i18next";

const HistoryRow = (props) => {
  const {
    historyItem,
    sectionWidth,
    toggleEventId,
    isIdChecked,
    retryWebhookEvent,
    fetchHistoryItems,
    historyFilters,
    formatFilters,
    isRetryPending,
  } = props;
  const { t } = useTranslation(["Webhooks", "Common"]);
  const navigate = useNavigate();
  const { id } = useParams();

  const redirectToDetails = () =>
    navigate(window.location.pathname + `/${historyItem.id}`);
  const handleRetryEvent = async () => {
    await retryWebhookEvent(historyItem.id);
    await fetchHistoryItems({
      ...(historyFilters ? formatFilters(historyFilters) : {}),
      configId: id,
    });
    toastr.success(t("WebhookRedilivered"), <b>{t("Common:Done")}</b>);
  };
  const handleOnSelect = () => toggleEventId(historyItem.id);
  const handleRowClick = (e) => {
    if (
      e.target.closest(".checkbox") ||
      e.target.closest(".table-container_row-checkbox") ||
      e.target.closest(".type-combobox") ||
      e.target.closest(".table-container_row-context-menu-wrapper") ||
      e.target.closest(".row_context-menu-wrapper") ||
      e.detail === 0
    ) {
      return;
    }
    toggleEventId(historyItem.id);
  };

  const contextOptions = [
    {
      id: "webhook-details",
      key: "Webhook details dropdownItem",
      label: t("WebhookDetails"),
      icon: InfoIcon,
      onClick: redirectToDetails,
    },
    {
      id: "retry",
      key: "Retry dropdownItem",
      label: t("Retry"),
      icon: RetryIcon,
      onClick: handleRetryEvent,
      disabled: isRetryPending,
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
      className={
        isIdChecked(historyItem.id) ? "row-item selected-row-item" : "row-item "
      }
      onClick={handleRowClick}
    >
      <HistoryRowContent
        sectionWidth={sectionWidth}
        historyItem={historyItem}
      />
    </Row>
  );
};

export default inject(({ webhooksStore }) => {
  const {
    toggleEventId,
    isIdChecked,
    retryWebhookEvent,
    fetchHistoryItems,
    historyFilters,
    formatFilters,
    isRetryPending,
  } = webhooksStore;

  return {
    toggleEventId,
    isIdChecked,
    retryWebhookEvent,
    fetchHistoryItems,
    historyFilters,
    formatFilters,
    isRetryPending,
  };
})(observer(HistoryRow));
