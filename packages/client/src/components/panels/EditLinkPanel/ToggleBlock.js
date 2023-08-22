import React from "react";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";

const ToggleBlock = ({
  isLoading,
  headerText,
  bodyText,
  isChecked,
  onChange,
  children,
  withToggle = true,
  isExpired,
}) => {
  return (
    <div className="edit-link-toggle-block">
      <div className="edit-link-toggle-header">
        <Text fontSize="16px" fontWeight={700}>
          {headerText}
        </Text>
        {withToggle && (
          <ToggleButton
            isDisabled={isLoading}
            isChecked={isChecked}
            onChange={onChange}
            className="edit-link-toggle"
          />
        )}
      </div>
      {bodyText && (
        <Text
          className={
            isExpired
              ? "edit-link-toggle-description_expired"
              : "edit-link-toggle-description"
          }
          fontSize="12px"
          fontWeight={400}
        >
          {bodyText}
        </Text>
      )}

      {children}
    </div>
  );
};

export default ToggleBlock;
