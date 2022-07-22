import React from "react";

import Avatar from "@docspace/components/avatar";
import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";

const Group = ({ data, style, index }) => {
  const { groupList, isMultiSelect, onGroupClick } = data;

  const { label, avatarUrl, total, selectedCount } = groupList[index];

  const isIndeterminate = selectedCount > 0 && selectedCount !== total;

  const isChecked = total !== 0 && total === selectedCount;

  let groupLabel = label;

  if (isMultiSelect && selectedCount > 0) {
    groupLabel = `${label} (${selectedCount})`;
  }

  const onGroupClickAction = React.useCallback(() => {
    onGroupClick && onGroupClick(index);
  }, []);

  return (
    <div
      style={style}
      className="row-option"
      name={`selector-row-option-${index}`}
      onClick={onGroupClickAction}
    >
      <div className="option-info">
        <Avatar
          className="option-avatar"
          role="user"
          size="min"
          source={avatarUrl}
          userName={label}
        />
        <Text
          className="option-text option-text__group"
          truncate={true}
          noSelect={true}
          fontSize="14px"
        >
          {groupLabel}
        </Text>
      </div>
      {isMultiSelect && (
        <Checkbox
          value={`${index}`}
          isChecked={isChecked}
          isIndeterminate={isIndeterminate}
          className="option-checkbox"
        />
      )}
    </div>
  );
};

export default React.memo(Group);
