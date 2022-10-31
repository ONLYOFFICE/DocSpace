import React, { useState } from "react";
import { useTranslation } from "react-i18next";
import { inject, observer } from "mobx-react";
import Text from "@docspace/components/text";
import Button from "@docspace/components/button";
import ManageNotificationsPanel from "../../../../../../components/panels/ManageNotificationsPanel";
import { StyledWrapper } from "./styled-subscriptions";

const Subscription = (props) => {
  const { t } = useTranslation(["Profile", "Common"]);

  const { theme } = props;

  const [
    isManageNotificationsPanelVisible,
    setIsManageNotificationsPanelVisible,
  ] = useState(false);

  const onButtonClick = () => {
    setIsManageNotificationsPanelVisible(true);
  };

  const onClosePanel = () => {
    setIsManageNotificationsPanelVisible(false);
  };

  return (
    <StyledWrapper>
      <Text fontSize="16px" fontWeight={700}>
        {t("Notifications")}
      </Text>

      <Button
        theme={theme}
        scale
        size="normalDesktop"
        label={t("ManageNotifications")}
        onClick={onButtonClick}
      />

      {isManageNotificationsPanelVisible && (
        <ManageNotificationsPanel
          t={t}
          isPanelVisible={isManageNotificationsPanelVisible}
          onClosePanel={onClosePanel}
        />
      )}
    </StyledWrapper>
  );
};

export default inject(({ auth }) => {
  const { theme } = auth.settingsStore;

  return {
    theme,
  };
})(observer(Subscription));
