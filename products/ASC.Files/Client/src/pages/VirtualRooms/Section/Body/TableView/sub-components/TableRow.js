import React from "react";

import TableRow from "@appserver/components/table-container/TableRow";
import TableCell from "@appserver/components/table-container/TableCell";

import FileNameCell from "./FileNameCell";
import TypeCell from "./TypeCell";
import OwnerCell from "./OwnerCell";
import DateCell from "./DateCell";
import SizeCell from "./SizeCell";
import TagsCell from "./TagsCell";

const Row = React.forwardRef(
  ({ item, tagCount, theme, getContextModel }, ref) => {
    return (
      <TableRow
        className="table-row"
        key={item.key}
        contextOptions={getContextModel()}
        getContextModel={getContextModel}
      >
        <FileNameCell
          theme={theme}
          label={item.label}
          type={item.type}
          isPrivacy={item.isPrivacy}
        />
        <TypeCell
          type={item.type}
          sideColor={theme.filesSection.tableView.row.sideColor}
        />
        <TagsCell ref={ref} tags={item.tags} tagCount={tagCount} />
        <OwnerCell sideColor={theme.filesSection.tableView.row.sideColor} />
        <DateCell sideColor={theme.filesSection.tableView.row.sideColor} />
        <SizeCell sideColor={theme.filesSection.tableView.row.sideColor} />
      </TableRow>
    );
  }
);

export default Row;
