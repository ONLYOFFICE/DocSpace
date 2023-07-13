import React from "react";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";
import { inject, observer } from "mobx-react";

import { NotificationsType } from "@docspace/common/constants";
import toastr from "@docspace/components/toast/toastr";
const UsefulTipsContainer = ({
  t,
  changeSubscription,
  usefulTipsSubscription,
  textProps,
  textDescriptionsProps,
}) => {
  const onChangeEmailSubscription = async (e) => {
    const checked = e.currentTarget.checked;
    try {
      await changeSubscription(NotificationsType.UsefulTips, checked);
    } catch (e) {
      toastr.error(e);
    }
  };

  return (
    <div className="notification-container">
      <div>
        <Text {...textProps} className="subscription-title">
          {t("UsefulTips")}
        </Text>
        <Text {...textDescriptionsProps}>{t("UsefulTipsDescription")}</Text>
      </div>
      <ToggleButton
        className="useful-tips toggle-btn"
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
