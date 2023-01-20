import React, { useState } from "react";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";
const UsefulTipsContainer = ({ t, isEnable }) => {
  const [isEnableEmail, setIsEnableEmail] = useState(isEnable);

  const onChangeEmailSubscription = (e) => {
    const checked = e.currentTarget.checked;
    setIsEnableEmail(checked);
  };

  return (
    <div className="notification-container">
      <div>
        <Text
          fontSize="15px"
          fontWeight="600"
          className="subscription-title"
          noSelect
        >
          {t("UsefulTips")}
        </Text>
        <Text fontSize="12px">{t("UsefulTipsDescription")}</Text>
      </div>
      <ToggleButton
        className="toggle-btn"
        onChange={onChangeEmailSubscription}
        isChecked={isEnableEmail}
      />
    </div>
  );
};
export default UsefulTipsContainer;
