import React, { useState } from "react";

import Row from "@docspace/components/row";

import { WebhookRowContent } from "./WebhookRowContent";
import WebhookDialog from "../../WebhookDialog";
import { DeleteWebhookDialog } from "../../DeleteWebhookDialog";

import SettingsIcon from "PUBLIC_DIR/images/catalog.settings.react.svg?url";
import HistoryIcon from "PUBLIC_DIR/images/history.react.svg?url";
import DeleteIcon from "PUBLIC_DIR/images/delete.react.svg?url";

import toastr from "@docspace/components/toast/toastr";

import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

export const WebhookRow = ({
  webhook,
  sectionWidth,
  toggleEnabled,
  deleteWebhook,
  editWebhook,
}) => {
  const navigate = useNavigate();
  const { t } = useTranslation(["Webhooks", "Common"]);

  const [isChecked, setIsChecked] = useState(webhook.enabled);
  const [isSettingsOpened, setIsSettingsOpened] = useState(false);
  const [isDeleteOpened, setIsDeleteOpened] = useState(false);

  const closeSettings = () => setIsSettingsOpened(false);
  const openSettings = () => setIsSettingsOpened(true);
  const onDeleteOpen = () => setIsDeleteOpened(true);
  const onDeleteClose = () => setIsDeleteOpened(false);
  const redirectToHistory = () => navigate(window.location.pathname + `/${webhook.id}`);

  const handleWebhookUpdate = async (webhookInfo) => {
    editWebhook(webhook, webhookInfo);
    toastr.success(t("WebhookEditedSuccessfully"), <b>{t("Common:Done")}</b>);
  };
  const handleWebhookDelete = async () => {
    await deleteWebhook(webhook);
    toastr.success(t("WebhookRemoved"), <b>{t("Common:Done")}</b>);
  };
  const handleToggleEnabled = () => {
    toggleEnabled(webhook);
    setIsChecked((prevIsChecked) => !prevIsChecked);
  };

  const contextOptions = [
    {
      key: "Settings dropdownItem",
      label: t("Common:Settings"),
      icon: SettingsIcon,
      onClick: openSettings,
    },
    {
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
        contextOptions={contextOptions}>
        <WebhookRowContent
          sectionWidth={sectionWidth}
          webhook={webhook}
          isChecked={isChecked}
          handleToggleEnabled={handleToggleEnabled}
        />
      </Row>
      <WebhookDialog
        visible={isSettingsOpened}
        onClose={closeSettings}
        header={t("SettingsWebhook")}
        isSettingsModal={true}
        webhook={webhook}
        onSubmit={handleWebhookUpdate}
      />
      <DeleteWebhookDialog
        visible={isDeleteOpened}
        onClose={onDeleteClose}
        header={t("DeleteWebhookForeverQuestion")}
        handleSubmit={handleWebhookDelete}
      />
    </>
  );
};
