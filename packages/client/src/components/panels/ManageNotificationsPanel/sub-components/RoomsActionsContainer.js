import { inject, observer } from "mobx-react";
import React from "react";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";
const RoomsActionsContainer = ({
  t,
  isEnableTelegram,
  isEnableBadge,
  isEnableEmail,
  onChangeBadgeState,
  onChangeEmailState,
  onChangeTelegramState,
  isTelegramConnected,
}) => {
  const onChangeEmailSubscription = (e) => {
    const checked = e.currentTarget.checked;
    onChangeEmailState(checked);
  };

  const onChangeBadgeSubscription = (e) => {
    const checked = e.currentTarget.checked;
    onChangeBadgeState(checked);
  };

  const onChangeTelegramSubscription = (e) => {
    const checked = e.currentTarget.checked;
    onChangeTelegramState(checked);
  };

  return (
    <div className="subscription-container">
      <Text
        fontSize="15px"
        fontWeight="600"
        className="subscription-title"
        noSelect
      >
        {t("RoomsActions")}
      </Text>
      <ToggleButton
        className="toggle-btn"
        label={t("Badges")}
        onChange={onChangeBadgeSubscription}
        isChecked={isEnableBadge}
      />
      <div className="toggle-btn_next">
        <ToggleButton
          className="toggle-btn"
          label={t("Common:Email")}
          onChange={onChangeEmailSubscription}
          isChecked={isEnableEmail}
        />
      </div>
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
})(observer(RoomsActionsContainer));
