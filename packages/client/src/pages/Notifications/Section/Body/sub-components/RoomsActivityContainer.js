import { inject, observer } from "mobx-react";
import React from "react";

import ToggleButton from "@docspace/components/toggle-button";
import Text from "@docspace/components/text";
import { NotificationsType } from "@docspace/common/constants";
import toastr from "@docspace/components/toast/toastr";

const RoomsActivityContainer = ({
  t,
  roomsActivitySubscription,
  changeSubscription,
  textProps,
  textDescriptionsProps,
}) => {
  const onChangeEmailSubscription = async (e) => {
    const checked = e.currentTarget.checked;
    try {
      await changeSubscription(NotificationsType.RoomsActivity, checked);
    } catch (e) {
      toastr.error(e);
    }
  };

  return (
    <div className="notification-container">
      <div>
        <Text {...textProps} className="subscription-title">
          {t("RoomsActivity")}
        </Text>
        <Text {...textDescriptionsProps}>{t("RoomsActivityDescription")}</Text>
      </div>
      <ToggleButton
        className="rooms-activity toggle-btn"
        onChange={onChangeEmailSubscription}
        isChecked={roomsActivitySubscription}
      />
    </div>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;

  const { roomsActivitySubscription, changeSubscription } = targetUserStore;

  return {
    roomsActivitySubscription,
    changeSubscription,
  };
})(observer(RoomsActivityContainer));
