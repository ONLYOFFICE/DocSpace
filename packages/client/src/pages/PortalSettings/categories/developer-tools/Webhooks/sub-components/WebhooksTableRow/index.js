import React, { useState } from "react";
import TableRow from "@docspace/components/table-container/TableRow";
import TableCell from "@docspace/components/table-container/TableCell";
import styled from "styled-components";
import { FailBadge } from "../FailBadge";
import { SuccessBadge } from "../SuccessBadge";
import { ToggleButton } from "@docspace/components";
import ContextMenuButton from "@docspace/components/context-menu-button";
import SettingsIcon from "PUBLIC_DIR/images/settings.webhooks.react.svg?url";
import HistoryIcon from "PUBLIC_DIR/images/history.react.svg?url";
import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";
import DeleteIcon from "PUBLIC_DIR/images/delete.react.svg?url";
import { WebhookDialog } from "../WebhookDialog";
import { DeleteWebhookDialog } from "../DeleteWebhookDialog";

export const WebhooksTableRow = ({ webhook, toggleEnabled, deleteWebhook, editWebhook }) => {
  const [isChecked, setIsChecked] = useState(webhook.isEnabled);
  const [isOpen, setIsOpen] = useState(false);
  const [isSettingsOpened, setIsSettingsOpened] = useState(false);
  const [isDeleteOpened, setIsDeleteOpened] = useState(false);

  const closeSettings = () => setIsSettingsOpened(false);
  const openSettings = () => setIsSettingsOpened(true);
  const onDeleteClose = () => setIsDeleteOpened(false);
  const toggleContextMenu = () => setIsOpen((prevIsOpen) => !prevIsOpen);

  const handleWebhookUpdate = (webhookInfo) => editWebhook(webhook, webhookInfo);
  const handleWebhookDelete = () => deleteWebhook(webhook);
  const handleToggleEnabled = () => {
    toggleEnabled(webhook);
    setIsChecked((prevIsChecked) => !prevIsChecked);
  };

  const getDropdownItems = () => {
    return [
      {
        key: "Settings dropdownItem",
        label: "Settings",
        icon: SettingsIcon,
        onClick: openSettings,
      },
      {
        key: "Webhook history dropdownItem",
        label: "Webhook history",
        icon: HistoryIcon,
        onClick: () => console.log("webhooks history"),
      },
      {
        key: "Retry dropdownItem",
        label: "Retry",
        icon: RetryIcon,
        onClick: () => console.log("retry"),
      },
      {
        key: "Separator dropdownItem",
        isSeparator: true,
      },
      {
        key: "Delete webhook dropdownItem",
        label: "Delete webhook",
        icon: DeleteIcon,
        onClick: () => setIsDeleteOpened(true),
      },
    ];
  };

  return (
    <>
      <TableRow>
        <TableCell>
          {webhook.title}{" "}
          {webhook.responseStatus === "success" ? (
            <SuccessBadge label={webhook.responseCode} />
          ) : webhook.responseStatus === "error" ? (
            <FailBadge label={webhook.responseCode} />
          ) : (
            ""
          )}
        </TableCell>
        <TableCell>{webhook.url}</TableCell>
        <TableCell>
          <ToggleButton
            className="toggle toggleButton"
            id="toggle id"
            isChecked={isChecked}
            onChange={handleToggleEnabled}
          />
        </TableCell>

        <TableCell>
          <ContextMenuButton
            directionX="right"
            getData={getDropdownItems}
            opened={isOpen}
            onClick={toggleContextMenu}
            title="Actions"
          />
        </TableCell>
      </TableRow>
      <WebhookDialog
        visible={isSettingsOpened}
        onClose={closeSettings}
        header="Setting webhook"
        isSettingsModal={true}
        webhook={webhook}
        onSubmit={handleWebhookUpdate}
      />
      <DeleteWebhookDialog
        visible={isDeleteOpened}
        onClose={onDeleteClose}
        header="Delete Webhook forever?"
        handleSubmit={handleWebhookDelete}
      />
    </>
  );
};
