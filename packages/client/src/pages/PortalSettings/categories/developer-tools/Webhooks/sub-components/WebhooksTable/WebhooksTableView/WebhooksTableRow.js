import React, { useState } from "react";
import styled from "styled-components";
import TableRow from "@docspace/components/table-container/TableRow";
import TableCell from "@docspace/components/table-container/TableCell";
import Text from "@docspace/components/text";

import ToggleButton from "@docspace/components/toggle-button";
import SettingsIcon from "PUBLIC_DIR/images/catalog.settings.react.svg?url";
import HistoryIcon from "PUBLIC_DIR/images/history.react.svg?url";
import DeleteIcon from "PUBLIC_DIR/images/delete.react.svg?url";
import StatusBadge from "../../StatusBadge";

import { useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";

const StyledWrapper = styled.div`
  display: contents;
`;

const StyledTableRow = styled(TableRow)`
  .table-container_cell {
    padding-right: 30px;
    text-overflow: ellipsis;
  }

  .mr-8 {
    margin-right: 8px;
  }
  .textOverflow {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }
`;

const WebhooksTableRow = (props) => {
  const {
    webhook,
    toggleEnabled,
    openSettingsModal,
    openDeleteModal,
    setCurrentWebhook,
    hideColumns,
  } = props;
  const navigate = useNavigate();

  const { t } = useTranslation(["Webhooks", "Common"]);

  const [isChecked, setIsChecked] = useState(webhook.enabled);

  const redirectToHistory = () => {
    navigate(window.location.pathname + `/${webhook.id}`);
  };

  const handleRowClick = (e) => {
    if (
      e.target.closest(".checkbox") ||
      e.target.closest(".table-container_row-checkbox") ||
      e.target.closest(".type-combobox") ||
      e.target.closest(".table-container_row-context-menu-wrapper") ||
      e.target.closest(".toggleButton") ||
      e.detail === 0
    ) {
      return;
    }

    redirectToHistory();
  };
  const handleToggleEnabled = () => {
    toggleEnabled(webhook);
    setIsChecked((prevIsChecked) => !prevIsChecked);
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
      key: "Settings dropdownItem",
      label: t("Common:Settings"),
      icon: SettingsIcon,
      onClick: onSettingsOpen,
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
      <StyledWrapper onClick={handleRowClick}>
        <StyledTableRow contextOptions={contextOptions} hideColumns={hideColumns}>
          <TableCell>
            <Text as="span" fontWeight={600} className="mr-8 textOverflow">
              {webhook.name}{" "}
            </Text>
            <StatusBadge status={webhook.status} />
          </TableCell>
          <TableCell>
            <Text
              as="span"
              fontSize="11px"
              color="#A3A9AE"
              fontWeight={600}
              className="textOverflow">
              {webhook.uri}
            </Text>
          </TableCell>
          <TableCell>
            <ToggleButton
              className="toggle toggleButton"
              id="toggle id"
              isChecked={isChecked}
              onChange={handleToggleEnabled}
            />
          </TableCell>
        </StyledTableRow>
      </StyledWrapper>
    </>
  );
};

export default inject(({ webhooksStore }) => {
  const { toggleEnabled, setCurrentWebhook } = webhooksStore;

  return {
    toggleEnabled,
    setCurrentWebhook,
  };
})(observer(WebhooksTableRow));
