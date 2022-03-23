import React from "react";

import Avatar from "@appserver/components/avatar";
import Text from "@appserver/components/text";
import Checkbox from "@appserver/components/checkbox";

const Group = ({
  style,
  index,
  onGroupClick,
  avatarUrl,
  label,
  groupLabel,
  isMultiSelect,
  isIndeterminate,
  isChecked,
}) => {
  return (
    <div
      style={style}
      className="row-option"
      name={`selector-row-option-${index}`}
      onClick={() => onGroupClick(index)}
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
