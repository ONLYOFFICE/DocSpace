import React, { useMemo } from "react";
import moment from "moment";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";

import { useNavigate } from "react-router-dom";

import TableRow from "@docspace/components/table-container/TableRow";
import TableCell from "@docspace/components/table-container/TableCell";
import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import { StatusBadge } from "../../../../sub-components/StatusBadge";

import toastr from "@docspace/components/toast/toastr";

import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";
import InfoIcon from "PUBLIC_DIR/images/info.outline.react.svg?url";

import { useTranslation } from "react-i18next";

const StyledTableRow = styled(TableRow)`
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
  const { item, toggleEventId, isIdChecked, retryWebhookEvent } = props;
  const { t } = useTranslation(["Webhooks", "Common"]);
  const navigate = useNavigate();

  const redirectToDetails = () => navigate(window.location.pathname + `/${item.id}`);
  const handleRetryEvent = async () => {
    await retryWebhookEvent(item.id);
    toastr.success(t("WebhookRedilivered"), <b>{t("Common:Done")}</b>);
  };

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

  const formattedDelivery = useMemo(
    () => moment(item.delivery).format("MMM D, YYYY, h:mm:ss A") + " UTC",
    [item],
  );

  const onChange = () => {
    toggleEventId(item.id);
  };

  const isChecked = isIdChecked(item.id);

  return (
    <StyledWrapper className={isChecked ? "selected-table-row" : ""} onClick={onChange}>
      <StyledTableRow contextOptions={contextOptions} checked={isChecked}>
        <TableCell>
          <TableCell checked={isChecked} className="noPadding">
            <Checkbox onChange={onChange} isChecked={isChecked} />
          </TableCell>

          <Text fontWeight={600}>{item.id}</Text>
        </TableCell>
        <TableCell>
          <StatusBadge status={item.status} />
        </TableCell>
        <TableCell>
          <Text fontWeight={600} fontSize="11px">
            {formattedDelivery}
          </Text>
        </TableCell>
      </StyledTableRow>
    </StyledWrapper>
  );
};

export default inject(({ webhooksStore }) => {
  const { toggleEventId, isIdChecked, retryWebhookEvent } = webhooksStore;

  return { toggleEventId, isIdChecked, retryWebhookEvent };
})(observer(HistoryTableRow));
