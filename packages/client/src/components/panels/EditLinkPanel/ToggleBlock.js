import React from "react";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";

const ToggleBlock = ({
  headerText,
  bodyText,
  isChecked,
  onChange,
  children,
}) => {
  return (
    <div className="edit-link-toggle-block">
      <div className="edit-link-toggle-header">
        <Text fontSize="16px" fontWeight={700}>
          {headerText}
        </Text>
        <ToggleButton
          isChecked={isChecked}
          onChange={onChange}
          className="edit-link-toggle"
        />
      </div>
      <Text
        className="edit-link-toggle-description"
        fontSize="12px"
        fontWeight={400}
      >
        {bodyText}
      </Text>

      {children}
    </div>
  );
};

export default ToggleBlock;
