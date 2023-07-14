import { inject, observer } from "mobx-react";
import React from "react";

import ToggleButton from "@docspace/components/toggle-button";
import Text from "@docspace/components/text";
import { NotificationsType } from "@docspace/common/constants";
import toastr from "@docspace/components/toast/toastr";

const DailyFeedContainer = ({
  t,
  dailyFeedSubscriptions,
  changeSubscription,
  textProps,
  textDescriptionsProps,
}) => {
  const onChangeEmailSubscription = async (e) => {
    const checked = e.currentTarget.checked;
    try {
      await changeSubscription(NotificationsType.DailyFeed, checked);
    } catch (e) {
      toastr.error(e);
    }
  };

  return (
    <div className="notification-container">
      <div>
        <Text {...textProps} className="subscription-title">
          {t("DailyFeed")}
        </Text>
        <Text {...textDescriptionsProps}>{t("DailyFeedDescription")}</Text>
      </div>
      <ToggleButton
        className="daily-feed toggle-btn"
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
