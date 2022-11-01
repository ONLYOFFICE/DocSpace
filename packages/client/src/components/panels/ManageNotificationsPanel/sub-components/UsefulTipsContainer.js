import React from "react";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";
const UsefulTipsContainer = React.memo(
  ({ t, onChangeTipsEmailState, isEnableEmail }) => {
    const onChangeEmailSubscription = (e) => {
      const checked = e.currentTarget.checked;
      onChangeTipsEmailState(checked);
    };

    return (
      <div className="subscription-container">
        <Text fontSize="15px" fontWeight="600" className="subscription-title">
          {t("UsefulTips")}
        </Text>
        <ToggleButton
          className="toggle-btn"
          label={t("Common:Email")}
          onChange={onChangeEmailSubscription}
          isChecked={isEnableEmail}
        />
      </div>
    );
  }
);

export default UsefulTipsContainer;
