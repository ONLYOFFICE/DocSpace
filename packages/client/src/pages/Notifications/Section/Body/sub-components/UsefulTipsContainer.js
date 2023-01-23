import React from "react";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";
import { inject, observer } from "mobx-react";

const UsefulTipsContainer = ({
  t,
  changeTipsSubscription,
  tipsSubscription,
}) => {
  const onChangeEmailSubscription = async (e) => {
    const checked = e.currentTarget.checked;
    changeTipsSubscription(checked);
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
        isChecked={tipsSubscription}
      />
    </div>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;

  const { changeTipsSubscription, tipsSubscription } = targetUserStore;

  return {
    changeTipsSubscription,
    tipsSubscription,
  };
})(observer(UsefulTipsContainer));
