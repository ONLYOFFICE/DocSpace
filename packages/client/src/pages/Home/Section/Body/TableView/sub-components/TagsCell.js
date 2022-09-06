import React from "react";

import Tags from "@docspace/common/components/Tags";

import Tag from "@docspace/components/tag";
import { RoomsTypeTranslations } from "@docspace/common/constants";

const TagsCell = React.forwardRef(
  ({ t, item, tagCount, onSelectTag, onSelectType }, ref) => {
    return (
      <div style={{ width: "100%", overflow: "hidden" }} ref={ref}>
        {item.tags.length > 0 ? (
          <Tags
            tags={item.tags}
            columnCount={tagCount}
            onSelectTag={onSelectTag}
          />
        ) : (
          <Tag
            isDefault
            label={t(RoomsTypeTranslations[item.roomType])}
            onClick={() => onSelectType(item.roomType)}
          />
        )}
      </div>
    );
  }
);

export default React.memo(TagsCell);
