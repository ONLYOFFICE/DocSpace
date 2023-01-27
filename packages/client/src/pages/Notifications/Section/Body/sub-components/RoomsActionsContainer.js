import { inject, observer } from "mobx-react";
import React from "react";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";
import { NotificationsType } from "@docspace/common/constants";
const RoomsActionsContainer = ({
  t,
  badgesSubscription,
  changeSubscription,
}) => {
  const onChangeEmailSubscription = (e) => {
    const checked = e.currentTarget.checked;
    changeSubscription(NotificationsType.Badges, checked);
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
          {t("RoomsActions")}
        </Text>
        <Text fontSize="12px">{t("ActionsWithFilesDescription")}</Text>
      </div>
      <ToggleButton
        className="toggle-btn"
        onChange={onChangeEmailSubscription}
        isChecked={badgesSubscription}
      />
    </div>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;

  const { changeSubscription, badgesSubscription } = targetUserStore;

  return {
    changeSubscription,
    badgesSubscription,
  };
})(observer(RoomsActionsContainer));
