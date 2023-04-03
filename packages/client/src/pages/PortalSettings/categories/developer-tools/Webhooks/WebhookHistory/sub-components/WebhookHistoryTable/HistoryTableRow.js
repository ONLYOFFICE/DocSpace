import React from "react";
import TableRow from "@docspace/components/table-container/TableRow";
import TableCell from "@docspace/components/table-container/TableCell";
import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";
import InfoIcon from "PUBLIC_DIR/images/info.outline.react.svg?url";

import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import { StatusBadge } from "../../../sub-components/StatusBadge";

export const HistoryTableRow = ({ eventData }) => {
  const contextOptions = [
    {
      key: "Webhook details dropdownItem",
      label: "Webhook details",
      icon: InfoIcon,
    },
    {
      key: "Retry dropdownItem",
      label: "Retry",
      icon: RetryIcon,
    },
  ];

  const onContentSelect = (boolVal, obj) => {};

  const onChange = (e) => {
    onContentSelect && onContentSelect(e.target.checked, item);
  };

  return (
    <TableRow contextOptions={contextOptions}>
      <TableCell>
        <TableCell>
          <Checkbox
            className="table-container_row-checkbox"
            onChange={onChange}
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
          {eventData.delivery}
        </Text>
      </TableCell>
    </TableRow>
  );
};
