import React from "react";
import { inject, observer } from "mobx-react";
import { useNavigate, useLocation } from "react-router-dom";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";

import { StyledWrapper } from "./styled-subscriptions";

const Subscription = (props) => {
  const { t } = props;

  const navigate = useNavigate();
  const location = useLocation();

  const onButtonClick = () => {
    navigate(`${location.pathname}/notification`);
  };

  return (
    <StyledWrapper>
      <Text fontSize="16px" fontWeight={700}>
        {t("Notifications:Notifications")}
      </Text>
      <Button
        className="notifications"
        size="small"
        label={t("Notifications:ManageNotifications")}
        onClick={onButtonClick}
      />
    </StyledWrapper>
  );
};

export default inject(({ auth }) => {
  const { theme } = auth.settingsStore;

  return {
    theme,
  };
})(observer(Subscription));
