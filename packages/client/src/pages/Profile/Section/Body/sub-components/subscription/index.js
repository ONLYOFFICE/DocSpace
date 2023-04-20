import React from "react";
import { inject, observer } from "mobx-react";
import { useNavigate } from "react-router-dom";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";

import { StyledWrapper } from "./styled-subscriptions";

import config from "PACKAGE_FILE";
import { combineUrl } from "@docspace/common/utils";
const Subscription = (props) => {
  const { t } = props;

  const navigate = useNavigate();

  const onButtonClick = () => {
    navigate(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        config.homepage,
        "/accounts/view/@self/notification"
      )
    );
  };

  return (
    <StyledWrapper>
      <Text fontSize="16px" fontWeight={700}>
        {t("Notifications:Notifications")}
      </Text>
      <Button
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
