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

export const WebhooksListRow = ({ webhook, index, setWebhooks }) => {
  const [isChecked, setIsChecked] = useState(webhook.isEnabled);

  const [isOpen, setIsOpen] = useState(false);

  const [isSettings, setIsSettings] = useState(false);
  const [isDelete, setIsDelete] = useState(false);
  const onSettingsClose = () => {
    setIsSettings(false);
  };
  const onDeleteClose = () => {
    setIsDelete(false);
  };

  const onClickHandler = () => {
    setIsOpen((prevIsOpen) => !prevIsOpen);
  };

  const getData = () => {
    return [
      {
        key: "key1",
        label: "Settings",
        icon: SettingsIcon,
        onClick: () => setIsSettings(true),
      },
      {
        key: "key2",
        label: "Webhook history",
        icon: HistoryIcon,
        onClick: () => console.log("Button 1 clicked"),
      },
      {
        key: "key3",
        label: "Retry",
        icon: RetryIcon,
        onClick: () => console.log("Button 1 clicked"),
      },
      {
        key: "key4",
        isSeparator: true,
        onClick: () => console.log("label2"),
      },
      {
        key: "key5",
        label: "Delete webhook",
        icon: DeleteIcon,
        onClick: () => setIsDelete(true),
      },
    ];
  };

  return (
    <>
      <TableRow>
        <TableCell>
          {webhook.title}{" "}
          {webhook.responseStatus === "success" ? (
            <SuccessBadge label={webhook.badgeTitle} />
          ) : webhook.responseStatus === "error" ? (
            <FailBadge label={webhook.badgeTitle} />
          ) : (
            <span></span>
          )}
        </TableCell>
        <TableCell>{webhook.url}</TableCell>
        <TableCell>
          <ToggleButton
            className="toggle toggleButton"
            id="toggle id"
            isChecked={isChecked}
            onChange={() => {
              setWebhooks((prevWebhooks) => {
                prevWebhooks[index].isEnabled = !prevWebhooks[index].isEnabled;
                return prevWebhooks;
              });
              setIsChecked((prevIsChecked) => !prevIsChecked);
            }}
          />
        </TableCell>

        <TableCell>
          <ContextMenuButton
            directionX="right"
            getData={getData}
            isDisabled={false}
            opened={isOpen}
            onClick={onClickHandler}
            title="Actions"
          />
        </TableCell>
      </TableRow>
      <WebhookDialog
        visible={isSettings}
        onClose={onSettingsClose}
        header="Setting webhook"
        isSettingsModal={true}
        webhook={webhook}
        onSubmit={(webhookInfo) => {
          setWebhooks((prevWebhooks) =>
            prevWebhooks.map((prevWebhook) =>
              prevWebhook.url === webhook.url ? webhookInfo : prevWebhook,
            ),
          );
        }}
      />
      <DeleteWebhookDialog
        visible={isDelete}
        onClose={onDeleteClose}
        header="Delete Webhook forever?"
        handleSubmit={() => {
          setWebhooks((prevWebhooks) =>
            prevWebhooks.filter((webhookInfo) => webhookInfo.url !== webhook.url),
          );
        }}
      />
    </>
  );
};
