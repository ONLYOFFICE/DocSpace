import React from "react";
import styled from "styled-components";
import Text from "@docspace/components/text";
import ToggleButton from "@docspace/components/toggle-button";

const StyledWrapper = styled.div`
  display: flex;
  flex-direction: column;
  gap: 12px;

  .toggle-btn {
    position: relative;

    .toggle-button-text {
      margin-top: -1px;
    }
  }
`;

const Subscription = (props) => {
  const { t, changeEmailSubscription, tipsSubscription } = props;

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

export default Subscription;
