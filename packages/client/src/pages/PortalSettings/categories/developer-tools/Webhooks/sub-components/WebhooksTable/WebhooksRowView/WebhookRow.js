import React, { useState } from "react";
import { inject, observer } from "mobx-react";

import Row from "@docspace/components/row";

import { WebhookRowContent } from "./WebhookRowContent";

import SettingsIcon from "PUBLIC_DIR/images/catalog.settings.react.svg?url";
import HistoryIcon from "PUBLIC_DIR/images/history.react.svg?url";
import DeleteIcon from "PUBLIC_DIR/images/delete.react.svg?url";

import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

export const WebhookRow = (props) => {
  const {
    webhook,
    sectionWidth,
    toggleEnabled,
    openSettingsModal,
    openDeleteModal,
    setCurrentWebhook,
  } = props;
  const navigate = useNavigate();
  const { t } = useTranslation(["Webhooks", "Common"]);

  const [isChecked, setIsChecked] = useState(webhook.enabled);

  const handleToggleEnabled = () => {
    toggleEnabled(webhook);
    setIsChecked((prevIsChecked) => !prevIsChecked);
  };

  const redirectToHistory = () => {
    navigate(window.location.pathname + `/${webhook.id}`);
  };
  const handleRowClick = (e) => {
    if (
      e.target.closest(".table-container_row-context-menu-wrapper") ||
      e.target.closest(".toggleButton") ||
      e.target.closest(".row_context-menu-wrapper") ||
      e.detail === 0
    ) {
      return;
    }

    redirectToHistory();
  };

  const onSettingsOpen = () => {
    setCurrentWebhook(webhook);
    openSettingsModal();
  };
  const onDeleteOpen = () => {
    setCurrentWebhook(webhook);
    openDeleteModal();
  };

  const contextOptions = [
    {
      id: "settings",
      key: "Settings dropdownItem",
      label: t("Common:Settings"),
      icon: SettingsIcon,
      onClick: onSettingsOpen,
    },
    {
      id: "webhook-history",
      key: "Webhook history dropdownItem",
      label: t("WebhookHistory"),
      icon: HistoryIcon,
      onClick: redirectToHistory,
    },
    {
      key: "Separator dropdownItem",
      isSeparator: true,
    },
    {
      id: "delete-webhook",
      key: "Delete webhook dropdownItem",
      label: t("DeleteWebhook"),
      icon: DeleteIcon,
      onClick: onDeleteOpen,
    },
  ];

  return (
    <>
      <Row
        sectionWidth={sectionWidth}
        key={webhook.id}
        data={webhook}
        contextOptions={contextOptions}
        onClick={handleRowClick}
      >
        <WebhookRowContent
          sectionWidth={sectionWidth}
          webhook={webhook}
          isChecked={isChecked}
          handleToggleEnabled={handleToggleEnabled}
        />
      </Row>
    </>
  );
};

export default inject(({ webhooksStore }) => {
  const { toggleEnabled, deleteWebhook, editWebhook, setCurrentWebhook } =
    webhooksStore;

  return { toggleEnabled, deleteWebhook, editWebhook, setCurrentWebhook };
})(observer(WebhookRow));
