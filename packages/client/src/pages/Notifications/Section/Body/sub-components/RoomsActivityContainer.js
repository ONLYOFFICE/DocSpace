import { inject, observer } from "mobx-react";
import React, { useState } from "react";
import ToggleButton from "@docspace/components/toggle-button";
import Text from "@docspace/components/text";

const RoomsActivityContainer = ({ t, isEnable }) => {
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
          {t("RoomsActivity")}
        </Text>
        <Text fontSize="12px">{t("RoomsActivityDescription")}</Text>
      </div>
      <ToggleButton
        className="toggle-btn"
        onChange={onChangeEmailSubscription}
        isChecked={isEnableEmail}
      />
    </div>
  );
};
export default RoomsActivityContainer;
