import React from "react";

import Tags from "@appserver/common/components/Tags";

const TagsCell = React.forwardRef(({ item, tagCount, onSelectTag }, ref) => {
  return (
    <div style={{ width: "100%" }} ref={ref}>
      <Tags tags={item.tags} columnCount={tagCount} onSelectTag={onSelectTag} />
    </div>
  );
});

export default React.memo(TagsCell);
