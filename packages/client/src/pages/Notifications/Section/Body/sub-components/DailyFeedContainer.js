import { inject, observer } from "mobx-react";
import React from "react";
import ToggleButton from "@docspace/components/toggle-button";
import Text from "@docspace/components/text";
import { NotificationsType } from "@docspace/common/constants";

const DailyFeedContainer = ({
  t,
  dailyFeedSubscriptions,
  changeSubscription,
}) => {
  const onChangeEmailSubscription = (e) => {
    const checked = e.currentTarget.checked;
    changeSubscription(NotificationsType.DailyFeed, checked);
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
          {t("DailyFeed")}
        </Text>
        <Text fontSize="12px">{t("DailyFeedDescription")}</Text>
      </div>
      <ToggleButton
        className="toggle-btn"
        onChange={onChangeEmailSubscription}
        isChecked={dailyFeedSubscriptions}
      />
    </div>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;

  const { changeSubscription, dailyFeedSubscriptions } = targetUserStore;

  return {
    changeSubscription,
    dailyFeedSubscriptions,
  };
})(observer(DailyFeedContainer));
