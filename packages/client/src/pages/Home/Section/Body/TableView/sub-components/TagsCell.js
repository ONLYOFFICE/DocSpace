import React from "react";

import Tags from "@docspace/common/components/Tags";

import Tag from "@docspace/components/tag";
import { RoomsTypeTranslations } from "@docspace/common/constants";

const TagsCell = ({ t, item, tagCount, onSelectTag, onSelectOption }) => {
  const styleTagsCell = {
    width: "100%",
    overflow: "hidden",
    display: item.thirdPartyIcon ? "flex" : "",
  };

  return (
    <div style={styleTagsCell}>
      {item.providerType && (
        <Tag
          icon={item.thirdPartyIcon}
          label={item.providerKey}
          onClick={() =>
            onSelectOption({
              option: "typeProvider",
              value: item.providerType,
            })
          }
        />
      )}

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
          onClick={() =>
            onSelectOption({
              option: "defaultTypeRoom",
              value: item.roomType,
            })
          }
        />
      )}
    </div>
  );
};
export default React.memo(TagsCell);
