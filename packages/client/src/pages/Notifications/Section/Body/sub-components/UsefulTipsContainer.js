import React from "react";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";
import { inject, observer } from "mobx-react";
import { NotificationsType } from "@docspace/common/constants";

const UsefulTipsContainer = ({
  t,
  changeSubscription,
  usefulTipsSubscription,
}) => {
  const onChangeEmailSubscription = (e) => {
    const checked = e.currentTarget.checked;
    changeSubscription(NotificationsType.UsefulTips, checked);
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
        isChecked={usefulTipsSubscription}
      />
    </div>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;

  const { changeSubscription, usefulTipsSubscription } = targetUserStore;

  return {
    changeSubscription,
    usefulTipsSubscription,
  };
})(observer(UsefulTipsContainer));
