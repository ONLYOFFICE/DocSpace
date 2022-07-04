import React from "react";

import Tags from "@appserver/common/components/Tags";

import TableCell from "@appserver/components/table-container/TableCell";

const TagsCell = React.forwardRef(({ tags, tagCount }, ref) => {
  return (
    <TableCell className="table-container_element-wrapper">
      <div style={{ width: "100%" }} ref={ref}>
        <Tags tags={tags} columnCount={tagCount} />
      </div>
    </TableCell>
  );
});

export default React.memo(TagsCell);
