import React, { useMemo } from "react";
import moment from "moment";
import { useNavigate } from "react-router-dom";

import TableRow from "@docspace/components/table-container/TableRow";
import TableCell from "@docspace/components/table-container/TableCell";
import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import { StatusBadge } from "../../../../sub-components/StatusBadge";
import withFileActions from "@docspace/client/src/HOCs/withFileActions";

import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";
import InfoIcon from "PUBLIC_DIR/images/info.outline.react.svg?url";

const HistoryTableRow = (props) => {
  const { item, checkedProps, onContentFileSelect } = props;
  const navigate = useNavigate();

  const redirectToDetails = () => navigate(window.location.pathname + `/${item.id}`);

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
    () => moment(item.delivery).format("MMM D, YYYY, h:mm:ss A") + " UTC",
    [item],
  );

  const onChange = (e) => {
    onContentFileSelect && onContentFileSelect(e.target.checked, item);
  };

  return (
    <TableRow contextOptions={contextOptions} checked={checkedProps}>
      <TableCell>
        <TableCell checked={checkedProps} className="noPadding">
          <Checkbox onChange={onChange} isChecked={checkedProps} title="TitleSelectFile" />
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
    </TableRow>
  );
};

export default withFileActions(HistoryTableRow);
