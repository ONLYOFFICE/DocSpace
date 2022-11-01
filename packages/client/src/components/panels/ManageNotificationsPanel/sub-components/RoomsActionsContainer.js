import React from "react";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";
const RoomsActionsContainer = React.memo(
  ({ t, isEnableBadge, onChangeBadgeSubscription }) => {
    return (
      <div className="subscription-container">
        <Text fontSize="15px" fontWeight="600" className="subscription-title">
          {t("RoomsActions")}
        </Text>
        <ToggleButton
          className="toggle-btn"
          label={t("Badges")}
          onChange={onChangeBadgeSubscription}
          isChecked={isEnableBadge}
        />
      </div>
    );
  }
);

export default RoomsActionsContainer;
