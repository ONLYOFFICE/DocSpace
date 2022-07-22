import React from "react";

import Tags from "@docspace/common/components/Tags";

import Tag from "@docspace/components/tag";

const TagsCell = React.forwardRef(({ t, item, tagCount, onSelectTag }, ref) => {
  return (
    <div style={{ width: "100%", overflow: "hidden" }} ref={ref}>
      {item.tags.length > 0 ? (
        <Tags
          tags={item.tags}
          columnCount={tagCount}
          onSelectTag={onSelectTag}
        />
      ) : (
        <Tag label={t("NoTag")} onClick={onSelectTag} />
      )}
    </div>
  );
});

export default React.memo(TagsCell);
