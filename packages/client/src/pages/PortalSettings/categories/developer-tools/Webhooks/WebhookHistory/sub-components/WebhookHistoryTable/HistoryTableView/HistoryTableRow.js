import React, { useMemo } from "react";
import moment from "moment";
import { useHistory } from "react-router-dom";

import TableRow from "@docspace/components/table-container/TableRow";
import TableCell from "@docspace/components/table-container/TableCell";
import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import { StatusBadge } from "../../../../sub-components/StatusBadge";

import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";
import InfoIcon from "PUBLIC_DIR/images/info.outline.react.svg?url";

export const HistoryTableRow = ({ eventData }) => {
  const history = useHistory();

  const redirectToDetails = () => history.push(window.location.pathname + `/event/${eventData.id}`);

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
    },
  ];

  const formattedDelivery = useMemo(
    () => moment(eventData.delivery).format("MMM D, YYYY, h:mm:ss A") + " UTC",
    [eventData],
  );

  return (
    <TableRow contextOptions={contextOptions}>
      <TableCell>
        <TableCell>
          <Checkbox
            className="table-container_row-checkbox"
            onChange={() => {}}
            isChecked={false}
            title="TitleSelectFile"
            label="abc"
          />
        </TableCell>

        <Text fontWeight={600}>{eventData.id}</Text>
      </TableCell>
      <TableCell>
        <StatusBadge status={eventData.status} />
      </TableCell>
      <TableCell>
        <Text fontWeight={600} fontSize="11px">
          {formattedDelivery}
        </Text>
      </TableCell>
    </TableRow>
  );
};
