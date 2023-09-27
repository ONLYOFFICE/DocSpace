import { inject, observer } from "mobx-react";

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
      <div className="row">
        <Text {...textProps} className="subscription-title">
          {t("DailyFeed")}
        </Text>
        <ToggleButton
          className="daily-feed"
          onChange={onChangeEmailSubscription}
          isChecked={dailyFeedSubscriptions}
        />
      </div>
      <Text {...textDescriptionsProps}>{t("DailyFeedDescription")}</Text>
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
