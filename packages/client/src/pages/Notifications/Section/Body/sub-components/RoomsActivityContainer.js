import { inject, observer } from "mobx-react";
import React from "react";
import ToggleButton from "@docspace/components/toggle-button";
import Text from "@docspace/components/text";
import { NotificationsType } from "@docspace/common/constants";

const RoomsActivityContainer = ({
  t,
  roomsActivitySubscription,
  changeSubscription,
}) => {
  const onChangeEmailSubscription = (e) => {
    const checked = e.currentTarget.checked;
    changeSubscription(NotificationsType.RoomsActivity, checked);
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
