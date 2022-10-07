import React from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";

import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";

import { StyledWrapper } from "./styled-subscriptions";

const Subscription = (props) => {
  const { t } = useTranslation("Profile");

  const { changeEmailSubscription, tipsSubscription } = props;

  const onChangeEmailSubscription = (e) => {
    const checked = e.currentTarget.checked;
    changeEmailSubscription(checked);
  };

  return (
    <StyledWrapper>
      <Text fontSize="16px" fontWeight={700}>
        {t("Subscriptions")}
      </Text>
      <ToggleButton
        className="toggle-btn"
        label={t("SubscriptionEmailTipsToggleLbl")}
        onChange={onChangeEmailSubscription}
        isChecked={tipsSubscription}
      />
    </StyledWrapper>
  );
};

export default inject(({ peopleStore }) => {
  const { targetUserStore } = peopleStore;

  const { changeEmailSubscription, tipsSubscription } = targetUserStore;

  return {
    changeEmailSubscription,
    tipsSubscription,
  };
})(observer(Subscription));
