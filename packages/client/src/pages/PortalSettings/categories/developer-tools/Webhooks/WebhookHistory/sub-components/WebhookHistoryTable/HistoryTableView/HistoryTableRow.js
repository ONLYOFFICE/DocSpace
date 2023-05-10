import React, { useMemo } from "react";
import moment from "moment";
import { inject, observer } from "mobx-react";

import { useNavigate } from "react-router-dom";

import TableRow from "@docspace/components/table-container/TableRow";
import TableCell from "@docspace/components/table-container/TableCell";
import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import { StatusBadge } from "../../../../sub-components/StatusBadge";

import Toast from "@docspace/components/toast";
import toastr from "@docspace/components/toast/toastr";

import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";
import InfoIcon from "PUBLIC_DIR/images/info.outline.react.svg?url";

const HistoryTableRow = (props) => {
  const { item, toggleEventId, isIdChecked, retryWebhookEvent } = props;
  const navigate = useNavigate();

  const redirectToDetails = () => navigate(window.location.pathname + `/${item.id}`);
  const handleRetryEvent = async () => {
    await retryWebhookEvent(item.id);
    toastr.success("Webhook retry again", <b>Done</b>);
  };

  const contextOptions = [
    {
      key: "Webhook details dropdownItem",
      label: "Webhook details",
      icon: InfoIcon,
      onClick: redirectToDetails,
    },
    {
      key: "Retry dropdownItem",
      label: "Retry",
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
    <>
      <TableRow contextOptions={contextOptions} checked={isChecked}>
        <TableCell>
          <TableCell checked={isChecked} className="noPadding">
            <Checkbox onChange={onChange} isChecked={isChecked} title="TitleSelectFile" />
          </TableCell>

          <Text fontWeight={600}>{item.id}</Text>
          <Toast />
        </TableCell>
        <TableCell>
          <StatusBadge status={item.status} />
        </TableCell>
        <TableCell>
          <Text fontWeight={600} fontSize="11px">
            {formattedDelivery}
          </Text>
        </TableCell>
      </TableRow>
    </>
  );
};

export default inject(({ webhooksStore }) => {
  const { toggleEventId, isIdChecked, retryWebhookEvent } = webhooksStore;

  return { toggleEventId, isIdChecked, retryWebhookEvent };
})(observer(HistoryTableRow));
