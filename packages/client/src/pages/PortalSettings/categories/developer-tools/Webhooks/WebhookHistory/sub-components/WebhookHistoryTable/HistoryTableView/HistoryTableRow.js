import React from "react";
import moment from "moment";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";

import { useNavigate, useParams } from "react-router-dom";

import TableRow from "@docspace/components/table-container/TableRow";
import TableCell from "@docspace/components/table-container/TableCell";
import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import StatusBadge from "../../../../sub-components/StatusBadge";

import toastr from "@docspace/components/toast/toastr";

import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";
import InfoIcon from "PUBLIC_DIR/images/info.outline.react.svg?url";

import { useTranslation } from "react-i18next";

const StyledTableRow = styled(TableRow)`
  .textOverflow {
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  .p-menuitem-icon {
    svg {
      path {
        fill: red;
      }
    }
  }
  .p-menuitem-text {
    color: red;
  }

  ${(props) =>
    props.isHighlight &&
    css`
      .table-container_cell {
        background-color: #f3f4f4;
      }
    `}
`;

const StyledWrapper = styled.div`
  display: contents;
`;

const HistoryTableRow = (props) => {
  const {
    item,
    toggleEventId,
    isIdChecked,
    retryWebhookEvent,
    hideColumns,
    fetchHistoryItems,
    historyFilters,
    formatFilters,
    isRetryPending,
  } = props;
  const { t } = useTranslation(["Webhooks", "Common"]);
  const navigate = useNavigate();
  const { id } = useParams();

  const redirectToDetails = () =>
    navigate(window.location.pathname + `/${item.id}`);
  const handleRetryEvent = async () => {
    if (isRetryPending) {
      return;
    }
    await retryWebhookEvent(item.id);
    await fetchHistoryItems({
      ...(historyFilters ? formatFilters(historyFilters) : {}),
      configId: id,
    });
    toastr.success(t("WebhookRedilivered"), <b>{t("Common:Done")}</b>);
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

  const formattedDelivery =
    moment(item.delivery).format("MMM D, YYYY, h:mm:ss A") + " UTC";

  const onChange = (e) => {
    if (
      e.target.closest(".checkbox") ||
      e.target.closest(".table-container_row-checkbox") ||
      e.target.closest(".type-combobox") ||
      e.target.closest(".table-container_row-context-menu-wrapper") ||
      e.detail === 0
    ) {
      return;
    }
    toggleEventId(item.id);
  };

  const isChecked = isIdChecked(item.id);

  return (
    <StyledWrapper
      className={isChecked ? "selected-table-row" : ""}
      onClick={onChange}
    >
      <StyledTableRow
        contextOptions={contextOptions}
        checked={isChecked}
        hideColumns={hideColumns}
      >
        <TableCell>
          <TableCell checked={isChecked} className="checkboxWrapper">
            <Checkbox
              className="checkbox"
              onChange={onChange}
              isChecked={isChecked}
            />
          </TableCell>

          <Text fontWeight={600}>{item.id}</Text>
        </TableCell>
        <TableCell>
          <StatusBadge status={item.status} />
        </TableCell>
        <TableCell>
          <Text fontWeight={600} fontSize="11px" className="textOverflow">
            {formattedDelivery}
          </Text>
        </TableCell>
      </StyledTableRow>
    </StyledWrapper>
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
})(observer(HistoryTableRow));
