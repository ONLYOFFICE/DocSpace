import { inject, observer } from "mobx-react";
import React from "react";
import ToggleButton from "@docspace/components/toggle-button";
import Text from "@docspace/components/text";

const DailyFeedContainer = ({
  t,
  onChangeEmailState,
  onChangeTelegramState,
  isEnableEmail,
  isTelegramConnected,
  isEnableTelegram,
}) => {
  const onChangeEmailSubscription = (e) => {
    const checked = e.currentTarget.checked;
    onChangeEmailState(checked);
  };

  const onChangeTelegramSubscription = (e) => {
    const checked = e.currentTarget.checked;
    onChangeTelegramState(checked);
  };

  return (
    <div className="subscription-container">
      <Text fontSize="15px" fontWeight="600" className="subscription-title">
        {t("DailyFeed")}
      </Text>
      <ToggleButton
        className="toggle-btn"
        label={t("Common:Email")}
        onChange={onChangeEmailSubscription}
        isChecked={isEnableEmail}
      />
      <div className="toggle-btn_next">
        <ToggleButton
          className="toggle-btn"
          label={t("Telegram")}
          onChange={onChangeTelegramSubscription}
          isChecked={isEnableTelegram}
          isDisabled={!isTelegramConnected}
        />
      </div>
    </div>
  );
};
export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;
  const { isTelegramConnected } = targetUserStore;

  return { isTelegramConnected };
})(observer(DailyFeedContainer));
